namespace YCAnalyzer.Analizer {
    public class Whitespace : IAnalizer {
        public bool scan(StringReader reader, AnalizerContext context) {
            int peek_val = reader.Peek();
            if (peek_val != -1 && char.IsWhiteSpace((char)peek_val)) {
                int p_val;
                while ((p_val = reader.Peek()) != -1 && char.IsWhiteSpace((char)p_val)) {
                    char c_val = (char)reader.Read();
                    context.current_lexeme.Append(c_val);

                    if (c_val == '\n') {
                        context.line++;
                        context.column = 1;
                    }
                    else {
                        context.column++;
                    }
                    context.position++;
                }
                return true;
            }
            return false;
        }

        public Token create_token(AnalizerContext context) {
            return new Token(TokenType.Whitespace, context.current_lexeme.ToString(), new LiteralValue(), context.line, context.column);
        }
    }
}
