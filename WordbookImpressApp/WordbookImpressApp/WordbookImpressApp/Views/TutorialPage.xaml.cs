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
	public partial class TutorialPage : ContentPage
    {
        public bool IsParentCarouselPage => this.Parent is CarouselPage;
        //public int ParentPagesTotal => (this.Parent as CarouselPage)?.ItemsSource.
        //public int ParentPagesCurrent => (this.Parent as CarouselPage)?.ItemsSource?.IndexOf(this.BindingContext) ?? -1;

        public TutorialPage ()
		{
			InitializeComponent ();

            this.BindingContext = this;
		}

        //private WordbookImpressLibrary.Helper.DelegateCommand nextPageCommand;
        //public WordbookImpressLibrary.Helper.DelegateCommand NextPageCommand =>
        //    nextPageCommand = nextPageCommand ?? new WordbookImpressLibrary.Helper.DelegateCommand(
        //        (o) => ParentPagesCurrent != -1 && ParentPagesCurrent + 1 < ParentPagesTotal,
        //        (o)=>
        //        );

        public event EventHandler TutorialFinished;

        private void Button_Clicked_Finish(object sender, EventArgs e)
        {
            TutorialFinished?.Invoke(this, new EventArgs());
        }
    }
}