using YCAnalyzer.Syntaxer.Grammar;
using YCSyntaxer;

namespace YCAnalyzer.Syntaxer {
    /// <summary>
    /// 一个由配置驱动的解析引擎，用于构建 AST。
    /// </summary>
    public class SyntaxBuilder {
        private readonly List<Token> _tokens;
        private readonly GrammarDefinition _grammar;
        private int _current = 0;

        // recursion guard to avoid infinite re-entry of the same rule at the same input position
        private readonly HashSet<string> _recursion_guard = new HashSet<string>();

        // recursion depth guard to prevent StackOverflowException
        private int _recursion_depth = 0;
        private const int MAX_RECURSION_DEPTH = 2000;

        public SyntaxBuilder(List<Token> tokens, GrammarDefinition grammar) {
            _tokens = tokens.Where(t => t.type != TokenType.Whitespace && t.type != TokenType.Comment).ToList();
            _grammar = grammar;
        }

        public Node build() {
            // Try start_rule first, but fall back to any rule that can parse the input.
            Node? ast = parse_rule(_grammar.start_rule, false);
            if (ast == null) {
                foreach (var kv in _grammar.AllRules) {
                    // try each rule by name
                    ast = parse_rule(kv.Key, false);
                    if (ast != null) {
                        break;
                    }

                    _current = 0; // reset position before next attempt
                }
            }

            if (ast == null || !is_at_end()) {
                Token? token = null;

                if (is_at_end()) {
                    if (_tokens.Count > 0) {
                        token = _tokens.Last();
                    }
                    else {
                        token = null;
                    }
                }
                else {
                    token = peek();
                }

                if (token != null) {
                    throw new Exception($"Unexpected token '{token.lexeme}' at line {token.line}, column {token.column} after parsing. Parser stopped at index {_current}.");
                }
                else {
                    throw new Exception($"Unexpected end of input. Parser stopped at index {_current}.");
                }
            }

            return ast!;
        }

        private Node? parse_rule(string rule_name, bool allowConsumeIdentifier) {
            var saved_pos = _current;

            // recursion guard key includes rule name, current input position, and flag
            var guard_key = $"{rule_name}:{allowConsumeIdentifier}:{_current}";
            if (_recursion_guard.Contains(guard_key)) {
                // already trying to parse this rule at this position — avoid infinite recursion
                return null;
            }

            _recursion_guard.Add(guard_key);
            try {
                var rule = _grammar.get_rule(rule_name);
                var node = new Node(rule_name);
                var children = parse_item(rule, allowConsumeIdentifier);
                if (children != null) {
                    node.add_children(children);
                    return node;
                }

                _current = saved_pos; // Backtrack if rule parsing failed
                return null;
            }
            finally {
                _recursion_guard.Remove(guard_key);
            }
        }

