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

using WordbookImpressApp.Resx;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WordbooksPage : ContentPage
	{
		public WordbooksPage ()
		{
			InitializeComponent ();

            this.BindingContext = new WordbooksImpressViewModel(WordbooksImpressStorage.Content, RecordStorage.Content) { Order = ConfigStorage.Content?.SortKindWordbooks ?? default(WordbooksImpressViewModel.OrderStatus) };

            WordbooksImpressStorage.Updated += WordbookImpressStorage_Updated;
            this.Appearing += WordbookImpressStorage_Updated;
            ConfigStorage.Updated += WordbookImpressStorage_Updated;

            //MessagingCenter.Subscribe<NewWordbookPage, WordbookImpressInfo>(this, "AddItem", WordbookImpressStorage_Updated);
        }

        private void WordbookImpressStorage_Updated(object sender, object e)
        {
            this.BindingContext = new WordbooksImpressViewModel(WordbooksImpressStorage.Content, RecordStorage.Content) { Order = ConfigStorage.Content?.SortKindWordbooks ?? default(WordbooksImpressViewModel.OrderStatus) };
        }

        private bool Pushing = false;

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            if (Pushing) return;
            Pushing = true;
            await Navigation.PushAsync(new NewWordbookPage());
            Pushing = false;

        }

        private async void OnItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is WordbookImpressViewModel item))
                return;

            if (Pushing) return;
            Pushing = true;
            await Navigation.PushAsync(new WordbookPage(item));
            Pushing = false;

            // Manually deselect item.
            ItemsListView.SelectedItem = null;
        }

        private async void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            var dic = new Dictionary<string, WordbooksImpressViewModel.OrderKind>()
            {
                { AppResources.OrderEnumOriginal,WordbooksImpressViewModel.OrderKind.Default },
                { AppResources.OrderEnumTitle,WordbooksImpressViewModel.OrderKind.Title },
                { AppResources.OrderEnumUrl,WordbooksImpressViewModel.OrderKind.Url },
            };
            var current = ConfigStorage.Content.SortKindWordbooks;
            var dic2 = new Dictionary<string, WordbooksImpressViewModel.OrderStatus>();
            foreach (var item in dic)
            {
                if (current.Kind == item.Value)
                {
                    dic2.Add(item.Key + " "+AppResources.OrderCurrentMark + " " + (current.Reversed ? AppResources.OrderAscending : AppResources.OrderDescending), new WordbooksImpressViewModel.OrderStatus() { Kind = item.Value, Reversed = !current.Reversed });
                }
                else {
                    dic2.Add(item.Key, new WordbooksImpressViewModel.OrderStatus() { Kind = item.Value, Reversed = false });
                }
            }
            var result = await DisplayActionSheet(AppResources.OrderActionMessage, AppResources.AlertCancel, null, dic2.Select(a => a.Key).ToArray());
            if (result == null || !dic2.ContainsKey(result)) return;
            var resultItem = ConfigStorage.Content.SortKindWordbooks = dic2[result];
            if(this.BindingContext is WordbooksImpressViewModel wvm)
            {
                wvm.Order = resultItem;
            }
            ConfigStorage.SaveLocalData();
        }
    }
}