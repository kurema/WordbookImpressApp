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
	public partial class StoreItemsCardContentView : ContentView
	{
        public string ItemsTitle { get => labelTitle.Text; set => labelTitle.Text = value; }
        public StoreItemsView StoreItems { get => storeItems; }
        private Action actionMore;
        public Action ActionMore { get => actionMore; set { labelMore.IsVisible = value != null; actionMore = value; } }


		public StoreItemsCardContentView()
		{
			InitializeComponent ();
		}

        public StoreItemsCardContentView(string title):this()
        {
            ItemsTitle = title;
        }

        private void ClickGestureRecognizer_Clicked(object sender, EventArgs e)
        {
            ActionMore?.Invoke();
        }
    }
}