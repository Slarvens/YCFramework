namespace YCAnalyzer.Analizer {
    public class Comment : IAnalizer {

        public bool scan(StringReader reader, AnalizerContext context) {
            int peek_val = reader.Peek();
            if (peek_val != '/') {
                return false;
            }

            reader.Read(); // Consume '/'.

            int q_val = reader.Peek();
            if (q_val == '/') // Single-line comment.
            {
                reader.Read(); // Consume second '/'.
                context.current_lexeme.Append("//");

                int r_val;
                while ((r_val = reader.Peek()) != -1 && r_val != '\n') {
                    context.current_lexeme.Append((char)reader.Read());
                }
                return true;
            }
            else if (q_val == '*') // Multi-line comment.
            {
                reader.Read(); // Consume '*'
                context.current_lexeme.Append("/*");

                int r_val;
                while ((r_val = reader.Peek()) != -1) {
                    char c_val = (char)reader.Read();
                    context.current_lexeme.Append(c_val);

                    if (c_val == '\n') {
                        context.line++;
                        context.column = 0;
                    }

                    context.column++;
                    context.position++;

                    if (c_val == '*' && reader.Peek() == '/') {
                        context.current_lexeme.Append((char)reader.Read());
                        context.column++;
                        context.position++;
                        break;
                    }
                }
                return true;
            }

            // It was not a comment, just a single '/'.
            // Add it to the lexeme and let the Symbol analyzer handle it.
            context.current_lexeme.Append('/');
            return false;
        }
        public Token create_token(AnalizerContext context) {
            return new Token(TokenType.Comment, context.current_lexeme.ToString(), new LiteralValue(), context.line, context.column);
        }
    }
}
