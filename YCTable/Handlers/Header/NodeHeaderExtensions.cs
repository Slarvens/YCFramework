using System.Linq;
using YCSyntaxer;
using YCAnalyzer.Syntaxer;


namespace YCTable.Handlers.Header {
    public static class NodeHeaderExtensions {
        // snake_case extension to build a header for a child node
        public static IHeader build_child(this Node n, AstProcessor? ap) {
            if (n == null) return new HeaderBasic("unknown", null);
            // token -> basic
            if (n.token != null) {
                return new HeaderBasic(HeaderHandlerBase.normalize_type_name(n.token.lexeme), n);
            }

            // Try to get a registered handler from AstProcessor
            if (ap != null) {
                var handler = ap.get_handler(n.type);
                if (handler is HeaderHandlerBase hh) {
                    return hh.build_node(n, ap);
                }
            }

            // special-case nullable wrapper nodes: build inner and wrap
            if (n.type == "header_nullable") {
                var child = n.get_real_children().FirstOrDefault();
                if (child != null) {
                    var inner = child.build_child(ap);
                    if (inner is HeaderNullable) return inner;
                    return new HeaderNullable(inner, n);
                }
                return new HeaderBasic("unknown", n);
            }

            // Fallback: try first meaningful child
            var fc = n.get_real_children().FirstOrDefault();
            if (fc != null) return fc.build_child(ap);
            return new HeaderBasic("unknown", n);
        }
    }
}
