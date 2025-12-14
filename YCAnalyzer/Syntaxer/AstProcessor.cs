using YCSyntaxer;

namespace YCAnalyzer.Syntaxer {
    public interface INodeHandler {
        // the node type this handler is responsible for
        string node_type { get; }
        // side-effect processing only
        void handle(Node node, AstProcessor ast_processor, int depth);
        // logging, handler outputs its own log using indentation
        void log(Node node, AstProcessor ast_processor, int depth);
    }

    public class AstProcessor {
        private readonly Dictionary<string, INodeHandler> _handlers = new();
        private readonly INodeHandler _default_handler;

        public AstProcessor() {
            // no built-in registrations here ¡ª callers should register handlers from their project
            _default_handler = new DefaultHandler();
        }

        public void register(INodeHandler handler) {
            if (handler == null) {
                throw new ArgumentNullException(nameof(handler));
            }

            if (string.IsNullOrEmpty(handler.node_type)) {
                throw new ArgumentException("Handler must provide a non-empty node_type.", nameof(handler));
            }

            _handlers[handler.node_type] = handler;
        }

        // Retrieve a registered handler instance by node type (or null if not found)
        public INodeHandler? get_handler(string nodeType) {
            _handlers.TryGetValue(nodeType, out var h);
            return h;
        }

        public void process(Node root) {
            if (root == null) {
                return;
            }

            process_node(root, 0);
        }

        private void process_node(Node node, int depth) {
            // use TryGetValue once and cache handler locally
            if (node == null) {
                return;
            }

            _handlers.TryGetValue(node.type, out var handler_obj);
            var handler = handler_obj ?? _default_handler;

            // side-effect processing
            handler.handle(node, this, depth);

            // handler performs its own logging (must handle indentation)
            handler.log(node, this, depth);

            // recurse for children by default using indexed loop to avoid enumerator allocations
            var children_list = node.children;
            for (int index = 0; index < children_list.Count; index++) {
                process_node(children_list[index], depth + 1);
            }
        }

        // Utility: extract identifier lexeme from a node (method name or basic_type)
        public string extract_identifier(Node node) {
            if (node == null) {
                return string.Empty;
            }

            var children_list = node.children;
            for (int index = 0; index < children_list.Count; index++) {
                var child = children_list[index];
                if (child.token != null && child.token.type == TokenType.Identifier) {
                    return child.token.lexeme;
                }
            }

            // fallback: if node itself has token
            if (node.token != null && node.token.type == TokenType.Identifier) {
                return node.token.lexeme;
            }

            return "<unknown>";
        }

        // Simple default handler so framework has a fallback implementation
        private class DefaultHandler : INodeHandler {
            public string node_type => "<default>";
            public void handle(Node node, AstProcessor ast_processor, int depth) { /* no-op */ }
            public void log(Node node, AstProcessor ast_processor, int depth) {
                // Suppress default logging to avoid duplicate output when using invocation callbacks
            }
        }
    }
}
