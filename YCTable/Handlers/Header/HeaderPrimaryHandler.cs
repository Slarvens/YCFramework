using System.Linq;
using System.Collections.Generic;
using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Header {
    public class HeaderPrimaryHandler : HeaderHandlerBase {
        public override string node_type => "header_primary";
        private readonly List<IHeader?> _collector;

        public HeaderPrimaryHandler() : this(new List<IHeader?>()) { }
        public HeaderPrimaryHandler(List<IHeader?> collector) {
            _collector = collector;
        }

        public override void handle(Node node, AstProcessor ast_processor, int depth) {
            var td = build_node(node, ast_processor);
            _collector.Add(td);
        }



        public override IHeader build_node(Node node, AstProcessor? ap = null) {
            if (node == null) return new HeaderBasic("unknown", null);
            // prefer token child
            var tok = node.get_real_children().FirstOrDefault(c => c.token != null)?.token ?? node.token;
            if (tok != null) {
                var name = normalize_type_name(tok.lexeme);
                IHeader res = new HeaderBasic(name, node);
                // wrap nullable if '?' present on this node
                if (node.has_nullable()) {
                    res = new HeaderNullable(res, node);
                }
                return res;
            }

            // fallback: build first non-? child
            var first = node.get_real_children().FirstOrDefault(c => !(c.token != null && c.token.lexeme == "?"));
            if (first != null) return first.build_child(ap);
            return new HeaderBasic("unknown", node);
        }
    }
}
