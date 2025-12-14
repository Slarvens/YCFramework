namespace YCAnalyzerProject {
    public class Program_DebugOld {
        //       static void Main(string[] args) {
        //           //1. 定义 ANTLR 风格的语法配置
        //           string rule_config = @"
        //script: field ('#' @Number)?;
        //field: multi_column_type '?'?;
        //multi_column_type: column_type (',' column_type)*;
        //column_type: method
        //| dictionary
        //| element_type ( ('key') | ('[' ']') )?
        //| 'json'
        //| 'json_string'
        //| '[' multi_column_type ']';
        //dictionary: 'dict' '<' element_type ',' element_type '>';
        //element_type: basic_type | tuple | method | array_literal;
        //tuple: '(' tuple_items ')';
        //tuple_items: element_type (',' element_type)*;
        //method: @Identifier '(' method_params? ')';
        //method_params: (method | basic_type) (',' (method | basic_type))*;
        //array_literal: '[' array_items? ']';
        //array_items: basic_type (',' basic_type)*;
        //basic_type: @Integer | @Long | @Float | @Double | @String | @Bool | @Identifier;
        //";

        //           var grammar_parser = new GrammarParser();
        //           var grammar = grammar_parser.parse(rule_config);
        //           var test_inputs = new List<string>
        //           {
        //"123",
        //"1234567890123L",
        //"123.45f",
        //"123.45",
        //"true",
        //"\"a string\"",
        //"my_basic_identifier",
        //"(1, \"hello\", true)",
        //"dict<1,2.5>",
        //"dict<my_key, (1, false)>",
        //"my_method()",
        //"another_method(123)",
        //"complex_method(true, \"hello\",3.14f)",
        //"outer()",
        //"outer(inner(1), inner2(2,3))",
        //"my_identifier",
        //"my_identifier key",
        //"(1,2) key",
        //"my_identifier[]",
        //"(true, false)[]",
        //"json",
        //"json_string",
        //"[my_id, dict<1,2>]",
        //"[1,2,3]",
        //"[]",
        //"my_identifier?"
        //};

        //           var lexer = new Lexer();
        //           var ast_processor = new AstProcessor();
        //           ast_processor.register(new YCAnalyzerProject.Handlers.BasicTypeHandler());
        //           ast_processor.register(new YCAnalyzerProject.Handlers.MethodHandler());
        //           ast_processor.register(new YCAnalyzerProject.Handlers.TupleHandler());
        //           ast_processor.register(new YCAnalyzerProject.Handlers.ArrayLiteralHandler());
        //           ast_processor.register(new YCAnalyzerProject.Handlers.DictionaryHandler());

        //           Console.WriteLine("--- Testing Scheme A: Recursive Descent (SyntaxBuilder) ---");
        //           foreach (var input_string in test_inputs) {
        //               Console.WriteLine("=============================================");
        //               Console.WriteLine($"Parsing input string: \"{input_string}\"");
        //               var tokens = lexer.parsing(input_string);
        //               var syntax_builder = new SyntaxBuilder(tokens, grammar);
        //               try {
        //                   Node ast_root = syntax_builder.build();
        //                   Console.WriteLine("\nSuccessfully built AST with Scheme A!");
        //                   ast_processor.process(ast_root);
        //               }
        //               catch (Exception e) {
        //                   Console.WriteLine($"\nError during syntax building (Scheme A): {e.Message}");
        //               }
        //               Console.WriteLine("=============================================\n");
        //           }
        //           Console.WriteLine("================================================\n");
        //       }
    }
}
