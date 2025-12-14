namespace YCAnalyzer.Analizer {
    public class Number : IAnalizer {
        public bool scan(StringReader reader, AnalizerContext context) {
            int peek_val = reader.Peek();
            if (peek_val == -1) {
                return false;
            }

            // Handle 0x (hex) or 0o (octal) prefixes
            if (peek_val == '0') {
                // Look ahead using context.source and context.position to avoid allocating a temp reader
                int next_pos = context.position + 1;
                if (next_pos < context.source.Length) {
                    char next_char = context.source[next_pos];
                    if (next_char == 'x' || next_char == 'X') {
                        return scan_hex(reader, context);
                    }
                    if (next_char == 'o' || next_char == 'O') {
                        return scan_octal(reader, context);
                    }
                }
            }

            // Standard decimal, float, or scientific notation
            if (char.IsDigit((char)peek_val) || peek_val == '.') {
                return scan_decimal(reader, context);
            }

            return false;
        }

        private bool scan_hex(StringReader reader, AnalizerContext context) {
            context.current_lexeme.Append((char)reader.Read()); // 0
            context.current_lexeme.Append((char)reader.Read()); // x or X

            int p_val = reader.Peek();
            if (!is_hex_digit((char)p_val)) {
                return false; // Must have at least one digit after 0x
            }

            while (reader.Peek() != -1 && is_hex_digit((char)reader.Peek())) {
                context.current_lexeme.Append((char)reader.Read());
            }
            return true;
        }

        private bool scan_octal(StringReader reader, AnalizerContext context) {
            context.current_lexeme.Append((char)reader.Read()); // 0
            context.current_lexeme.Append((char)reader.Read()); // o or O

            int p_val = reader.Peek();
            if (!is_octal_digit((char)p_val)) {
                return false; // Must have at least one digit after 0o
            }

            while (reader.Peek() != -1 && is_octal_digit((char)reader.Peek())) {
                context.current_lexeme.Append((char)reader.Read());
            }
            return true;
        }

        private bool scan_decimal(StringReader reader, AnalizerContext context) {
            bool has_digits = false;
            // Integer part
            while (reader.Peek() != -1 && char.IsDigit((char)reader.Peek())) {
                has_digits = true;
                context.current_lexeme.Append((char)reader.Read());
            }

            // Fractional part
            if (reader.Peek() == '.') {
                context.current_lexeme.Append((char)reader.Read());
                while (reader.Peek() != -1 && char.IsDigit((char)reader.Peek())) {
                    has_digits = true;
                    context.current_lexeme.Append((char)reader.Read());
                }
            }

            if (!has_digits) {
                return false;
            }

            // Scientific notation
            int p_val = reader.Peek();
            if (p_val == 'e' || p_val == 'E') {
                context.current_lexeme.Append((char)reader.Read());
                if (reader.Peek() == '+' || reader.Peek() == '-') {
                    context.current_lexeme.Append((char)reader.Read());
                }
                if (reader.Peek() == -1 || !char.IsDigit((char)reader.Peek())) {
                    return true; // "1e" is valid, exponent is optional
                }

                while (reader.Peek() != -1 && char.IsDigit((char)reader.Peek())) {
                    context.current_lexeme.Append((char)reader.Read());
                }
            }

            // Float/Double/Long suffix
            int suffix_char = reader.Peek();
            if (suffix_char == 'f' || suffix_char == 'F' || suffix_char == 'd' || suffix_char == 'D' || suffix_char == 'l' || suffix_char == 'L') {
                context.current_lexeme.Append((char)reader.Read());
            }

            return true;
        }

        public Token create_token(AnalizerContext context) {
            string lexeme = context.current_lexeme.ToString();
            LiteralValue literal;
            TokenType type;
            string normalized_lexeme = lexeme.ToLowerInvariant();

            try {
                if (normalized_lexeme.Contains('.') || normalized_lexeme.Contains('e') || normalized_lexeme.EndsWith('f') || normalized_lexeme.EndsWith('d')) {
                    if (normalized_lexeme.EndsWith('f')) {
                        type = TokenType.Float;
                        literal = new LiteralValue(float.Parse(lexeme.Substring(0, lexeme.Length - 1), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
                    }
                    else {
                        type = TokenType.Double;
                        string double_str_val = normalized_lexeme.EndsWith('d') ? lexeme.Substring(0, lexeme.Length - 1) : lexeme;
                        literal = new LiteralValue(double.Parse(double_str_val, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture));
                    }
                }
                else {
                    if (normalized_lexeme.EndsWith("l")) {
                        type = TokenType.Long;
                        literal = new LiteralValue(long.Parse(lexeme.Substring(0, lexeme.Length - 1)));
                    }
                    else {
                        type = TokenType.Integer;
                        if (lexeme.StartsWith("0x") || lexeme.StartsWith("0X")) {
                            literal = new LiteralValue(Convert.ToInt32(lexeme.Substring(2), 16));
                        }
                        else if (lexeme.StartsWith("0o") || lexeme.StartsWith("0O")) {
                            literal = new LiteralValue(Convert.ToInt32(lexeme.Substring(2), 8));
                        }
                        else {
                            literal = new LiteralValue(int.Parse(lexeme));
                        }
                    }
                }
                return new Token(type, lexeme, literal, context.line, context.column);
            }
            catch (System.Exception) {
                return new Token(TokenType.Unknown, lexeme, new LiteralValue(), context.line, context.column);
            }
        }

        private bool is_hex_digit(char c) => char.IsDigit(c) || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');
        private bool is_octal_digit(char c) => c >= '0' && c <= '7';
    }
}
