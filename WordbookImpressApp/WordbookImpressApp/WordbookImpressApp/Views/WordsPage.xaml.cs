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

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class WordsPage : CarouselPage
    {
        private ObservableCollection<IWordViewModel> Model
        {
            get
            {
                if (this.BindingContext == null || !(this.BindingContext is ObservableCollection<IWordViewModel>)) return null;
                else return (ObservableCollection<IWordViewModel>)BindingContext;
            }
        }


        public System.Threading.SemaphoreSlim CanPushSemaphore = new System.Threading.SemaphoreSlim(1,1);
        public WordsPage(WordbookImpressViewModel model) : this()
        {
            this.BindingContext = model.Words;
            model.PropertyChanged += async (s, e) =>
            {
                if (e.PropertyName == nameof(model.Words))
                {
                    await Task.Run(async () =>
                    {
                        await CanPushSemaphore.WaitAsync();
                        this.BindingContext = model.Words;
                        CanPushSemaphore.Release();
                    });
                }
            };
        }

        public WordsPage(ObservableCollection<IWordViewModel> model) : this()
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