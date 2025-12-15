using System.Linq;
using YCAnalyzer.Syntaxer;
using YCSyntaxer;
using YCTable.Types;

namespace YCTable.Handlers.Header {
    public class HeaderTupleHandler : HeaderHandlerBase {
        public override string node_type => "header_tuple";
        private ExcelTable context;

        public HeaderTupleHandler(ExcelTable context) {
            this.context = context;
        }
        public override void handle(Node node, AstProcessor ast_processor, int depth) {
            var td = build_node(node, ast_processor); 
        }
        public override void log(Node node, AstProcessor ast_processor, int depth) {
            // suppressed
        }

        public override IHeader build_node(Node node, AstProcessor ap) {
            if (node == null) return new HeaderBasic("unknown", null);
            var elems = node.get_real_children()
            .Where(c => !(c.token != null && (c.token.lexeme == "(" || c.token.lexeme == ")" || c.token.lexeme == ",")))
            .Select(c => c.build_child(ap)).ToList();
            IHeader res = new HeaderTuple(elems, node);
            if (node.has_nullable() && !(res is HeaderNullable)) {
                res = new HeaderNullable(res, node);
            }
            return res;
        }
    }
}
