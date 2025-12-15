using System.Linq;
using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Header {
 public class HeaderElementTypeHandler : HeaderHandlerBase {
 public override string node_type => "element_type";
 private ExcelTable context;

 public HeaderElementTypeHandler(ExcelTable context) {
 this.context = context;
 }

 public override void handle(Node node, AstProcessor ast_processor, int depth) {
 var td = build_node(node, ast_processor);
 }

 public override IHeader build_node(Node node, AstProcessor ap) {
 // reuse header_primary logic: if token present, create HeaderBasic, else fallback
 if (node == null) return new HeaderBasic("unknown", null);
 var tok = node.get_real_children().FirstOrDefault(c => c.token != null)?.token ?? node.token;
 if (tok != null) {
 var name = normalize_type_name(tok.lexeme);
 IHeader res = new HeaderBasic(name, node);
 if (node.has_nullable()) res = new HeaderNullable(res, node);
 return res;
 }
 var first = node.get_real_children().FirstOrDefault(c => !(c.token != null && c.token.lexeme == "?"));
 if (first != null) return first.build_child(ap);
 return new HeaderBasic("unknown", node);
 }
 }
}
