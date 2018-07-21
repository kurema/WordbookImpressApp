using System;
using System.Collections.Generic;
using System.Text;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using System.Linq;

namespace WordbookImpressLibrary.Helper
{
    public static class SpreadSheet
    {
        public static Dictionary<RowColumn, string> GetXlsxDictionaryOpenXml(System.IO.Stream stream)
        {
            var dics = new Dictionary<RowColumn, string>();
            using (SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
            {
                var sheet = doc.WorkbookPart.Workbook.Descendants<Sheet>().FirstOrDefault();
                var workSheet = (doc.WorkbookPart.GetPartById(sheet.Id) as WorksheetPart)?.Worksheet;
                var stringTable = doc.WorkbookPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();
                if (workSheet == null) return dics;
                foreach (var row in workSheet.Descendants<Row>().ToArray())
                {
                    foreach (Cell cell in row)
                    {
                        string value = cell.InnerText;
                        if (cell.DataType?.Value == CellValues.SharedString)
                        {
                            if (stringTable != null)
                            {
                                var shared = stringTable.SharedStringTable.ElementAt(int.Parse(value));
                                if (shared.HasChildren)
                                {
                                    value = shared.FirstChild.InnerText;
                                }
                                else
                                {
                                    value = shared.InnerText;
                                }
                            }
                        }
                        dics.Add(new RowColumn(cell.CellReference), value);
                    }
                }
            }
            return dics;
        }

        public struct RowColumn
        {
            public int Row;
            public int Column;

            public RowColumn(int r, int c) { this.Row = r; this.Column = c; }

            public RowColumn(string arg)
            {
                var match = System.Text.RegularExpressions.Regex.Match(arg.ToUpper(), @"([a-zA-Z]+)(\d+)");
                int r = 0;
                var alphabets = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                foreach (var item in match.Groups[1].Value)
                {
                    r = r * alphabets.Length;
                    r += alphabets.IndexOf(match.Groups[1].Value);
                }
                Column = r + 1;
                if (int.TryParse(match.Groups[2].Value, out Row)) { } else { Row = 0; }
            }
        }
    }
}
