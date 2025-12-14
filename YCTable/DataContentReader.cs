using System;
using System.Collections.Generic;

namespace YCTable {
 // Reads data content rows from an ExcelTable (rows after header)
 public class DataContentReader {
 public ExcelTable excel_table { get; }
 // data_start_row: inclusive start row index for data area (0-based)
 public int data_start_row { get; set; }

 public DataContentReader(ExcelTable excel_table, int data_start_row =1) {
 this.excel_table = excel_table ?? throw new ArgumentNullException(nameof(excel_table));
 this.data_start_row = data_start_row;
 }

 // Read all data rows as array of string[] rows (each row length = width)
 public List<string[]> read_all_rows() {
 var width = excel_table.size.x;
 var height = excel_table.size.y;
 var rows = new List<string[]>();
 if (excel_table.content == null) return rows;
 if (data_start_row <0 || data_start_row >= height) return rows;

 for (int y = data_start_row; y < height; y++) {
 var row = new string[width];
 var base_idx = y * width;
 for (int x =0; x < width; x++) {
 var idx = base_idx + x;
 row[x] = (idx >=0 && idx < excel_table.content.Length) ? excel_table.content[idx] : null;
 }
 rows.Add(row);
 }
 return rows;
 }
 }
}
