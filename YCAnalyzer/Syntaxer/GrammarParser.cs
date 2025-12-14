using System.Text.RegularExpressions;
using YCAnalyzer;
using YCAnalyzer.Syntaxer.Grammar;

namespace YCSyntaxer {
    /// <summary>
    /// 用于解析类 ANTLR 风格语法定义字符串的元解析器
    /// </summary>
    public class GrammarParser {
        private record MetaToken(string Type, string Value);
        private List<MetaToken> _meta_tokens = new List<MetaToken>();
        private int _current =0;

        // 以下为正则->MetaToken 映射中使用的命名捕获组名称常量
        //这些名称对应于组合正则表达式中的命名捕获组
        private const string TOKEN_LITERAL = "LITERAL"; // 字面量，例如 'if'
        private const string TOKEN_AT_ID = "AT_ID"; // @ 标记的标识符映射
        private const string TOKEN_ID = "ID"; // 普通标识符
        private const string TOKEN_COLON = "COLON"; // ':'
        private const string TOKEN_PIPE = "PIPE"; // '|'
        private const string TOKEN_SEMICOLON = "SEMICOLON"; // ';'
        private const string TOKEN_LPAREN = "LPAREN"; // '('
        private const string TOKEN_RPAREN = "RPAREN"; // ')'
        private const string TOKEN_LBRACK = "LBRACK"; // '['
        private const string TOKEN_RBRACK = "RBRACK"; // ']'
        private const string TOKEN_L_ANGLE = "L_ANGLE"; // '<'
        private const string TOKEN_R_ANGLE = "R_ANGLE"; // '>'
        private const string TOKEN_LBRACE = "LBRACE"; // '{'
        private const string TOKEN_RBRACE = "RBRACE"; // '}'
        private const string TOKEN_HASH = "HASH"; // '#'
        private const string TOKEN_QUESTION = "QUESTION"; // '?'
        private const string TOKEN_STAR = "STAR"; // '*'
        private const string TOKEN_PLUS = "PLUS"; // '+'
        private const string TOKEN_COMMA = "COMMA"; // ',')
        private const string TOKEN_COMMENT_MULTI = "COMMENT_MULTI"; // 多行注释 /* .. */
        private const string TOKEN_COMMENT_SINGLE = "COMMENT_SINGLE"; // 单行注释 // ..
        private const string TOKEN_WHITESPACE = "WHITESPACE"; // 空白字符
        private const string TOKEN_UNKNOWN = "UNKNOWN"; //其它任意单字符

        // Cache token patterns and compiled regex to avoid reallocating and recompiling on every lex call
        private static readonly KeyValuePair<string, string>[] TOKEN_PATTERNS = new[] {
            new KeyValuePair<string,string>(TOKEN_LITERAL, @"'[^']*'"),
            new KeyValuePair<string,string>(TOKEN_AT_ID, @"@[a-zA-Z_][a-zA-Z0-9_]*"),
            new KeyValuePair<string,string>(TOKEN_ID, @"[a-zA-Z_][a-zA-Z0-9_]*"),
            new KeyValuePair<string,string>(TOKEN_COLON, @":"),
            new KeyValuePair<string,string>(TOKEN_PIPE, @"\|"),
            new KeyValuePair<string,string>(TOKEN_SEMICOLON, @";"),
            new KeyValuePair<string,string>(TOKEN_LPAREN, @"\("),
            new KeyValuePair<string,string>(TOKEN_RPAREN, @"\)"),
            new KeyValuePair<string,string>(TOKEN_LBRACK, @"\["),
            new KeyValuePair<string,string>(TOKEN_RBRACK, @"\]"),
            new KeyValuePair<string,string>(TOKEN_L_ANGLE, @"<"),
            new KeyValuePair<string,string>(TOKEN_R_ANGLE, @">") ,
            new KeyValuePair<string,string>(TOKEN_LBRACE, @"\{"),
            new KeyValuePair<string,string>(TOKEN_RBRACE, @"\}"),
            new KeyValuePair<string,string>(TOKEN_HASH, @"#"),
            new KeyValuePair<string,string>(TOKEN_QUESTION, @"\?"),
            new KeyValuePair<string,string>(TOKEN_STAR, @"\*"),
            new KeyValuePair<string,string>(TOKEN_PLUS, @"\+"),
            new KeyValuePair<string,string>(TOKEN_COMMA, @",") ,
            new KeyValuePair<string,string>(TOKEN_COMMENT_MULTI, @"/\*[\s\S]*?\*/"),
            new KeyValuePair<string,string>(TOKEN_COMMENT_SINGLE, @"//[^\r\n]*"),
            new KeyValuePair<string,string>(TOKEN_WHITESPACE, @"\s+"),
            new KeyValuePair<string,string>(TOKEN_UNKNOWN, @".")
        };

