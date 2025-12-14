using System.Linq;
using System.Collections.Generic;
using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Header {
    public class HeaderNullableHandler : HeaderHandlerBase {
        public override string node_type => "header_nullable";
        private readonly List<IHeader?> _collector;

        public HeaderNullableHandler() : this(new List<IHeader?>()) { }
        public HeaderNullableHandler(List<IHeader?> collector) { _collector = collector; }

        public override void handle(Node node, AstProcessor ast_processor, int depth) {
            var td = build_node(node, ast_processor);
            _collector.Add(td);
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
