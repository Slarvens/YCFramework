
namespace YCAnalyzerProject {
    internal class Program {
        static void Main(string[] args) {
            //// Only test Table.parse_headers here. Other tests disabled.

            //Console.WriteLine("--- Table parse_headers tests ---");

            //var test_headers = new[] {
            //    "int",
            //    "(string,string)",
            //    "dict<string,int>",
            //    "int[]",
            //    "(int,string)",
            //    "(int?,string)",
            //    "dict<int,string>",
            //    "dict<int?,string>",
            //    "int[]",
            //    "(int,string)?"
            //};

            //try {
            //    var results = Table.parse_headers(test_headers, throwOnError: true);

            //    for (int i =0; i < test_headers.Length; i++) {
            //        var h = test_headers[i];
            //        var r = results.Count > i ? results[i] : null;
            //        if (r == null) {
            //            Console.WriteLine($"Header: {h} => FAILED");
            //        }
            //        else {
            //            Console.WriteLine($"Header: {h} => {r.header_type}");
            //        }
            //    }
            //}
            //catch (Exception ex) {
            //    Console.WriteLine($"Parsing failed with exception: {ex.Message}");
            //    Console.WriteLine(ex.StackTrace);
            //}

            // Other tests (value parsing, validation) are disabled while focusing on parse_headers.
            // Console.WriteLine("--- Table value parsing tests ---");
            // Table.parse_value_test("123");
            // Table.parse_value_test("true");
            // Table.parse_value_test("1~2");
            // Table.parse_value_test("1;2;3");
            // Table.parse_value_test("\"k\"=1;\"k2\"=2");

            // Console.WriteLine();
            // Console.WriteLine("--- Header vs Values validation tests ---");

            // Table.validate_values_for_header("Integer", new[] { "123", "456", "abc" });
            // Table.validate_values_for_header("(Integer,String)", new[] { "1~\"a\"", "2~\"b\"", "3~4" });
            // Table.validate_values_for_header("dict<Integer,String>", new[] { "\"k\"=\"v\"", "\"k\"=123" });
            // Table.validate_values_for_header("Integer[]", new[] { "1;2;3", "" });

            // Console.WriteLine("--- New-rule tests: header/value validation ---");

            // void run_case(string header, string[] values) {
            //     Console.WriteLine();
            //     Console.WriteLine($"Header: {header}");
            //     var desc = Table.parse_header_descriptor(header);
            //     if (desc == null) {
            //         Console.WriteLine("Header parse: FAILED");
            //     }
            //     else {
            //         Console.WriteLine("Header parse: OK");
            //     }

            //     Table.validate_values_for_header(header, values);
            // }

            // //1) tuple with non-nullable items (valid)
            // run_case("(Integer,String)", new[] { "1~\"a\"", "2~\"b\"" });

            // //2) tuple with nullable item (invalid in header grammar)
            // run_case("(Integer?,String)", new[] { "1~\"a\"" });

            // //3) dict with non-nullable key/value (valid)
            // run_case("dict<Integer,String>", new[] { "\"k\"=\"v\"", "\"k\"=123" });

            // //4) dict with nullable key (invalid in header grammar)
            // run_case("dict<Integer?,String>", new[] { "\"k\"=\"v\"" });

            // //5) array with non-nullable elements (valid)
            // run_case("Integer[]", new[] { "1;2;3" });

            // //6) top-level nullable type (allowed) - empty value should be accepted
            // run_case("(Integer,String)?", new[] { "", "1~\"x\"" });
        }
    }
}