        // Only apply small optimizations to parse_item and starts_with_identifier_open_paren where loops are used
        private List<Node>? parse_item(GrammarItem item, bool allowConsumeIdentifier) {
            // recursion depth guard
            _recursion_depth++;
            if (_recursion_depth > MAX_RECURSION_DEPTH) {
                _recursion_depth = 0; // reset to avoid further problems
                throw new Exception($"Maximum parser recursion depth ({MAX_RECURSION_DEPTH}) exceeded. Possible left-recursive or cyclic grammar.");
            }

            try {
                switch (item) {
                    case Sequence seq: {
                            var saved_pos_sequence = _current;
                            var children = new List<Node>();
                            for (int i = 0; i < seq.Items.Length; i++) {
                                var sub_item = seq.Items[i];
                                var result = parse_item(sub_item, allowConsumeIdentifier);
                                if (result != null) {
                                    children.AddRange(result);
                                }
                                else {
                                    if (sub_item is Optional || sub_item is Repetition) {
                                        continue;
                                    }

                                    _current = saved_pos_sequence; // reset
                                    return null;
                                }
                            }

                            return children;
                        }
                    case RuleRef r: {
                            var node = parse_rule(r.Name, allowConsumeIdentifier);
                            if (node != null) {
                                return new List<Node> { node };
                            }

                            return null;
                        }
                    case Literal l: {
                            if (check(l.Type) && peek().lexeme == l.Lexeme) {
                                var token = advance();
                                return new List<Node> { new Node(l.Lexeme, token) };
                            }

                            return null;
                        }
                    case TokenTypeRef t: {
                            if (check(t.Type)) {
                                // Special handling: if this is an Identifier and next token is '(',
                                // prefer parsing a method-like rule. Attempt to parse candidate rules that start with @Identifier '('.
                                if (t.Type == TokenType.Identifier && _current + 1 < _tokens.Count && _tokens[_current + 1].lexeme == "(") {
                                    // Only try candidate rules when not already in a candidate parse to avoid infinite recursion
                                    if (!allowConsumeIdentifier) {
                                        foreach (var kv in _grammar.AllRules) {
                                            if (starts_with_identifier_open_paren(kv.Value)) {
                                                var saved = _current;
                                                var candidateNode = parse_rule(kv.Key, true); // allow identifier consumption inside candidate
                                                if (candidateNode != null) {
                                                    return new List<Node> { candidateNode };
                                                }

                                                _current = saved;
                                            }
                                        }
                                    }
                                }

                                var token = advance();
                                return new List<Node> { new Node(t.Type.ToString(), token, token.literal) };
                            }

                            return null;
                        }
                    case OneOrMore o: {
                            var saved = _current;
                            var first = parse_item(o.Item, allowConsumeIdentifier);
                            if (first == null) {
                                _current = saved;
                                return null;
                            }

                            var collected = new List<Node>();
                            collected.AddRange(first);
                            while (true) {
                                var saved_loop = _current;
                                var more = parse_item(o.Item, allowConsumeIdentifier);
                                if (more != null) {
                                    collected.AddRange(more);
                                }
                                else {
                                    _current = saved_loop;
                                    break;
                                }
                            }

                            return collected;
                        }
                    case Separated s: {
                            var saved = _current;
                            var first = parse_item(s.Item, allowConsumeIdentifier);
                            if (first == null) {
                                _current = saved;
                                return null;
                            }

                            var collected = new List<Node>();
                            collected.AddRange(first);
                            while (true) {
                                var saved_loop = _current;
                                var sep = parse_item(s.Separator, allowConsumeIdentifier);
                                if (sep == null) {
                                    _current = saved_loop;
                                    break;
                                }

                                var next = parse_item(s.Item, allowConsumeIdentifier);
                                if (next == null) {
                                    _current = saved_loop; // rollback to before separator
                                    break;
                                }

                                collected.AddRange(sep);
                                collected.AddRange(next);
                            }

                            return collected;
                        }
                    case Choice c: {
                            var saved_pos_choice = _current;
                            for (int i = 0; i < c.Choices.Length; i++) {
                                var choice = c.Choices[i];
                                var result = parse_item(choice, allowConsumeIdentifier);
                                if (result != null) {
                                    return result;
                                }

                                _current = saved_pos_choice; // Backtrack
                            }

                            return null;
                        }
                    case Optional o: {
                            var optional_result = parse_item(o.Item, allowConsumeIdentifier);
                            if (optional_result != null) {
                                return optional_result;
                            }

                            return new List<Node>();
                        }
                    case Repetition r: {
                            var repeated_children = new List<Node>();
                            while (true) {
                                var saved_pos_repetition = _current;
                                var result = parse_item(r.Item, allowConsumeIdentifier);
                                if (result != null) {
                                    repeated_children.AddRange(result);
                                }
                                else {
                                    _current = saved_pos_repetition;
                                    break;
                                }
                            }

                            return repeated_children;
                        }
                    default: {
                            throw new NotSupportedException($"Grammar item '{item.GetType().Name}' not supported.");
                        }
                }
            }
            finally {
                _recursion_depth--;
            }
        }

        private bool starts_with_identifier_open_paren(GrammarItem item) {
            return starts_with_identifier_open_paren(item, new HashSet<GrammarItem>());
        }

        private bool starts_with_identifier_open_paren(GrammarItem item, HashSet<GrammarItem> visited) {
            if (item == null) {
                return false;
            }

            if (visited.Contains(item)) {
                return false;
            }

            visited.Add(item);

            switch (item) {
                case Sequence seq: {
                        if (seq.Items == null || seq.Items.Length == 0) {
                            return false;
                        }

                        if (seq.Items.Length >= 2 && seq.Items[0] is TokenTypeRef ttr && ttr.Type == TokenType.Identifier && seq.Items[1] is Literal lit && lit.Lexeme == "(") {
                            return true;
                        }

                        return starts_with_identifier_open_paren(seq.Items[0], visited);
                    }
                case Choice ch: {
                        for (int i = 0; i < ch.Choices.Length; i++) {
                            if (starts_with_identifier_open_paren(ch.Choices[i], visited)) {
                                return true;
                            }
                        }

                        return false;
                    }
                default: {
                        return false;
                    }
            }
        }

        private bool check(TokenType type) {
            if (is_at_end()) {
                return false;
            }

            var token = peek();
            return token.type == type;
        }

        private Token advance() {
            if (!is_at_end()) {
                _current++;
            }

            return _tokens[_current - 1];
        }

        private bool is_at_end() {
            return _current >= _tokens.Count;
        }

        private Token peek() {
            return _tokens[_current];
        }
    }
}
