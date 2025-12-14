using System;
using YCAnalyzer.Syntaxer;
using YCSyntaxer;

namespace YCTable.Handlers.Header {
 public abstract class HeaderHandlerBase : INodeHandler {
 // Each concrete handler must supply the node_type it handles
 public abstract string node_type { get; }

 // Concrete handlers must implement processing behavior
 public abstract void handle(Node node, AstProcessor ast_processor, int depth);

 // Default logging is suppressed; concrete handlers can override
 public virtual void log(Node node, AstProcessor ast_processor, int depth) { }

 // Helper to normalize type names (snake_case name as requested)
 public static string normalize_type_name(string raw) {
 if (string.IsNullOrEmpty(raw)) return "unknown";
 var key = raw.Trim();
 if (key.Length >=2 && key[0] == '\'' && key[key.Length -1] == '\'') {
 key = key.Substring(1, key.Length -2);
 }

 switch (key.ToLowerInvariant()) {
 case "int":
 case "integer": return "int";
 case "long": return "long";
 case "float": return "float";
 case "double": return "double";
 case "string":
 case "str": return "string";
 case "bool":
 case "boolean": return "bool";
 case "json": return "json";
 default: return key.ToLowerInvariant();
 }
 }

 // Build a header node (instance method, snake_case)
 public abstract IHeader build_node(Node node, AstProcessor? ap = null);

 // Per-handler validation hook. Default: no validation. Handlers override to enforce rules and throw when invalid.
 protected virtual void validate_node(Node node, AstProcessor? ap = null) { }
 }
}
