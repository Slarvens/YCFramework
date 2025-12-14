using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Value {
    public class BasicTypeHandler : INodeHandler {
        public string node_type => "basic_type";

        public void handle(Node node, AstProcessor ast_processor, int depth) { }
        public void log(Node node, AstProcessor ast_processor, int depth) {
            var indent = new string(' ', depth * 2);
            var tok = node.children.FirstOrDefault(c => c.token != null)?.token;
            var val = tok != null ? tok.lexeme : node.type;
            Console.WriteLine(indent + $"basic_type: {val}");
        }
    }
}
