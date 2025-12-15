using System.Linq;
using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Header {
    public class HeaderArrayHandler : HeaderHandlerBase {
        public override string node_type => "header_array";
        private ExcelTable context;

        public HeaderArrayHandler(ExcelTable context) {
            this.context = context;
        }
        public override void handle(Node node, AstProcessor ast_processor, int depth) {
            var td = build_node(node, ast_processor);
        }
        public override void log(Node node, AstProcessor ast_processor, int depth) {
            // suppressed
        }

        // Build logic for array nodes
        public override IHeader build_node(Node node, AstProcessor? ap = null) {
            if (node == null) return new HeaderBasic("unknown", null);
            // find element child (skip '?', '[' ,']')
            var child = node.get_real_children().FirstOrDefault(c => !(c.token != null && (c.token.lexeme == "[" || c.token.lexeme == "]")));
            IHeader elem = child != null ? child.build_child(ap) : new HeaderBasic("unknown", null);
            IHeader res = new HeaderArray(elem, node);
            // nullable wrapper
            if (node.has_nullable() && !(res is HeaderNullable)) {
                res = new HeaderNullable(res, node);
            }
            return res;
        }
    }
}
