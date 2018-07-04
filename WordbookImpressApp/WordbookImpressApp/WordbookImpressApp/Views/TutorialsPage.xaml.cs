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
	public partial class TutorialsPage : CarouselPage
	{
        public WordbookImpressLibrary.ViewModels.TutorialPagesViewModel Model => this.BindingContext as WordbookImpressLibrary.ViewModels.TutorialPagesViewModel;

        public TutorialsPage ()
		{
			InitializeComponent ();

            this.BindingContext = new WordbookImpressLibrary.ViewModels.TutorialPagesViewModel(
                new System.Collections.ObjectModel.ObservableCollection<WordbookImpressLibrary.ViewModels.TutorialPageViewModel>()
                {
                    new WordbookImpressLibrary.ViewModels.TutorialPageViewModel("正解画面","正解画面はこんな感じ","Correct.png"),
                    new WordbookImpressLibrary.ViewModels.TutorialPageViewModel("不正解画面","不正解画面はこんな感じ","Wrong.png"),
                }
                );
		}

        public async void Finish()
        {
            if (Model?.OnFinishAction != null)
            {
                Model?.OnFinishAction?.Invoke();
                return;
            }
            else
            {
                await Navigation.PopAsync();
            }
        }

        private void Button_Clicked_Finish(object sender, EventArgs e)
        {
            Finish();
        }
    }
}