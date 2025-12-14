using System;
using System.Collections.Generic;
using System.IO;

namespace YCTable {
    // Vector2 similar to Unity's Vector2 but using integers for grid coordinates
    public readonly record struct Vector2(int x, int y) {
        // Provide a convenience deconstruct method
        public void Deconstruct(out int x, out int y) { x = this.x; y = this.y; }
        public override string ToString() => $"({x}, {y})";
    }

    // ExcelTable class: class name uses PascalCase; all other identifiers use snake_case per request
    public class ExcelTable {
        public Vector2 size { get; set; }

        public string[] content { get; set; }

        public Vector2 start_index { get; set; }

        public List<((int x_start, int x_length), (int y_start, int y_length))> merged_cells { get; set; }

        // Number of rows that have been read/processed from the table (set by readers)
        public int read_row_count { get; set; }

        public void load_from_path(string file_path) {
            if (string.IsNullOrWhiteSpace(file_path)) throw new ArgumentException("file_path is null or empty", nameof(file_path));
            if (!File.Exists(file_path)) throw new FileNotFoundException("Excel file not found", file_path);

            // Pretend we read a3x4 table from the file. Fill content with sample values.
            var mock_width = 3;
            var mock_height = 4;
            size = new Vector2(mock_width, mock_height);
            content = new string[checked(mock_width * mock_height)];
            for (int y = 0; y < mock_height; y++) {
                for (int x = 0; x < mock_width; x++) {
                    content[y * mock_width + x] = $"R{y}C{x}";
                }
            }
            start_index = new Vector2(0, 0);
            merged_cells = new List<((int, int), (int, int))> {
                ((0,2), (0,1)) // example: cells from x=0 length=2 merged on row y=0 length=1
            };
            // initialize read count
            read_row_count = 0;
        }

        // Return the row at the given row_index (0-based). If out of range or content is null, returns an empty array.
        public string[] get_row(int row_index) {
            if (content == null) return Array.Empty<string>();
            if (row_index <0 || row_index >= size.y) return Array.Empty<string>();
            int width = size.x;
            var row = new string[width];
            int base_idx = row_index * width;
            for (int x =0; x < width; x++) {
                int idx = base_idx + x;
                row[x] = (idx >=0 && idx < content.Length) ? content[idx] : null;
            }
            return row;
        }
    }
}
