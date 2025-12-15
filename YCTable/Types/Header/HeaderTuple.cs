using System.Linq;

namespace YCTable {
 public class HeaderTuple : IHeader {
 public System.Collections.Generic.List<IHeader> elements { get; }
 public string? field_name => null;
 private readonly string _header_type;
 public string header_type => _header_type;
 private readonly global::YCSyntaxer.Node? _node;
 public global::YCSyntaxer.Node? node => _node;
 public HeaderTuple(System.Collections.Generic.List<IHeader> elements, global::YCSyntaxer.Node? node) {
 this.elements = elements;
 this._node = node;
 this._header_type = $"({string.Join(",", elements.Select(e => e.header_type))})";
 }
 }
}
