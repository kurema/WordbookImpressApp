using System;
using System.Collections.Generic;
using System.Text;

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
        private string _CsvHeadKey;
        public string CsvHeadKey { get => _CsvHeadKey; set => SetProperty(ref _CsvHeadKey, value); }

        private string[] _CsvHeadKeys;
        public string[] CsvHeadKeys { get => _CsvHeadKeys; set => SetProperty(ref _CsvHeadKeys, value); }

        private string _CsvDescriptionKey;
        public string CsvDescriptionKey { get => _CsvDescriptionKey; set => SetProperty(ref _CsvDescriptionKey, value); }

        private string[] __CsvDescriptionKeys;
        public string[] _CsvDescriptionKeys { get => __CsvDescriptionKeys; set => SetProperty(ref __CsvDescriptionKeys, value); }

        private string _Encoding;
        public string Encoding { get => _Encoding; set => SetProperty(ref _Encoding, value); }
    }
}
