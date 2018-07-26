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

using System.Collections.ObjectModel;

using WordbookImpressApp.Resx;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WordPage : ContentPage
	{
        private IWordViewModel Model
        {
            get
            {
                if (this.BindingContext == null || !(this.BindingContext is IWordViewModel)) return null;
                else return (IWordViewModel)BindingContext;
            }
        }

        public WordPage ()
		{
			InitializeComponent ();
        }

        public WordPage(IWordViewModel model):this()
        {
            this.BindingContext = model;
        }

        protected override void OnParentSet()
        {
            Proceed.BindingContext = new ProceedViewModel(this);

            base.OnParentSet();
        }

        private void Button_Clicked_SearchWeblio(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(String.Format(AppResources.WordPageSearchDictionaryUrl, System.Web.HttpUtility.UrlEncode(Model.Head))));
        }

        private void Button_Clicked_SearchWikipedia(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(String.Format(AppResources.WordPageSearchWikipediaUrl, System.Web.HttpUtility.UrlEncode(Model.Head))));
        }

        private void Button_Clicked_SearchGoogle(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(String.Format(AppResources.WordPageSearchGoogleUrl, System.Web.HttpUtility.UrlEncode(Model.Head))));
        }

        public class ProceedViewModel : BaseViewModel
        {
            private WordPage page;
            private IWordViewModel model;
            private CarouselPage parentPage => (page?.Parent as CarouselPage);
            private Collection<IWordViewModel> items => (parentPage?.BindingContext as Collection<IWordViewModel>);
            private int currentCount => items?.IndexOf(model) ?? -1;

            public ProceedViewModel(WordPage p)
            {
                page = p;
                if (p.BindingContext != null && p.BindingContext is IWordViewModel)
                {
                    model = p.BindingContext as IWordViewModel;
                    model.PropertyChanged += (s, e) => this.OnPropertyChanged(nameof(Message));
                }
                if (model != null)
                {
                    model.PropertyChanged += (s, e) =>
                    {
                        ProceedCommand?.OnCanExecuteChanged();
                        OnPropertyChanged(nameof(Message));
                    };
                }
            }

            public string Message { get => CanFlip ? AppResources.WordPageBottomShow : (CanGoNextPage?AppResources.WordPageBottomNext : AppResources.WordPageBottomLast); }
            public bool IsVisible { get => items!=null; }

            public bool CanGoNextPage => (currentCount + 1 < items?.Count() && currentCount != -1);

            public void GoNextPage() { if (CanGoNextPage) parentPage.SelectedItem = items[currentCount + 1]; }

            public bool CanProceed => CanGoNextPage || CanFlip;

            public bool CanFlip => model != null ? (!model.IsVisibleHead || !model.IsVisibleDescription) : false;

            public void Proceed()
            {
                if(CanFlip)
                {
                    model.IsVisibleHead = model.IsVisibleDescription = true;
                    return;
                }
                if (CanGoNextPage)
                {
                    GoNextPage();
                }
            }

            private WordbookImpressLibrary.Helper.DelegateCommand proceedCommand;
            public WordbookImpressLibrary.Helper.DelegateCommand ProceedCommand => proceedCommand = proceedCommand ?? new WordbookImpressLibrary.Helper.DelegateCommand((o) => CanProceed, (o) => { Proceed(); });

        }

    }
}