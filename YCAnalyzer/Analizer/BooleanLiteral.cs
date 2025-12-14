namespace YCAnalyzer.Analizer {
    public class BooleanLiteral : IAnalizer {
        public bool scan(StringReader reader, AnalizerContext context) {
            if (context.position >= context.source.Length) {
                return false;
            }
            // Peek at the next characters without consuming them from the main reader
            string remaining_source = context.source.Substring(context.position);

            if (remaining_source.StartsWith("true")) {
                // Check for a word boundary to avoid matching "true_story"
                if (remaining_source.Length == 4 || !char.IsLetterOrDigit(remaining_source[4])) {
                    // It's a match, now consume the characters from the actual reader
                    reader.Read(new char[4], 0, 4);
                    context.current_lexeme.Append("true");
                    return true;
                }
            }
            else if (remaining_source.StartsWith("false")) {
                // Check for a word boundary
                if (remaining_source.Length == 5 || !char.IsLetterOrDigit(remaining_source[5])) {
                    // It's a match, consume
                    reader.Read(new char[5], 0, 5);
                    context.current_lexeme.Append("false");
                    return true;
                }
            }

            return false;
        }

        private bool is_word_boundary(StringReader reader) {
            int peek = reader.Peek();
            return peek == -1 || !char.IsLetterOrDigit((char)peek);
        }

        public Token create_token(AnalizerContext context) {
            string lexeme = context.current_lexeme.ToString();
            bool value = bool.Parse(lexeme);
            return new Token(TokenType.Bool, lexeme, new LiteralValue(value), context.line, context.column);
        }
    }
}
