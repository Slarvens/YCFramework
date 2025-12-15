using System.Linq;
using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Header {
 public class HeaderJsonHandler : HeaderHandlerBase {
 public override string node_type => "header_json";
 private ExcelTable context;

 public HeaderJsonHandler(ExcelTable context) {
 this.context = context;
 }

 public override void handle(Node node, AstProcessor ast_processor, int depth) {
 var td = build_node(node, ast_processor);
 }

 public override IHeader build_node(Node node, AstProcessor? ap = null) {
 // json is treated as a basic type named 'json'
 return new HeaderBasic("json", node);
 }
 }
}
