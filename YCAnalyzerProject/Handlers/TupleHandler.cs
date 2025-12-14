using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCAnalyzerProject.Handlers {
    public class TupleHandler : INodeHandler {
        public string node_type => "tuple";
        public void handle(Node node, AstProcessor ast_processor, int depth) { }
        public void log(Node node, AstProcessor ast_processor, int depth) {
            var indent_str = new string(' ', depth * 2);
            Console.WriteLine(indent_str + "(");
            int index = 0;
            foreach (var child in node.children) {
                var val_str = child.token != null ? child.token.lexeme : child.type;
                Console.WriteLine(indent_str + " " + $"item[{index}]: {val_str}");
                index++;
            }
            Console.WriteLine(indent_str + ")");
        }
    }
}
