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
	public partial class WordsPage : CarouselPage
    {
        private WordbookImpressViewModel Model
        {
            get
            {
                if (this.BindingContext == null || !(this.BindingContext is WordbookImpressViewModel)) return null;
                else return (WordbookImpressViewModel)BindingContext;
            }
        }

        public WordsPage(WordbookImpressViewModel model) : this()
        {
            this.BindingContext = model;
        }

        public WordsPage ()
		{
			InitializeComponent ();
		}

        protected override bool OnBackButtonPressed()
        {
            RecordStorage.SaveLocalData();

            return base.OnBackButtonPressed();
        }
    }
}