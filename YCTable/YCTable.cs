

namespace YCTable {
    public class YCTable {

        public static void load_from_path(string file_path) {
            if (string.IsNullOrWhiteSpace(file_path)) throw new ArgumentException("file_path is null or empty", nameof(file_path));
            if (!File.Exists(file_path)) throw new FileNotFoundException("Excel file not found", file_path);

            // Create excel_table object and let it load the file (instance method).
            var excel_table = new ExcelTable();
            excel_table.load_from_path(file_path);

            // Create a HeaderContentReader, pass the loaded excel_table and process headers
            var header_reader = new HeaderContentReader();
            header_reader.process_headers(excel_table);

            // TODO: use processed headers for further processing (validation, mapping, etc.)
        }
    }
}
