using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCAnalyzerProject.Handlers {
    public class ArrayLiteralHandler : INodeHandler {
        public string node_type => "array_literal";
        public void handle(Node node, AstProcessor ast_processor, int depth) { }
        public void log(Node node, AstProcessor ast_processor, int depth) {
            var indent_str = new string(' ', depth * 2);
            var items_node = node.children.FirstOrDefault(c => c.type == "array_items");
            if (items_node == null) { Console.WriteLine(indent_str + "array_literal"); return; }
            Console.WriteLine(indent_str + "array_literal:");
            int index = 0;
            foreach (var child in items_node.children) {
                var val_str = child.token != null ? child.token.lexeme : child.type;
                Console.WriteLine(indent_str + " " + $"item[{index}]: {val_str}");
                index++;
            }
        }
    }
}
