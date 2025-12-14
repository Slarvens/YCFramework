using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Value {
    public class DictHandler : INodeHandler {
        public string node_type => "dict";
        public void handle(Node node, AstProcessor ast_processor, int depth) { }
        public void log(Node node, AstProcessor ast_processor, int depth) {
            var indent = new string(' ', depth * 2);
            var pairs = node.children.Where(c => c.type == "pair").ToList();
            if (pairs.Count == 0) {
                Console.WriteLine(indent + "dict: {}");
                return;
            }

            Console.WriteLine(indent + "dict:");
            int i = 0;
            foreach (var p in pairs) {
                var k = p.children.FirstOrDefault();
                var v = p.children.Skip(2).FirstOrDefault();

                string ks;
                if (k != null) {
                    if (k.token != null) {
                        ks = k.token.lexeme;
                    } else {
                        ks = k.type;
                    }
                } else {
                    ks = "<unknown>";
                }

                string vs;
                if (v != null) {
                    if (v.token != null) {
                        vs = v.token.lexeme;
                    } else {
                        vs = v.type;
                    }
                } else {
                    vs = "<unknown>";
                }

                Console.WriteLine(indent + $" pair[{i}]: {ks} = {vs}");
                i++;
            }
        }
    }
}
