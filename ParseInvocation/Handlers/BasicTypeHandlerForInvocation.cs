using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCParseInvocation.Handlers {
    public class BasicTypeHandlerForInvocation : INodeHandler {
        public string node_type => "basic_type";

        public ParamContext? param_context { get; set; }

        public void handle(Node node, AstProcessor ast_processor, int depth) {
            // if this node was already processed manually, skip
            if (param_context != null && param_context.is_processed((YCSyntaxer.Node)node)) {
                return;
            }

            var tok = node.children.Find(c => c.token != null)?.token;
            var val = tok?.lexeme ?? node.type;
            var t = tok != null ? tok.type.ToString() : "Unknown";
            var frame = param_context?.peek();
            if (frame != null && frame.method_started && frame.on_param != null) {
                frame.on_param(frame.current_index, val, t);
                frame.current_index++;
            }
            else {
                // no-op when not within a method parameter context or method not started
            }
        }

        // No logging here ¡ª all processing happens in handle()
        public void log(Node node, AstProcessor ast_processor, int depth) { }
    }
}
