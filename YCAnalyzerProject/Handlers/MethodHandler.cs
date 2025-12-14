using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCAnalyzerProject.Handlers {
    public class MethodHandler : INodeHandler {
        public string node_type => "method";
        public void handle(Node node, AstProcessor ast_processor, int depth) { }
        public void log(Node node, AstProcessor ast_processor, int depth) {
            var indent_str = new string(' ', depth * 2);
            var name_str = ast_processor.extract_identifier(node);
            Console.WriteLine(indent_str + $"method: {name_str}");
            var params_node = node.children.FirstOrDefault(c => c.type == "method_params");
            if (params_node != null) {
                int index = 0;
                foreach (var child in params_node.children) {
                    var rendered = child.token != null ? child.token.lexeme : child.type;
                    Console.WriteLine(indent_str + " " + $"param[{index}]: {rendered}");
                    index++;
                }
            }
        }
    }
}
