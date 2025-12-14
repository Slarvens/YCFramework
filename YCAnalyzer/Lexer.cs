using YCAnalyzer.Analizer;

namespace YCAnalyzer {
    /// <summary>
    /// 一个基于策略模式的词法分析器，使用一组 IAnalizer 来解析 Token。
    /// </summary>
    public class Lexer {
        private readonly List<IAnalizer> _analizers;

        public Lexer() {
            // 初始化所有分析器策略（顺序决定优先级）
            _analizers = new List<IAnalizer> {
 new Comment(),
 new Whitespace(),
 new BooleanLiteral(),
 new Identifier(),
 new Number(),
 new StringLiteral(),
 new Symbol()
 };
        }

        public List<Token> parsing(string content) {
            var tokens = new List<Token>();
            var context = new AnalizerContext(content);
            using var reader = new System.IO.StringReader(content);

            var analyzers = _analizers; // local cache
            while (reader.Peek() != -1) {
                bool matched = false;

                for (int i = 0; i < analyzers.Count; i++) {
                    var analizer = analyzers[i];

                    context.reset_lexeme();

                    //让分析器尝试从当前 reader位置扫描
                    if (analizer.scan(reader, context)) {
                        // 空白和注释不产出 Token
                        if (analizer is not Whitespace && analizer is not Comment) {
                            var token = analizer.create_token(context);
                            if (token != null) {
                                tokens.Add(token);
                                // Update position after a successful token creation
                                context.position += token.lexeme.Length;
                            }
                        }
                        else {
                            // For whitespace and comments, we still need to advance the position
                            context.position += context.current_lexeme.Length;
                        }

                        matched = true;
                        break; // 本轮匹配成功，进入下一轮
                    }
                }

                if (!matched) {
                    // 所有分析器都无法匹配，则消费一个字符作为 Unknown
                    char c = (char)reader.Read();
                    context.current_lexeme.Append(c);
                    tokens.Add(new Token(TokenType.Unknown, c.ToString(), new LiteralValue(c.ToString()), context.line, context.column));
                    context.position++;
                    context.reset_lexeme();
                }
            }

            return tokens;
        }
    }
}
