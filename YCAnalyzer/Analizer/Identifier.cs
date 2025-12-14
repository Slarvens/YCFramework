namespace YCAnalyzer.Analizer {
    public class Identifier : IAnalizer {
        public bool scan(StringReader reader, AnalizerContext context) {
            int peek_val = reader.Peek();
            if (peek_val == -1) {
                return false;
            }

            char c = (char)peek_val;
            if (char.IsLetter(c) || c == '_') {
                context.current_lexeme.Append(c);
                reader.Read();
                int p_val;
                while ((p_val = reader.Peek()) != -1) {
                    char ch_val = (char)p_val;
                    if (char.IsLetterOrDigit(ch_val) || ch_val == '_') {
                        context.current_lexeme.Append((char)reader.Read());
                    }
                    else {
                        break;
                    }
                }
                return true;
            }
            return false;
        }

        public Token create_token(AnalizerContext context) {
            string lexeme = context.current_lexeme.ToString();
            return new Token(TokenType.Identifier, lexeme, new LiteralValue(lexeme), context.line, context.column);
        }
    }
}