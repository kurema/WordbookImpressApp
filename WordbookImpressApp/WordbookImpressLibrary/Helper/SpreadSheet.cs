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
                    foreach (DocumentFormat.OpenXml.Spreadsheet.Cell cell in row)
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
                    r += alphabets.IndexOf(item.ToString().ToUpper()) + 1;
                }
                Column = r - 1;
                if (int.TryParse(match.Groups[2].Value, out Row)) { Row--; } else { Row = -1; }
            }
        }

        public struct Cell
        {
            public RowColumn RowColumn;
            public string Text;
        }

        public interface ISpreadSheetProvider
        {
            IEnumerable<Cell> GetCells();
            string GetCell(int column, int row);
            (int column, int row) GetSize();
        }

        public class SpreadSheetProviderCsv : ISpreadSheetProvider
        {
            public List<KeyValuePair<string, List<string>>> Content;

            public IEnumerable<Cell> GetCells()
            {
                int c = 0;
                foreach (var item in Content)
                {
                    int r = 0;
                    foreach (var cell in item.Value)
                    {
                        yield return new Cell() { RowColumn = new RowColumn(r,c), Text = cell };
                        r++;
                    }
                    c++;
                }
            }

            public string GetCell(int column, int row)
            {
                if (Content.Count > column && Content[column].Value.Count > row)
                {
                    return Content[column].Value[row];
                }
                else
                {
                    return null;
                }
            }

            public (int column, int row) GetSize()
            {
                var row = Content.Max((a) => a.Value.Count() + 1);
                return (Content.Count, row);
            }

            public SpreadSheetProviderCsv(List<KeyValuePair<string, List<string>>> arg)
            {
                Content = arg;
            }
        }

        public static List<KeyValuePair<string, List<string>>> GetCsvDictionary(System.IO.TextReader tr)
        {
            using (var reader = new CsvHelper.CsvReader(new CsvHelper.CsvParser(tr, System.Globalization.CultureInfo.InvariantCulture)))
            {
                reader.Read();
                reader.ReadHeader();
                var headers = new List<string>();
                var result = new List<KeyValuePair<string, List<string>>>();
                foreach (var item in reader.Context.HeaderRecord)
                {
                    {
                        result.Add(new KeyValuePair<string, List<string>>(item, new List<string>()));
                        headers.Add(item);
                    }
                }
                while (reader.Read())
                {
                    for (int i = 0; i < Math.Min(headers.Count, reader.Context.Record.Length); i++)
                    {
                        result[i].Value.Add(reader.Context.Record[i]);
                    }
                }
                return result;
            }
        }

        public class SpreadSheetProviderCellList : ISpreadSheetProvider
        {
            public Dictionary<WordbookImpressLibrary.Helper.SpreadSheet.RowColumn, string> Content;
            public IEnumerable<Cell> GetCells()
            {
                foreach (var cell in Content)
                {
                    yield return new Cell() {RowColumn=cell.Key, Text = cell.Value };
                }
            }

            public string GetCell(int column, int row)
            {
                int maxRow = 0;
                int maxColumn = 0;
                foreach(var item in Content)
                {
                    if(item.Key.Column==column && item.Key.Row == row)
                    {
                        return item.Value;
                    }
                    maxRow = Math.Max(maxRow, item.Key.Row);
                    maxColumn = Math.Max(maxColumn, item.Key.Column);
                }
                if (column <= maxColumn && row <= maxRow) return "";
                else return null;
            }

            public (int column, int row) GetSize()
            {
                int r = -1;
                int c = -1;
                foreach(var item in Content)
                {
                    r = Math.Max(item.Key.Row,r);
                    c = Math.Max(item.Key.Column, c);
                }
                return (c + 1, r + 1);
            }

            public SpreadSheetProviderCellList(Dictionary<RowColumn, string> arg)
            {
                this.Content = arg;
            }
        }
    }
}
