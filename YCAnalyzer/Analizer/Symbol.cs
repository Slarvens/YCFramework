namespace YCAnalyzer.Analizer {
    public class Symbol : IAnalizer {
        public bool scan(StringReader reader, AnalizerContext context) {
            int p = reader.Peek();
            if (p == -1) {
                return false;
            }

            char c = (char)p;
            if (is_symbol(c)) {
                context.current_lexeme.Append(c);
                reader.Read();
                return true;
            }
            return false;
        }

        public Token create_token(AnalizerContext context) {
            return new Token(TokenType.Symbol, context.current_lexeme.ToString(), new LiteralValue(), context.line, context.column);
        }

        private static bool is_symbol(char c) {
            return !char.IsLetterOrDigit(c) && !char.IsWhiteSpace(c);
        }
    }
}
