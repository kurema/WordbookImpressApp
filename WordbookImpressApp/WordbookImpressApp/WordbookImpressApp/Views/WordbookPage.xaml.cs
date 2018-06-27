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
	public partial class WordbookPage : ContentPage
	{
        private WordbookImpressViewModel Model
        {
            get
            {
                if (this.BindingContext == null || !(this.BindingContext is WordbookImpressViewModel)) return null;
                else return (WordbookImpressViewModel)BindingContext;
            }
        }

        public WordbookPage ()
		{
			InitializeComponent ();
		}

        public WordbookPage(WordbookImpressViewModel model):this()
        {
            this.BindingContext = model;

            WordbooksImpressStorage.Updated += WordbooksImpressStorage_Updated;
        }

        private void WordbooksImpressStorage_Updated(object sender, EventArgs e)
        {
            var model = this.Model;
            try
            {
                //ToList()はおかしいけどコストは0に近いと思う。
                this.BindingContext = new WordbookImpressViewModel(WordbooksImpressStorage.Content.ToList().Find(w => w.Uri == this.Model.Uri), Model.Record);
            }
            catch
            {
                this.BindingContext = model;
            }
        }

        protected override void OnDisappearing()
        {
            WordbooksImpressStorage.Updated -= WordbooksImpressStorage_Updated;
            base.OnDisappearing();
        }

        private bool Pushing = false;

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (Pushing) return;
            Pushing = true;
            await Navigation.PushModalAsync(new QuizWordChoicePage(new QuizWordChoiceViewModel(Model, QuizWordChoiceViewModel.ChoiceKind.Title)));
            Pushing = false;
        }

        private async void Button_Clicked2(object sender, EventArgs e)
        {
            if (Pushing) return;
            Pushing = true;
            await Navigation.PushModalAsync(new QuizWordChoicePage(new QuizWordChoiceViewModel(Model, QuizWordChoiceViewModel.ChoiceKind.Description)));
            Pushing = false;
        }

        private async void Button_Clicked_WordsPage(object sender, EventArgs e)
        {
            if (Pushing) return;
            Pushing = true;
            //var page = new CarouselPage() { ItemTemplate = new DataTemplate(typeof(WordPage)), ItemsSource = Model.Words };
            var page = new WordsPage(Model);
            await Navigation.PushAsync(page);
            Pushing = false;
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null || !(e.SelectedItem is WordViewModel)) return;
            if (Pushing) return;
            Pushing = true;
            var page = new WordsPage(Model);
            page.SelectedItem = (WordViewModel)e.SelectedItem;
            await Navigation.PushAsync(page);

            (sender as ListView).SelectedItem = null;
            Pushing = false;
        }

        private async void ToolbarItem_Clicked_Delete(object sender, EventArgs e)
        {
            if (Model.HasMultipleWordbook)
            {
                await DisplayAlert("警告", "総合単語帳は削除できません。", "はい");
                return;
            }
            var alertResult = await DisplayAlert("警告", "この単語帳を削除しますか。", "はい", "いいえ");
            if (!alertResult) return;
            WordbooksImpressStorage.Content.Remove(Model.Wordbooks[0]);
            //認証情報を削除するか。迷うが削除しない事にする。
            //WordbooksImpressInfoStorage.Content.RemoveAll((w) => w.Url == Model.Wordbook.Uri);
            await Navigation.PopAsync();
        }

        private async void ToolbarItem_Clicked_Sort(object sender, EventArgs e)
        {
            var currentOrder = Model.SortKind;
            var labels = new Dictionary<string,WordbookImpressViewModel.SortKindType>()
            {
                {"標準",WordbookImpressViewModel.SortKindType.original },
                {"単語名順",WordbookImpressViewModel.SortKindType.headword },
                {"成績順",WordbookImpressViewModel.SortKindType.score },
            };

            var options = new List<string>(labels.Count * 2);
            var optionsDic = new Dictionary<string,WordbookImpressViewModel.SortKindInfo>();
            string currentOrderName = "";
            foreach (var item in labels)
            {
                foreach(var b in new[] { true, false })
                {
                    var word = item.Key + (b ? " (昇順)" : " (降順)");
                    var val = new WordbookImpressViewModel.SortKindInfo(item.Value, b);
                    options.Add(word);
                    optionsDic.Add(word, val);

                    if (currentOrder.Ascending == val.Ascending && currentOrder.Kind == val.Kind)
                        currentOrderName = word;
                }
            }

            string cancel = "キャンセル";
            var alertResult = await DisplayActionSheet("並び替え (現在 : "+currentOrderName+")", cancel, "", options.ToArray());
            if (string.IsNullOrEmpty(alertResult) || alertResult == cancel) { return; }
            ConfigStorage.Content.SortKind = Model.SortKind = optionsDic[alertResult];
            ConfigStorage.SaveLocalData();
        }
    }
}