namespace YCAnalyzer {
    public enum TokenType {
        Comment,
        Identifier,
        Integer,
        Long,
        Float,
        Double,
        String,
        Bool,
        Symbol,
        Whitespace,
        Unknown
    }

    public class Token {
        public TokenType type { get; }
        public string lexeme { get; }
        public LiteralValue literal { get; }
        public int line { get; }
        public int column { get; }

        public Token(TokenType type, string lexeme, LiteralValue literal, int line, int column) {
            this.type = type;
            this.lexeme = lexeme;
            this.literal = literal;
            this.line = line;
            this.column = column;
        }

        public override string ToString() {
            return $"{type} {lexeme} {literal}";
        }
    }

    public class AnalizerContext {
        public int line { get; set; }
        public int column { get; set; }
        public int position { get; set; }
        public System.Text.StringBuilder current_lexeme { get; } = new System.Text.StringBuilder();
        public string source { get; }

        public AnalizerContext(string source) {
            this.source = source;
            line = 1;
            column = 1;
            position = 0;
        }

        public void reset_lexeme() {
            current_lexeme.Clear();
        }
    }
}
