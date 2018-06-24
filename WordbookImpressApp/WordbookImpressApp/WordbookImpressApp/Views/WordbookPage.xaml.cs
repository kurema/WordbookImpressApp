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
        }

        private bool Pushing = false;

        private async void Button_Clicked(object sender, EventArgs e)
        {
            if (Pushing) return;
            Pushing = true;
            await Navigation.PushModalAsync(new QuizWordChoicePage(new QuizWordChoiceViewModel(Model, new ConfigViewModel()) { ChoiceType=QuizWordChoiceViewModel.ChoiceKind.Title}));
            Pushing = false;
        }

        private async void Button_Clicked2(object sender, EventArgs e)
        {
            if (Pushing) return;
            Pushing = true;
            await Navigation.PushModalAsync(new QuizWordChoicePage(new QuizWordChoiceViewModel(Model, new ConfigViewModel()) { ChoiceType = QuizWordChoiceViewModel.ChoiceKind.Description }));
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
    }
}