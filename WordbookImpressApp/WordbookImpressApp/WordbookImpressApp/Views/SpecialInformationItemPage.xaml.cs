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
        public SpecialInformationItemPage ()
		{
			InitializeComponent ();
		}

        protected override void OnBindingContextChanged()
        {
            //この辺をアンコメントするとなぜか別画面でも画面が真っ白になって操作不能になる。
            //理由はわけわからん。Xamarin Formsのバグだと思う。
            //if (Model != null) Model.SimilarPrepared += Model_RelatedPrepared;

            base.OnBindingContextChanged();
        }

        //private async void Model_RelatedPrepared(object sender, EventArgs e)
        //{
        //    if (Model.Similar == null || Model.Similar.Count() == 0) return;
        //    storeItems.Clear();
        //    await storeItems.AddASIN(Model.Similar.ToArray());
        //}

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            string url = (string)((Button)sender).CommandParameter;
            await Navigation.PushAsync(new QRCodePage(url));
        }
    }
}