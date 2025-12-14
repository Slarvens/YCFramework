using System.Linq;

namespace YCTable {
    public class HeaderBasic : IHeader {
        public string name { get; }
        public string? field_name => name;
        private readonly string _header_type;
        public string header_type => _header_type;
        private readonly global::YCSyntaxer.Node? _node;
        public global::YCSyntaxer.Node? node => _node;
        public HeaderBasic(string name, global::YCSyntaxer.Node? node) {
            this.name = name;
            this._node = node;
            this._header_type = name.ToLowerInvariant();
        }
        public bool match(Table.ValueDescriptor value) {
            if (value is Table.ValueBasic vb) {
                var hv = name.ToLowerInvariant();
                var vv = vb.name.ToLowerInvariant();
                if (hv == vv) return true;
                var numeric = new HashSet<string> { "integer", "long", "float", "double" };
                if (numeric.Contains(hv) && numeric.Contains(vv)) return true;
                if (hv == "string" && (vv == "string" || vv == "identifier")) return true;
            }
            return false;
        }
    }
}
