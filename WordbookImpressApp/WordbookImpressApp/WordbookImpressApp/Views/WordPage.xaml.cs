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
	public partial class WordPage : ContentPage
	{
        private WordViewModel Model
        {
            get
            {
                if (this.BindingContext == null || !(this.BindingContext is WordViewModel)) return null;
                else return (WordViewModel)BindingContext;
            }
        }

        public WordPage ()
		{
			InitializeComponent ();
		}

        private void Button_Clicked_SearchWeblio(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://www.weblio.jp/content/" + System.Web.HttpUtility.UrlEncode(Model.Head)));
        }

        private void Button_Clicked_SearchWikipedia(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://ja.wikipedia.org/wiki/" + System.Web.HttpUtility.UrlEncode(Model.Head)));
        }

        private void Button_Clicked_SearchGoogle(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri("https://www.google.co.jp/search?q=" + System.Web.HttpUtility.UrlEncode(Model.Head)));
        }
    }
}