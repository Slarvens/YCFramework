using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Value {
    public class ArrayHandler : INodeHandler {
        public string node_type => "array";
        public void handle(Node node, AstProcessor ast_processor, int depth) { }
        public void log(Node node, AstProcessor ast_processor, int depth) {
            var indent = new string(' ', depth * 2);
            var elems = node.children.Where(c => c.token != null || c.type != "[" && c.type != "]").ToList();
            if (elems.Count == 0) { Console.WriteLine(indent + "array: []"); return; }
            Console.WriteLine(indent + "array:");
            int i = 0;
            foreach (var e in elems) {
                if (e.token != null) Console.WriteLine(indent + $" item[{i}]: {e.token.lexeme}");
                else Console.WriteLine(indent + $" item[{i}]: {e.type}");
                i++;
            }
        }
    }
}
