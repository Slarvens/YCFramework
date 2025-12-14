using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCAnalyzerProject.Handlers {
    public class BasicTypeHandler : INodeHandler {
        public string node_type => "basic_type";

        public void handle(Node node, AstProcessor ast_processor, int depth) { }

        public void log(Node node, AstProcessor ast_processor, int depth) {
            var indent_str = new string(' ', depth * 2);
            var tok_token = node.children.FirstOrDefault(c => c.token != null)?.token;
            var val_str = tok_token?.lexeme ?? node.type;
            Console.WriteLine(indent_str + $"basic_type: {val_str}");
        }
    }
}
