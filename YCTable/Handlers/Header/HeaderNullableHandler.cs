
using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Header {
    public class HeaderNullableHandler : HeaderHandlerBase {
        public override string node_type => "header_nullable";
        private ExcelTable context;

        public HeaderNullableHandler(ExcelTable context) {
            this.context = context;
        }

        public override void handle(Node node, AstProcessor ast_processor, int depth) {
            var td = build_node(node, ast_processor);
        }

        public override IHeader build_node(Node node, AstProcessor? ap = null) {
            if (node == null) return new HeaderBasic("unknown", null);
            var child = node.get_real_children().FirstOrDefault();
            if (child == null) return new HeaderBasic("unknown", node);
            var inner = child.build_child(ap);
            if (inner is HeaderNullable) return inner;
            // if this nullable node has explicit '?' marker, wrap; otherwise return inner
            if (node.has_nullable()) {
                return new HeaderNullable(inner, node);
            }
            return inner;
        }
    }
}
