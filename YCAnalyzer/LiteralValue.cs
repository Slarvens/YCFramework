using System.Globalization;
using System.Runtime.InteropServices;

namespace YCAnalyzer {
    public enum LiteralType {
        None,
        Integer,
        Long,
        Double,
        Float,
        Bool,
        String
    }

    [StructLayout(LayoutKind.Explicit)]
    public readonly struct LiteralValue {
        [FieldOffset(0)]
        private readonly int _int_value;

        [FieldOffset(0)]
        private readonly long _long_value;

        [FieldOffset(0)]
        private readonly double _double_value;

        [FieldOffset(0)]
        private readonly float _float_value;

        [FieldOffset(0)]
        private readonly bool _bool_value;

        [FieldOffset(8)]
        private readonly string? _string_value;

        [FieldOffset(16)]
        public readonly LiteralType type;

        public LiteralValue(int value) {
            this = default;
            type = LiteralType.Integer;
            _int_value = value;
        }

        public LiteralValue(long value) {
            this = default;
            type = LiteralType.Long;
            _long_value = value;
        }

        public LiteralValue(double value) {
            this = default;
            type = LiteralType.Double;
            _double_value = value;
        }

        public LiteralValue(float value) {
            this = default;
            type = LiteralType.Float;
            _float_value = value;
        }

        public LiteralValue(bool value) {
            this = default;
            type = LiteralType.Bool;
            _bool_value = value;
        }

        public LiteralValue(string? value) {
            this = default;
            type = LiteralType.String;
            _string_value = value;
        }

        public int as_int() => type switch {
            LiteralType.Integer => _int_value,
            LiteralType.Long => (int)_long_value,
            LiteralType.Double => (int)_double_value,
            LiteralType.Float => (int)_float_value,
            LiteralType.String when int.TryParse(_string_value, out var result) => result,
            _ => 0
        };

        public long as_long() => type switch {
            LiteralType.Long => _long_value,
            LiteralType.Integer => _int_value,
            LiteralType.Double => (long)_double_value,
            LiteralType.Float => (long)_float_value,
            LiteralType.String when long.TryParse(_string_value, out var result) => result,
            _ => 0
        };

        public double as_double() => type switch {
            LiteralType.Double => _double_value,
            LiteralType.Long => _long_value,
            LiteralType.Float => _float_value,
            LiteralType.String when double.TryParse(_string_value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) => result,
            _ => 0.0
        };

        public float as_float() => type switch {
            LiteralType.Float => _float_value,
            LiteralType.Long => _long_value,
            LiteralType.Double => (float)_double_value,
            LiteralType.String when float.TryParse(_string_value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) => result,
            _ => 0.0f
        };

        public bool as_bool() => type switch {
            LiteralType.Bool => _bool_value,
            LiteralType.Long => _long_value != 0,
            LiteralType.Double => _double_value != 0,
            LiteralType.Float => _float_value != 0,
            LiteralType.String => !string.IsNullOrEmpty(_string_value) && !string.Equals(_string_value, "false", StringComparison.OrdinalIgnoreCase),
            _ => false
        };

        public override string ToString() => type switch {
            LiteralType.String => _string_value ?? string.Empty,
            LiteralType.Integer => _int_value.ToString(CultureInfo.InvariantCulture),
            LiteralType.Long => _long_value.ToString(CultureInfo.InvariantCulture),
            LiteralType.Double => _double_value.ToString(CultureInfo.InvariantCulture),
            LiteralType.Float => _float_value.ToString(CultureInfo.InvariantCulture),
            LiteralType.Bool => _bool_value.ToString(),
            _ => string.Empty
        };
    }
}
