
using YCAnalyzer.Syntaxer;
using YCSyntaxer;
using YCTable.Exceptions;

namespace YCTable.Handlers.Header {
    public class HeaderDictHandler : HeaderHandlerBase {
        public override string node_type => "header_dict";
        private ExcelTable context;

        public HeaderDictHandler(ExcelTable context) {
            this.context = context;
        }
        public override void handle(Node node, AstProcessor ast_processor, int depth) {
            var td = build_node(node, ast_processor);
        }
        public override void log(Node node, AstProcessor ast_processor, int depth) {
            // suppressed
        }

        protected override void validate_node(Node node, AstProcessor? ap = null) {
            // check that dictionary key type is not nullable
            var firstChild = node.get_real_children().FirstOrDefault();
            if (firstChild != null) {
                var keyHeader = firstChild.build_child(ap ?? this.ast_processor);
                if (keyHeader is HeaderNullable) {
                    string headerText = null;
                    try { headerText = string.Join("", node.children.Where(ch => ch.token != null).Select(ch => ch.token.lexeme)); } catch { }
                    throw new UnsupportedHeaderException($"Dictionary key type cannot be nullable: {keyHeader.header_type}", headerText, node);
                }
            }
        }

        public override IHeader build_node(Node node, AstProcessor? ap = null) {
            if (node == null) return new HeaderBasic("unknown", null);
            // validation
            validate_node(node, ap);

            var parts = node.get_real_children().Where(c => !(c.token != null && (c.token.lexeme == "dict" || c.token.lexeme == "<" || c.token.lexeme == ">" || c.token.lexeme == ","))).ToList();
            IHeader k = parts.Count >0 ? parts[0].build_child(ap ?? this.ast_processor) : new HeaderBasic("unknown", null);
            IHeader v = parts.Count >1 ? parts[1].build_child(ap ?? this.ast_processor) : new HeaderBasic("unknown", null);
            IHeader res = new HeaderDict(k, v, node);
            if (node.has_nullable() && !(res is HeaderNullable)) {
                res = new HeaderNullable(res, node);
            }
            return res;
        }
    }
}
