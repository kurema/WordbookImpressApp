using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SpecialInformationItemPage : ContentPage
	{
        private WordbookImpressLibrary.ViewModels.BookInformationViewModel Model => this.BindingContext as WordbookImpressLibrary.ViewModels.BookInformationViewModel;

        public SpecialInformationItemPage ()
		{
			InitializeComponent ();
		}

        public SpecialInformationItemPage(WordbookImpressLibrary.ViewModels.BookInformationViewModel model):this()
        {
            this.BindingContext = model;
        }

        protected override void OnBindingContextChanged()
        {
            //if(Model!=null) Model.SimilarPrepared += Model_RelatedPrepared;

            base.OnBindingContextChanged();
        }

        private async void Model_RelatedPrepared(object sender, EventArgs e)
        {
            if (Model.Similar == null || Model.Similar.Count() == 0) return;
            storeItems.Clear();
            await storeItems.AddASIN(Model.Similar.ToArray());
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
            if (e.SelectedItem is WordbookImpressLibrary.ViewModels.BookInformationViewModel.BookInformationLinkViewModel item)
            {
                try { Device.OpenUri(new Uri(item.Url)); } catch { }
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string url = (string)((Button)sender).CommandParameter;
            await Navigation.PushAsync(new QRCodePage(url));
        }
    }
}