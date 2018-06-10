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
	public partial class NewWordbookPage : ContentPage
	{
        public WordbookImpressLibrary.Models.WordbookInfo WordbookInfo { get; set; }

		public NewWordbookPage ()
		{
			InitializeComponent ();

            WordbookInfo = new WordbookImpressLibrary.Models.WordbookInfo();

            BindingContext = this;
		}

        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(this, "AddItem", WordbookInfo);
            await Navigation.PopModalAsync();
        }
    }
}