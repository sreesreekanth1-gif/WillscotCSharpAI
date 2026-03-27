using ClosedXML.Excel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Framework.Utilities
{
    public static class ExcelReader
    {
        public static DataTable Read(string fileName, string sheetName)
        {
            var path = Path.Combine(Directory.GetCurrentDirectory(), "TestData", fileName);
            if (!File.Exists(path)) throw new FileNotFoundException($"Excel file not found: {path}");

            using var workbook = new XLWorkbook(path);
            var ws = workbook.Worksheet(sheetName);
            if (ws == null) throw new ArgumentException($"Worksheet '{sheetName}' not found in {fileName}");

            var table = new DataTable();
            var firstRow = ws.FirstRowUsed();
            if (firstRow == null) return table;

            foreach (var cell in firstRow.CellsUsed())
                table.Columns.Add(cell.GetString());

            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var dataRow = table.NewRow();
                var cells = row.Cells(1, table.Columns.Count).ToList();
                for (int i = 0; i < cells.Count; i++)
                {
                    dataRow[i] = cells[i].Value.ToString() ?? string.Empty;
                }
                table.Rows.Add(dataRow);
            }

            return table;
        }

        public static IEnumerable<TestCaseData> GetTestData(string fileName, string sheetName)
        {
            var table = Read(fileName, sheetName);
            foreach (DataRow row in table.Rows)
            {
                yield return new TestCaseData(row);
            }
        }
    }
}
