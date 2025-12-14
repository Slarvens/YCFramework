using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCAnalyzerProject.Handlers {
    public class DictionaryHandler : INodeHandler {
        public string node_type => "dictionary";
        public void handle(Node node, AstProcessor ast_processor, int depth) { }
        public void log(Node node, AstProcessor ast_processor, int depth) {
            var indent_str = new string(' ', depth * 2);
            var children_list = node.children.Where(c => c.type == "element_type" || c.token != null).ToList();
            if (children_list.Count >= 3) {
                var key_str = children_list[0].token?.lexeme ?? children_list[0].type;
                var value_str = children_list[2].token?.lexeme ?? children_list[2].type;
                Console.WriteLine(indent_str + $"dictionary: {key_str} -> {value_str}");
            }
            else {
                Console.WriteLine(indent_str + "dictionary");
            }
        }
    }
}
