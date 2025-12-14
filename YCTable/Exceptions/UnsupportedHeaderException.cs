using System;
using YCSyntaxer;

namespace YCTable.Exceptions {
 public class UnsupportedHeaderException : Exception {
 public Node? Node { get; }
 public string? HeaderText { get; }

 public UnsupportedHeaderException(string message) : base(message) { }
 public UnsupportedHeaderException(string message, Node? node) : base(message) { Node = node; }
 public UnsupportedHeaderException(string message, string? headerText, Node? node) : base(message) { HeaderText = headerText; Node = node; }
 public UnsupportedHeaderException(string message, Exception inner, Node? node) : base(message, inner) { Node = node; }

 public override string ToString() {
 var baseStr = base.ToString();
 try {
 var info = Node != null ? $" NodeType={Node.type}" : string.Empty;
 var header = HeaderText != null ? $" Header='{HeaderText}'" : string.Empty;
 return baseStr + info + header;
 }
 catch {
 return baseStr;
 }
 }
 }
}