        private static readonly Regex TOKEN_REGEX = new Regex(
            string.Join("|", TOKEN_PATTERNS.Select(kv => $"(?<{kv.Key}>{kv.Value})")),
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        // 将 @xxx 映射到 TokenType（不区分大小写）
        private static readonly Dictionary<string, TokenType> s_atIdTokenMap =
            new(StringComparer.OrdinalIgnoreCase) {
                ["identifier"] = TokenType.Identifier,
                ["integer"] = TokenType.Integer,
                ["long"] = TokenType.Long,
                ["float"] = TokenType.Float,
                ["double"] = TokenType.Double,
                ["string"] = TokenType.String,
                ["bool"] = TokenType.Bool
            };

        private static GrammarItem CreateAtIdItem(string rawName) {
            var name = rawName.ToLowerInvariant();
            if (name == "number") {
                return new Choice(new GrammarItem[] {
                    new TokenTypeRef(TokenType.Integer),
                    new TokenTypeRef(TokenType.Long),
                    new TokenTypeRef(TokenType.Float),
                    new TokenTypeRef(TokenType.Double)
                });
            }

            if (!s_atIdTokenMap.TryGetValue(name, out var tokenType)) {
                throw new Exception($"Unknown @ token type: '{rawName}'");
            }

            return new TokenTypeRef(tokenType);
        }

        private static GrammarItem CreateLiteralItem(string lexeme) {
            // 避免使用 LINQ：判断 lexeme 是否全部由字母组成
            bool allLetters = true;
            for (int i =0; i < lexeme.Length; i++) {
                if (!char.IsLetter(lexeme[i])) {
                    allLetters = false;
                    break;
                }
            }

            TokenType tokenType;
            if (allLetters) {
                tokenType = TokenType.Identifier;
            }
            else {
                tokenType = TokenType.Symbol;
            }

            return new Literal(lexeme, tokenType);
        }

        public GrammarDefinition parse(string grammar_text) {
            _meta_tokens = lex(grammar_text);
            _current =0;
            var rules = new Dictionary<string, GrammarItem>();

            while (!is_at_end()) {
                var rule_name = consume(TOKEN_ID, "Expected rule name.").Value;
                consume(TOKEN_COLON, "Expected ':' after rule name.");
                rules[rule_name] = parse_choice();
                consume(TOKEN_SEMICOLON, "Expected ';' after rule definition.");
            }

            if (rules.Count ==0) {
                throw new Exception("No grammar rules were parsed.");
            }

            //选择第一个规则作为起始规则
            string startRule = null!;
            foreach (var k in rules.Keys) {
                startRule = k;
                break;
            }

            return new GrammarDefinition(startRule, rules);
        }

        private GrammarItem parse_choice() {
            var items = new List<GrammarItem> { parse_sequence() };
            while (match(TOKEN_PIPE)) {
                items.Add(parse_sequence());
            }

            if (items.Count ==1) {
                return items[0];
            }

            return new Choice(items.ToArray());
        }

        private GrammarItem parse_sequence() {
            var items = new List<GrammarItem>();
            //读取直到遇到 ')' 或 '|' 或 ';'
            while (!is_at_end() && !check(TOKEN_PIPE) && !check(TOKEN_SEMICOLON) && !check(TOKEN_RPAREN)) {
                items.Add(parse_term());
            }

            if (items.Count ==1) {
                return items[0];
            }

            return new Sequence(items.ToArray());
        }

        private GrammarItem parse_term() {
            GrammarItem item;

            if (match(TOKEN_LPAREN)) {
                item = parse_choice();
                consume(TOKEN_RPAREN, "Expected ')' after group.");
            }
            else if (check(TOKEN_AT_ID)) {
                var raw = consume(TOKEN_AT_ID, "Expected @-identifier.").Value.Substring(1);
                item = CreateAtIdItem(raw);
            }
            else if (check(TOKEN_LITERAL)) {
                var literalValue = consume(TOKEN_LITERAL, "Expected literal.").Value.Trim('\'');
                item = CreateLiteralItem(literalValue);
            }
            else if (check(TOKEN_ID)) {
                var name = consume(TOKEN_ID, "Expected identifier.").Value;
                item = new RuleRef(name);
            }
            else {
                var token = peek();
                item = CreateLiteralItem(token.Value);
                consume(token.Type, "Unexpected error consuming token.");
            }

            if (match(TOKEN_QUESTION)) {
                return new Optional(item);
            }

            if (match(TOKEN_STAR)) {
                return new Repetition(item);
            }

            if (match(TOKEN_PLUS)) {
                return new OneOrMore(item);
            }

            return item;
        }

        private List<MetaToken> lex(string text) {
            var matches_list = TOKEN_REGEX.Matches(text);
            var results_list = new List<MetaToken>(matches_list.Count);

            for (int i =0; i < matches_list.Count; i++) {
                var m = matches_list[i];
                string found_kind = null!;
                foreach (var kvp in TOKEN_PATTERNS) {
                    if (m.Groups[kvp.Key].Success) {
                        found_kind = kvp.Key;
                        break;
                    }
                }

                // 跳过空白和注释
                if (found_kind == TOKEN_WHITESPACE || found_kind == TOKEN_COMMENT_SINGLE || found_kind == TOKEN_COMMENT_MULTI) {
                    continue;
                }

                if (found_kind == TOKEN_UNKNOWN) {
                    throw new Exception($"Unknown char in grammar: {m.Value}");
                }

                results_list.Add(new MetaToken(found_kind, m.Value));
            }

            return results_list;
        }

        private bool match(string type) {
            if (check(type)) {
                _current++;
                return true;
            }

            return false;
        }

        private MetaToken consume(string type, string error) {
            if (check(type)) {
                var mt = _meta_tokens[_current++];
                return mt;
            }

            if (is_at_end()) {
                throw new Exception($"{error} Reached end of input.");
            }

            throw new Exception($"{error} Got '{peek().Value}' instead.");
        }

        private bool check(string type) {
            if (is_at_end()) {
                return false;
            }

            return _meta_tokens[_current].Type == type;
        }

        private bool is_at_end() {
            return _current >= _meta_tokens.Count;
        }

        private MetaToken peek() {
            return _meta_tokens[_current];
        }
    }
}
