namespace YCAnalyzer.Analizer {
    public class StringLiteral : IAnalizer {
        public bool scan(StringReader reader, AnalizerContext context) {
            int peek_val = reader.Peek();
            if (peek_val == '"') {
                context.current_lexeme.Append((char)reader.Read()); // Consume opening quote
                int q_val;
                while ((q_val = reader.Peek()) != -1 && q_val != '"') {
                    context.current_lexeme.Append((char)reader.Read());
                }

                if (reader.Peek() == '"') {
                    context.current_lexeme.Append((char)reader.Read()); // Consume closing quote
                    return true;
                }
            }
            return false;
        }

        public Token create_token(AnalizerContext context) {
            string lexeme = context.current_lexeme.ToString();
            string value_str = lexeme.Substring(1, lexeme.Length - 2);
            return new Token(TokenType.String, lexeme, new LiteralValue(value_str), context.line, context.column);
        }
    }
}
