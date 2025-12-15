namespace YCTable {
 public class HeaderNullable : IHeader {
 public IHeader inner { get; }
 public string? field_name => null;
 private readonly string _header_type;
 public string header_type => _header_type;
 private readonly global::YCSyntaxer.Node? _node;
 public global::YCSyntaxer.Node? node => _node;
 public HeaderNullable(IHeader inner, global::YCSyntaxer.Node? node) {
 this.inner = inner;
 this._node = node;
 this._header_type = inner.header_type + "?";
 }
 }
}
