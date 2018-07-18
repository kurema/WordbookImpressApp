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
    public partial class NewWordbookPage : ContentPage
    {
        public WordbookImpressLibrary.Models.WordbookImpressInfo WordbookInfo { get; set; }
        public String TitleUser { get; set; }

        public NewWordbookPage()
        {
            InitializeComponent();

            WordbookInfo = new WordbookImpressLibrary.Models.WordbookImpressInfo();

            BindingContext = this;

            //picker.ItemsSource = new PickerItem[] {
            //    new PickerItem("インプレスブックス","impress"),
            //    new PickerItem("CSV","csv"),
            //};
        }

        public class PickerItem
        {
            public string Title { get; set; }
            public string Id { get; set; }
            public PickerItem(string title,string id)
            {
                this.Title = title;
                this.Id = id;
            }
            public override string ToString()
            {
                return Title;
            }
        }

        bool Adding = false;

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            if (Adding) return;
            Adding = true;
            WordbookImpress result;
            try
            {
                result = (await WordbookImpress.Load(WordbookInfo)).wordbook;
            }
            catch
            {
                await DisplayAlert("認証失敗", "認証に失敗しました。", "OK");
                Adding = false;
                return;
            }
            result.TitleUser = TitleUser;
            WordbooksImpressInfoStorage.Add(WordbookInfo);
            await WordbooksImpressInfoStorage.SaveLocalData();
            WordbooksImpressStorage.Add(result);
            await WordbooksImpressStorage.SaveLocalData();

            MessagingCenter.Send(this, "AddItem", this.WordbookInfo);
            await Navigation.PopAsync();

            Adding = false;
        }

        private async void Cancel_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}