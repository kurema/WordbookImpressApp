using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressLibrary.Storage;
using WordbookImpressLibrary.Models;
using WordbookImpressLibrary.ViewModels;

using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace WordbookImpressApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewWordbookPage : TabbedPage
    {
        public RegisterWordbookViewModel ModelImpress
        {
            get => TabImpress.BindingContext as RegisterWordbookViewModel;
            set => TabImpress.BindingContext = value;
        }

        public RegisterWordbookCsvViewModel ModelCsv
        {
            get => TabCsv.BindingContext as RegisterWordbookCsvViewModel;
            set => TabCsv.BindingContext = value;
        }

        public NewWordbookPage()
        {
            InitializeComponent();

            ModelImpress = new RegisterWordbookViewModel() { Format = WordbookImpressInfo.Formats.QuizGenerator, Url = WordbookImpressInfo.DefaultUrl };
            ModelCsv = new RegisterWordbookCsvViewModel() { Format = WordbookImpressInfo.Formats.Csv };

            EntryListTitleCsv.Entries= EntryListTitle.Entries = new EntryListViewItem[] {
                new EntryListViewItem(){Title="タイトル",ImageUrl="icon_g_title.png",PlaceHolder="タイトル (任意)",EntryBinding="Title"},
            };
            EntryListTitle.Update();
            EntryListTitleCsv.Update();

            EntryListLogin.Entries = new EntryListViewItem[] {
                new EntryListViewItem(){Title="URL",ImageUrl="icon_g_url.png",PlaceHolder="URL",EntryBinding="Url"},
                new EntryListViewItem(){Title="ID",ImageUrl="icon_g_id.png",PlaceHolder="ID",EntryBinding="ID"},
                new EntryListViewItem(){Title="Password",ImageUrl="icon_g_pw.png",PlaceHolder="Password",EntryBinding="Password"},
            };
            EntryListLogin.Update();
            EntryListLoginCsv.Entries = new EntryListViewItem[] {
                new EntryListViewItem(){Title="URL",ImageUrl="icon_g_url.png",PlaceHolder="URL",EntryBinding="Url"},
                new EntryListViewItem(){Title="ID",ImageUrl="icon_g_id.png",PlaceHolder="ID (任意)",EntryBinding="ID"},
                new EntryListViewItem(){Title="Password",ImageUrl="icon_g_pw.png",PlaceHolder="Password (任意)",EntryBinding="Password"},
            };
            EntryListLoginCsv.Update();

            EntryListEncodingCsv.Entries = EntryListTitle.Entries = new EntryListViewItem[] {
                new EntryListViewItem(){Title="文字コード",ImageUrl="icon_g_encoding.png",PlaceHolder="文字コード",EntryBinding="Encoding"},
            };
            EntryListEncodingCsv.Update();

            ModelCsv.Encoding = (Encoding.GetEncodings().FirstOrDefault(a => a.Name.ToLower() == "shift_jis")?.Name ?? Encoding.UTF8.EncodingName);
        }

        bool Adding = false;

        private async void AddItem_Clicked_Csv_Preview(object sender, EventArgs e)
        {
            var (isSmb, url) = WordbookImpressLibrary.Helper.Functions.DistinguishHttpCifs(ModelCsv.Url, ModelCsv.ID, ModelCsv.Password);
            if (isSmb == false)
            {
                var client = System.Net.WebRequest.Create(url);
                client.Credentials = new System.Net.NetworkCredential(ModelCsv.ID, ModelCsv.Password);
                using(var stream=(await client.GetResponseAsync()).GetResponseStream())
                {
                    SetSpreadsheet(stream, url);
                }
            }
            else if (isSmb == true)
            {
                try
                {
                    var file = new SharpCifs.Smb.SmbFile(url);
                    if (file.IsFile())
                    {
                        try
                        {
                            using (var stream = file.GetInputStream())
                            {
                                SetSpreadsheet(stream, url);
                            }
                        }
                        catch
                        {
                            await DisplayAlert("警告", "ファイルの読み取りに失敗しました。", "OK");
                            return;
                        }
                    }
                    else
                    {
                        await DisplayAlert("警告", "ファイルが見当たりません。", "OK");
                        return;
                    }
                }
                catch
                {
                    await DisplayAlert("警告", "ファイルの読み取りに失敗しました。", "OK");
                    return;
                }
            }

        }

        private void SetSpreadsheet(System.IO.Stream stream,string url)
        {
            if (System.IO.Path.GetExtension(url).ToLower() == ".csv")
            {
                System.Text.Encoding encoding;
                try
                {
                    encoding = Encoding.GetEncoding(ModelCsv.Encoding);
                }
                catch
                {
                    encoding = Encoding.UTF8;
                }
                using (var sr = new System.IO.StreamReader(stream, encoding))
                {
                    var dic = WordbookImpressLibrary.Helper.Functions.GetCsvDictionary(sr);
                    //sheetCsv.SetDitcionary(dic,30);
                    sheetImgCsv.Load(new WordbookImpressLibrary.Helper.SpreadSheet.SpreadSheetProviderCsv(dic));
                    sheetImgCsv.InvalidateSurface();
                    ModelCsv.CsvHeaders = dic.Select(a => a.Key).ToArray();
                }
            }
            else if (System.IO.Path.GetExtension(url).ToLower() == ".xlsx")
            {
                var dic = WordbookImpressLibrary.Helper.SpreadSheet.GetXlsxDictionaryOpenXml(stream);
                //sheetCsv.SetDitcionary(dic,30);
                sheetImgCsv.Load(new WordbookImpressLibrary.Helper.SpreadSheet.SpreadSheetProviderCellList(dic));
                sheetImgCsv.InvalidateSurface();
                ModelCsv.CsvHeaders = dic.Where(a => a.Key.Row == 0).OrderBy(a => a.Key.Column).Select(a => a.Value).ToArray();
            }
            return;
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            if (Adding) return;
            Adding = true;
            WordbookImpress result;
            var wbi = ModelImpress.GetWordbookInfo();
            try
            {
                var (wordbook, html, data, format) = await WordbookImpress.Load(wbi);
                wbi.Format = format;
                result = wordbook;
            }
            catch
            {
                await DisplayAlert("認証失敗", "認証に失敗しました。", "OK");
                Adding = false;
                return;
            }
            result.TitleUser = ModelImpress.Title;
            WordbooksImpressInfoStorage.Add(wbi);
            await WordbooksImpressInfoStorage.SaveLocalData();
            WordbooksImpressStorage.Add(result);
            await WordbooksImpressStorage.SaveLocalData();

            await Navigation.PopAsync();

            Adding = false;
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (ModelImpress == null) return;
            var options = new ObservableCollection<EntryWithOptionViewModel.EntryWithOptionViewModelEntry>();
            if (!string.IsNullOrEmpty(ModelImpress.Title))
            {
                var item = new EntryWithOptionViewModel.EntryWithOptionViewModelEntry(ModelImpress.Title, ModelImpress.Title);
                options.Add(item);
            }
            foreach (var item in RemoteStorage.GetNameSuggestions(ModelImpress.Url))
            {
                if (item != ModelImpress.Title)
                    options.Add(new EntryWithOptionViewModel.EntryWithOptionViewModelEntry(item, item));
            }
            var vm = (new EntryWithOptionViewModel("タイトルを入力してください。", options, ModelImpress.Title));
            var page = new EntryWithOptionPage(vm);
            await Navigation.PushAsync(page);
            await page.WaitEntry();
            var tmp = vm.GetValue<string>();
            if (tmp.Item2)
            {
                ModelImpress.Title = tmp.Item1;
            }
        }

        private void Editor_Completed(object sender, EventArgs e)
        {
            var text = ((Editor)sender).Text;
            if (string.IsNullOrWhiteSpace(text)) return;
            {
                var match = new Regex(@"https://impress.quizgenerator.net/impress/\d+impress/").Match(text);
                if (match.Success && (string.IsNullOrWhiteSpace(match.Value) || ModelImpress.Url.ToLower() == WordbookImpressInfo.DefaultUrl))
                {
                    ModelImpress.Url = match.Value;
                }
            }
            {
                var match = new Regex(@"[UＵuｕ][RＲrｒ][LＬlｌ][：\:\s\t]+(http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?)").Match(text);
                if (match.Success && (string.IsNullOrWhiteSpace(match.Groups[1].Value) || ModelImpress.Url.ToLower() == WordbookImpressInfo.DefaultUrl))
                {
                    ModelImpress.Url = match.Groups[1].Value;
                }
            }
            {
                var match = new Regex(@"[IＩiｉ][DDdｄ][：\:\s\t]+([a-zA-Z\d]+)").Match(text);
                if (match.Success && ModelImpress.ID == "")
                {
                    ModelImpress.ID = match.Groups[1].Value;
                }
            }
            {
                var match = new Regex(@"(パスワード|Password|[Ｐｐ]ａｓｓｗｏｒｄ)[：\:\s\t]+([a-zA-Z\d]+)",RegexOptions.IgnoreCase).Match(text);
                if (match.Success && ModelImpress.Password == "")
                {
                    ModelImpress.Password = match.Groups[2].Value;
                }
            }
        }

        private async void Button_Clicked_1(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SpecialInformationPage(SpecialInformationPage.GetGroupsByWordbooks()));
        }

        private async void Button_Clicked_2(object sender, EventArgs e)
        {
            var urls = WordbooksImpressStorage.Content.Select(a => a.Id);
            var items = WordbooksImpressInfoStorage.Content.Where(a => !urls.Contains(a.Url));
            if (items.Count() == 0)
            {
                await DisplayAlert("単語帳","削除された単語帳がありません。","OK");
                return;
            }
            var answer = await DisplayActionSheet("単語帳", "復活する単語帳を選択してください。", null, items.Select(a => a.Url).ToArray());
            var result = items.FirstOrDefault(a => a.Url == answer);
            if (result != null)
            {
                ModelImpress.Url = result.Url;
                ModelImpress.ID = result.ID;
                ModelImpress.Password = result.Password;
            }
        }

        private async void Button_Clicked_3(object sender, EventArgs e)
        {
            var options=new ObservableCollection<EntryWithOptionViewModel.EntryWithOptionViewModelEntry>();
            var encs = Encoding.GetEncodings().OrderBy(a => new[] { "shift_jis" , "utf-8","utf-16","utf-16be" }.Contains(a.Name.ToLower())?0:1).Select(enc => new EntryWithOptionViewModel.EntryWithOptionViewModelEntry(enc.Name, enc.Name));

            var vm = new EntryWithOptionViewModel("エンコードを設定してください", new ObservableCollection<EntryWithOptionViewModel.EntryWithOptionViewModelEntry>(encs) , ModelCsv.Encoding ?? ""
                , a=> { try { Encoding.GetEncoding(a.ToString()); } catch { return false; } return true; });
            var page = new EntryWithOptionPage(vm);
            await Navigation.PushAsync(page);
            await page.WaitEntry();
            ModelCsv.Encoding = vm.GetValue<string>().Item1;
        }

    }
}
