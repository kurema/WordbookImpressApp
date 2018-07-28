using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressApp.Resx;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TutorialsPage : CarouselPage
	{
        public WordbookImpressLibrary.ViewModels.TutorialPagesViewModel Model => this.BindingContext as WordbookImpressLibrary.ViewModels.TutorialPagesViewModel;

        public TutorialsPage()
        {
            InitializeComponent();

            var model = GetTutorial();
            model.OnFinishAction = async () =>
             {
                 WordbookImpressLibrary.Storage.TutorialStorage.SetTutorialCompleted(true);
                 if (Navigation.ModalStack.Count > 0)
                 {
                     await Navigation.PopModalAsync();
                 }
                 else
                 {
                     await Navigation.PopAsync();
                 }
             };
            this.BindingContext = model;
        }

        public TutorialsPage(WordbookImpressLibrary.ViewModels.TutorialPagesViewModel model)
        {
            InitializeComponent();

            this.BindingContext = model;
        }

        public static WordbookImpressLibrary.ViewModels.TutorialPagesViewModel GetTutorial()
        {
            return new WordbookImpressLibrary.ViewModels.TutorialPagesViewModel(
                new System.Collections.ObjectModel.ObservableCollection<WordbookImpressLibrary.ViewModels.TutorialPageViewModel>()
                {
                    new WordbookImpressLibrary.ViewModels.TutorialPageViewModel(AppResources.TutorialPage1Title,AppResources.TutorialPage1Detail,AppResources.TutorialPage1Image,"#FFFFFF"),
                    new WordbookImpressLibrary.ViewModels.TutorialPageViewModel(AppResources.TutorialPage2Title,AppResources.TutorialPage2Detail,AppResources.TutorialPage2Image,"#FFFFFF"),
                    new WordbookImpressLibrary.ViewModels.TutorialPageViewModel(AppResources.TutorialPage3Title,AppResources.TutorialPage3Detail,AppResources.TutorialPage3Image,"#FFFFFF"),
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
                if (Navigation.ModalStack.Count > 0)
                {
                    await Navigation.PopModalAsync();
                }
                else
                {
                    await Navigation.PopAsync();
                }
            }
        }

        private void Button_Clicked_Finish(object sender, EventArgs e)
        {
            Finish();
        }
    }
}