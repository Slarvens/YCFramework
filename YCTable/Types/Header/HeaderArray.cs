
namespace YCTable {
 public class HeaderArray : IHeader {
 public IHeader element { get; }
 public string? field_name => null;
 private readonly string _header_type;
 public string header_type => _header_type;
 private readonly global::YCSyntaxer.Node? _node;
 public global::YCSyntaxer.Node? node => _node;
 public HeaderArray(IHeader element, global::YCSyntaxer.Node? node)
 {
 this.element = element;
 this._node = node;
 this._header_type = element.header_type + "[]";
 }
 }
}
