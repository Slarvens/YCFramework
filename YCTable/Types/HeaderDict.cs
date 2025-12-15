namespace YCTable {
    public class HeaderDict : IHeader {
        public IHeader key_type { get; }
        public IHeader value_type { get; }
        public string? field_name => null;
        private readonly string _header_type;
        public string header_type => _header_type;
        private readonly global::YCSyntaxer.Node? _node;
        public global::YCSyntaxer.Node? node => _node;
        public HeaderDict(IHeader key_type, IHeader value_type, global::YCSyntaxer.Node? node) {
            this.key_type = key_type;
            this.value_type = value_type;
            this._node = node;
            this._header_type = $"dict<{key_type.header_type},{value_type.header_type}>";
        }
    }
}
