using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCParseInvocation.Handlers {
    public class MethodHandlerForInvocation : INodeHandler {
        public string node_type => "method";

        // Callbacks
        public Action<string, int>? on_method_start { get; set; }
        public Action? on_method_end { get; set; }

        // shared param context
        public ParamContext? param_context { get; set; }

        public void handle(Node node, AstProcessor ast_processor, int depth) {
            // Avoid double-processing of nodes that were already manually handled
            if (param_context != null && param_context.is_processed(node)) {
                return;
            }

            // If this is the method node itself, invoke method start, push a param frame,
            // process children manually, then pop the frame and invoke method-end callbacks.
            if (node.type == "method") {
                var pc = param_context;
                var name = ast_processor.extract_identifier(node);

                // Count parameters by inspecting method_params child
                var paramsNode = node.children.FirstOrDefault(c => c.type == "method_params");
                int paramCount = 0;
                if (paramsNode != null) {
                    paramCount = paramsNode.children.Count(c => c.type == "method" || c.type == "basic_type");
                }

                // invoke method start when node encountered ¡ª include parameter count as separate arg
                on_method_start?.Invoke(name, paramCount);

                // Push a new frame for this method's parameters; do NOT treat nested methods as parameters
                if (pc != null) {
                    pc.push(pc.global_on_param, pc.global_on_method_end);
                    // mark the frame as started so params won't be reported before this method start
                    var frame = pc.peek();
                    if (frame != null) {
                        frame.method_started = true;
                    }
                }

                // Manually process children to ensure we can pop the frame after children are handled
                var children = node.children;
                for (int i = 0; i < children.Count; i++) {
                    var child = children[i];
                    // skip if already processed (defensive)
                    if (pc != null && pc.is_processed(child)) {
                        continue;
                    }

                    ast_processor.process(child);
                }

                // After manual processing, mark children as processed so outer AstProcessor won't handle them again later
                if (pc != null) {
                    pc.mark_processed((YCSyntaxer.Node)node);
                }

                // Pop the frame and invoke its end callbacks
                var completedFrame = pc?.pop();
                if (completedFrame != null) {
                    // invoke frame's on_method_end (could be the global handler)
                    completedFrame.on_method_end?.Invoke();
                    // if the frame's on_method_end is not the global handler, also call global on_method_end
                    if (completedFrame.on_method_end != pc?.global_on_method_end) {
                        on_method_end?.Invoke();
                    }
                }
            }
        }

        // No side-effect logic in log() ¡ª all processing happens in handle()
        public void log(Node node, AstProcessor ast_processor, int depth) { }
    }
}
