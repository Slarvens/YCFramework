using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Value {
    public class TupleHandler : INodeHandler {
        public string node_type => "tuple";
        public void handle(Node node, AstProcessor ast_processor, int depth) { }
        public void log(Node node, AstProcessor ast_processor, int depth) {
            var indent = new string(' ', depth * 2);
            Console.WriteLine(indent + "tuple:");
            int i = 0;
            foreach (var child in node.children) {
                if (child.token != null) Console.WriteLine(indent + $" item[{i}]: {child.token.lexeme}");
                else Console.WriteLine(indent + $" item[{i}]: {child.type}");
                i++;
            }
        }
    }
}
