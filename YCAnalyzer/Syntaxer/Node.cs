using YCAnalyzer;
using System.Linq;
using System.Collections.Generic;

namespace YCSyntaxer {
    /// <summary>
    /// 一个通用的、可以表示任何语法结构的 AST 节点。
    /// </summary>
    public class Node {
        public string type { get; }
        public Token? token { get; }
        public List<Node> children { get; } = new List<Node>();
        public LiteralValue? value { get; }


        public Node(string type, Token? token = null, LiteralValue? value = null) {
            this.type = type;
            this.token = token;
            this.value = value;
        }

        public void add_child(Node child) => children.Add(child);
        public void add_children(IEnumerable<Node> children) => this.children.AddRange(children);

        // Return children except optional '?' marker nodes
        public IEnumerable<Node> get_real_children() => children.Where(c => !(c.token != null && c.token.lexeme == "?"));

        // Whether this node carries a nullable marker ('?')
        public bool has_nullable() => children.Any(c => c.token != null && c.token.lexeme == "?");

        // Simple node info helper
        public string node_info() {
            try {
                return type ?? "<unknown>";
            }
            catch {
                return "<unknown>";
            }
        }
    }
}
