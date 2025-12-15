using YCSyntaxer;

namespace YCTable {
 public interface IHeader {
 string? field_name { get; }
 string header_type { get; }
 Node? node { get; }
 }
}
