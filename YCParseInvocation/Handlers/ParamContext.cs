namespace YCParseInvocation.Handlers {
    public class ParamContext {
        public class Frame {
            public Action<int, string, string>? on_param;
            public Action? on_method_end;
            public int current_index;
            // indicates the method start callback has been invoked for this frame
            public bool method_started;
        }

        private readonly Stack<Frame> _stack = new();

        // Global defaults that MethodHandler will use when pushing frames
        public Action<int, string, string>? global_on_param { get; set; }
        public Action? global_on_method_end { get; set; }

        // track nodes that have been processed manually to avoid duplicate callbacks
        private readonly HashSet<YCSyntaxer.Node> _processed = new();

        public void push(Action<int, string, string>? on_param, Action? on_method_end) {
            _stack.Push(new Frame { on_param = on_param, on_method_end = on_method_end, current_index = 0, method_started = false });
        }

        public Frame? pop() {
            if (_stack.Count == 0) return null;
            return _stack.Pop();
        }

        public Frame? peek() {
            return _stack.Count > 0 ? _stack.Peek() : null;
        }

        public bool is_active => _stack.Count > 0;

        // Mark a node and its descendants as processed to avoid double-processing
        public void mark_processed(YCSyntaxer.Node node) {
            if (node == null) {
                return;
            }

            // Iterative traversal using a stack to avoid recursion
            var stack = new Stack<YCSyntaxer.Node>();
            stack.Push(node);

            while (stack.Count > 0) {
                var cur = stack.Pop();
                if (cur == null) {
                    continue;
                }
                if (_processed.Contains(cur)) {
                    continue;
                }
                _processed.Add(cur);
                var children = cur.children;
                for (int i = 0; i < children.Count; i++) {
                    stack.Push(children[i]);
                }
            }
        }

        public bool is_processed(YCSyntaxer.Node node) {
            return node != null && _processed.Contains(node);
        }
    }
}
