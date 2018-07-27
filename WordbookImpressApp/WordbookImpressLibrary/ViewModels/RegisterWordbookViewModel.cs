using System;
using System.Collections.Generic;
using System.Text;

using System.Linq;

namespace WordbookImpressLibrary.ViewModels
{
    public class RegisterWordbookViewModel : BaseViewModel
    {
        private string _Title = "";
        public string Title { get => _Title; set => SetProperty(ref _Title, value); }

        private string _Url = "";
        public string Url { get => _Url; set => SetProperty(ref _Url, value); }

        private string _ID = "";
        public string ID { get => _ID; set => SetProperty(ref _ID, value); }

        private string _Password = "";
        public string Password { get => _Password; set => SetProperty(ref _Password, value); }

        private string _Format = "";
        public string Format { get => _Format; set => SetProperty(ref _Format, value); }

        public Models.WordbookImpressInfo GetWordbookInfo()
        {
            return new Models.WordbookImpressInfo() {
                Format = this.Format,
                ID=this.ID,
                Password=this.Password,
                Url=this.Url
            };
        }
    }

    public class RegisterWordbookCsvViewModel: RegisterWordbookViewModel
    {
        private CsvHeader _CsvHeadKey;
        public CsvHeader CsvHeadKey { get => _CsvHeadKey; set => SetProperty(ref _CsvHeadKey, value); }

        private CsvHeader _CsvDescriptionKey;
        public CsvHeader CsvDescriptionKey { get => _CsvDescriptionKey; set => SetProperty(ref _CsvDescriptionKey, value); }

        private CsvHeader[] _CsvHeaders;
        public CsvHeader[] CsvHeaders { get => _CsvHeaders; set => SetProperty(ref _CsvHeaders, value); }

        private string _Encoding;
        public string Encoding { get => _Encoding; set => SetProperty(ref _Encoding, value); }

        private string _CurrentlyLoaded;
        public string CurrentlyLoaded { get => _CurrentlyLoaded; set => SetProperty(ref _CurrentlyLoaded, value); }

        private Helper.SpreadSheet.ISpreadSheetProvider _SpreadSheet;
        public Helper.SpreadSheet.ISpreadSheetProvider SpreadSheet { get => _SpreadSheet; set => SetProperty(ref _SpreadSheet, value); }

        public class CsvHeader
        {
            public string Title { get; set; }
            public int Index { get; set; }
        }

        public Models.WordbookCsv GetModel()
        {
            if (SpreadSheet == null || CsvHeadKey == null || CsvDescriptionKey == null || CsvHeadKey.Index == CsvDescriptionKey.Index) return null;
            var indexHead = CsvHeadKey.Index;
            var indexDesc = CsvDescriptionKey.Index;
            var size = SpreadSheet.GetSize();
            var words = SpreadSheet.GetCells()
                .Where(a => (a.RowColumn.Column == indexHead || a.RowColumn.Column == indexDesc) && a.RowColumn.Row != 0)
                .OrderBy(a => a.RowColumn.Row)
                .GroupBy(a => a.RowColumn.Row)
                .Select(a =>
                {
                    var t = a.FirstOrDefault(b => b.RowColumn.Column == indexHead);
                    var d = a.FirstOrDefault(b => b.RowColumn.Column == indexDesc);
                    if (String.IsNullOrWhiteSpace(t.Text) || String.IsNullOrWhiteSpace(d.Text)) return null;
                    return new Models.Word() { Title = t.Text, Description = d.Text };
                }).Where(a => a != null).ToArray();

            return new Models.WordbookCsv()
            {
                ColumnDescriptionHeader = CsvDescriptionKey.Title,
                ColumnDescriptionIndex = indexDesc,
                ColumnTitleHeader = CsvHeadKey.Title,
                ColumnTitleIndex = indexHead,
                Id = Guid.NewGuid().ToString(),
                OriginalUrl = Url,
                QuizChoices = new Models.QuizChoice[0],
                TitleUser = this.Title,
                Words = words,
                Authentication = new Models.Authentication()
                {
                    UserName = ID,
                    //Password is not recorder for security.
                }
            };
        }
    }
}
