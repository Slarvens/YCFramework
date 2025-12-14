using YCAnalyzer;
using YCAnalyzer.Syntaxer;
using YCParseInvocation.Handlers;
using YCSyntaxer;

namespace YCParseInvocation {
    public class ParseInvocation {
        //入口规则必须顶格写，不能有缩进或空格
        public static string rule_config = @"method: @Identifier '(' method_params? ')';
method_params: (method | basic_type) (',' (method | basic_type))*;
basic_type: @Integer | @Long | @Float | @Double | @String | @Bool | @Identifier;";
        public static void parse(string content, Action<string, int> on_method_start, Action<int, string, string> on_param, Action on_method_end) {
            //1.解析规则
            var grammar_parser = new GrammarParser();
            var grammar_def = grammar_parser.parse(rule_config);
            //2.词法分析
            var lexer_obj = new Lexer();
            var tokens_list = lexer_obj.parsing(content);
            //3. 构建AST
            var syntax_builder = new SyntaxBuilder(tokens_list, grammar_def);
            var ast_root = syntax_builder.build(); //直接 build()
                                                   //4.处理AST
            var ast_processor = new AstProcessor();

            // always register both handlers and use ParamContext to route params
            var param_ctx = new ParamContext();
            param_ctx.global_on_param = on_param;
            param_ctx.global_on_method_end = on_method_end;
            var basic_handler = new BasicTypeHandlerForInvocation { param_context = param_ctx };
            var method_handler = new MethodHandlerForInvocation { param_context = param_ctx };
            method_handler.on_method_start = on_method_start;
            method_handler.on_method_end = on_method_end;
            ast_processor.register(basic_handler);
            ast_processor.register(method_handler);

            ast_processor.process(ast_root);
        }

        // Helper moved from Program: parse_test uses snake_case naming and prints indented callbacks
        public static void parse_test(string input) {
            Console.WriteLine();
            Console.WriteLine($"输入: {input}");

            int indent_level = 0;
            string indent() => new string(' ', indent_level * 2);

            ParseInvocation.parse(input, (method_name, paramCount) => {
                Console.WriteLine($"{indent()}方法开始: {method_name}, 参数数量: {paramCount}");
                indent_level++;
            }, (index, value, type) => {
                Console.WriteLine($"{indent()}参数[{index}]: 值={value}, 类型={type}");
            }, () => {
                indent_level = Math.Max(0, indent_level - 1);
                Console.WriteLine($"{indent()}方法结束");
            });
        }
    }
}
