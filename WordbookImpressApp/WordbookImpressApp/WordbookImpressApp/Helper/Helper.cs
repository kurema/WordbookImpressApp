using System;
using System.Collections.Generic;
using System.Text;

//using ClosedXML.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using System.Linq;

namespace WordbookImpressApp.Helper
{
    public static class Helper
    {
        //public static Dictionary<string, List<string>> GetXlsxDictionary(System.IO.Stream stream)
        //{
        //    var result = new Dictionary<string, List<string>>();
        //    var tables = new List<string>();
        //    tables.Add("error");

        //    var wordbook = new XLWorkbook(stream);
        //    var worksheet = wordbook.Worksheet(1);
        //    int lastColumn = worksheet.LastColumnUsed().ColumnNumber();
        //    for (int i = 1; i <= lastColumn; i++)
        //    {
        //        var cell = worksheet.Cell(1, i);
        //        var header = cell.Value.ToString();
        //        if (result.ContainsKey(header))
        //        {
        //            int j = 1;
        //            while (result.ContainsKey(header + j)) { j++; }
        //        }
        //        var lastRow = worksheet.Column(i).LastCellUsed().WorksheetRow().RowNumber();
        //        var item = new List<string>();
        //        for(int j = 1; j < lastRow; j++)
        //        {
        //            item.Add(worksheet.Cell(i, j).Value.ToString());
        //        }
        //    }
        //    return result;
        //}

        public static Dictionary<RowColumn, string> GetXlsxDictionaryOpenXml(System.IO.Stream stream)
        {
            var dics = new Dictionary<RowColumn, string>();
            using(SpreadsheetDocument doc = SpreadsheetDocument.Open(stream, false))
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

            public RowColumn(int r,int c) { this.Row = r;this.Column = c; }

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
                Row = r;
                if (int.TryParse(match.Groups[2].Value, out Column)) { } else { Column = 0; }
            }
        }

    }
}
