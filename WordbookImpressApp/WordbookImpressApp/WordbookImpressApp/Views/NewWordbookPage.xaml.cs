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

        public NewWordbookPage()
        {
            InitializeComponent();

            ModelImpress = new RegisterWordbookViewModel() { Format = WordbookImpressInfo.Formats.QuizGenerator };

            //elv.Entries = new EntryListViewItem[] {
            //    new EntryListViewItem(){Title="タイトル",ImageUrl="icon_g_title.png",PlaceHolder="タイトル (任意)"},
            //    new EntryListViewItem(){Title="タイトル",ImageUrl="icon_g_title.png",PlaceHolder="タイトル (任意)"},
            //};
            //elv.Update();
        }

        bool Adding = false;

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            if (Adding) return;
            Adding = true;
            WordbookImpress result;
            var wbi = ModelImpress.GetWordbookInfo();
            try
            {
                var res = await WordbookImpress.Load(wbi);
                wbi.Format = res.format;
                result = res.wordbook;
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

            //I dont think MessagingCenter is useful. event is enough.
            //MessagingCenter.Send(this, "AddItem", this.WordbookInfo);
            await Navigation.PopAsync();

            Adding = false;
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}