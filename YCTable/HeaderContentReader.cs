using System;
using System.Collections.Generic;
using YCAnalyzer;
using YCAnalyzer.Syntaxer;
using YCAnalyzer.Syntaxer.Grammar;
using YCSyntaxer;
using YCTable.Handlers.Header;

namespace YCTable {
    // Represents one header column's three parts
    public readonly record struct HeaderColumn(string variable, string type, string comment);


    public class HeaderContentReader {
        public static string header_rule_config = @"
        header_nullable: basic_type ('?')? | header_json | header_array | header_tuple | header_dict;
        // basic_type lists the primitive/identifier options used as atomic types (json is excluded here)
        basic_type: @Identifier | 'Integer' | 'Long' | 'Float' | 'Double' | 'String' | 'Bool';
        // dedicated json rule so parser can detect json specifically
        header_json: 'json';
        // primary types are either basic types or json
        header_primary: basic_type | header_json;
        // tuple items must be non-json basic types (nullable allowed on items if desired)
        element_type: basic_type ('?')?;
        header_array: element_type '[' ']';
        header_tuple: '(' element_type (',' element_type)* ')';
        // use basic_type for dict keys (keys must be basic types)
        header_dict: 'dict' '<' basic_type ',' element_type '>';
        ";
        public GrammarParser gp;
        private GrammarDefinition grammar;
        private Lexer lexer;
        private AstProcessor ast_processor;

        public HeaderContentReader() {
            gp = new GrammarParser();
            grammar = gp.parse(header_rule_config);
            lexer = new Lexer();
            ast_processor = new AstProcessor();

        }
        private void register_header_handlers(ExcelTable context) {
            ast_processor.register(new HeaderPrimaryTypeHandler(context));
            ast_processor.register(new HeaderTupleHandler(context));
            ast_processor.register(new HeaderNullableHandler(context));
            ast_processor.register(new HeaderArrayHandler(context));
            ast_processor.register(new HeaderDictHandler(context));
            ast_processor.register(new HeaderJsonHandler(context));
            ast_processor.register(new HeaderElementTypeHandler(context));
        }
        // Return header contents as array of HeaderColumn; this method also updates context.read_row_count
        private HeaderColumn[] get_header_contents(ExcelTable context) {
            // start from table context
            var height = context.size.y;

            var collected = new List<string[]>();
            int row = context.start_index.y; // start from row0 by design

            while (collected.Count < 3 && row < height) {
                var current = context.get_row(row);
                if (current == null || current.Length == 0) { row++; continue; }

                var first = current.Length > 0 ? current[0] : null;
                if (!string.IsNullOrEmpty(first) && first.Contains("#")) { row++; continue; }

                collected.Add(current);
                row++;
            }

            string[] temp_variable = collected.Count > 0 ? collected[0] : Array.Empty<string>();
            string[] temp_type = collected.Count > 1 ? collected[1] : Array.Empty<string>();
            string[] temp_comments = collected.Count > 2 ? collected[2] : Array.Empty<string>();

            if (temp_variable.Length == 0) throw new InvalidOperationException($"Failed to read variable names starting at row {0}");
            if (temp_type.Length == 0) throw new InvalidOperationException($"Failed to read type names starting at row {0}");

            // determine number of columns to return
            int max_cols = Math.Max(temp_variable.Length, Math.Max(temp_type.Length, temp_comments.Length));
            var columns = new HeaderColumn[max_cols];

            for (int i = 0; i < max_cols; i++) {
                var v = i < temp_variable.Length ? temp_variable[i] : null;
                var t = i < temp_type.Length ? temp_type[i] : null;
                var c = i < temp_comments.Length ? temp_comments[i] : null;
                columns[i] = new HeaderColumn(v, t, c);
            }

            // Update read count on the table to indicate how many rows were scanned
            context.read_row_count = row;

            return columns;
        }

        // process_headers now simply calls get_header_contents and relies on it to update read count on the table
        public void process_headers(ExcelTable context) {
            var columns = get_header_contents(context);
            register_header_handlers(context);
            parse_columns(columns);
        }

        // Parse the column type strings into AST nodes using the header grammar and return array of AST roots.
        public void parse_columns(HeaderColumn[] columns) {
            if (columns == null) throw new ArgumentNullException(nameof(columns));

            for (int i = 0; i < columns.Length; i++) {
                var type_text = columns[i].type;
                try {
                    var tokens = lexer.parsing(type_text);
                    var builder = new SyntaxBuilder(tokens, grammar);
                    var ast = builder.build();
                    ast_processor.process(ast);
                    //TODO这里一会继续写
                }
                catch (Exception ex) {
                    throw new Exception($"Failed to parse header type for column {i}: '{type_text}'. Error: {ex.Message}", ex);
                }
            }

        }


    }
}
