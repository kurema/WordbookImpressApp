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
	public partial class WordbooksPage : ContentPage
	{
		public WordbooksPage ()
		{
			InitializeComponent ();

            this.BindingContext = new WordbooksImpressViewModel(WordbooksImpressStorage.Content, RecordStorage.Content);

            WordbooksImpressStorage.Updated += WordbookImpressStorage_Updated;
            this.Appearing += WordbookImpressStorage_Updated;

            MessagingCenter.Subscribe<NewWordbookPage, WordbookImpressInfo>(this, "AddItem", WordbookImpressStorage_Updated);
        }

        private void WordbookImpressStorage_Updated(object sender, object e)
        {
            this.BindingContext = new WordbooksImpressViewModel(WordbooksImpressStorage.Content, RecordStorage.Content);
        }

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new Views.NewWordbookPage()));
        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as WordbookImpressViewModel;
            if (item == null)
                return;

            await Navigation.PushAsync(new WordbookPage(item));

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }
    }
}