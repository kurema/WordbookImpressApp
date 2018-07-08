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
                    new WordbookImpressLibrary.ViewModels.TutorialPageViewModel("本を買う","店頭やネットで\nインプレスブックスの対象書籍を購入","Tutorial_buy.jpg","#FFFFFF"),
                    new WordbookImpressLibrary.ViewModels.TutorialPageViewModel("特典を得る","公式サイトから特典を取得","Tutorial_privilege.png","#FFFFFF"),
                    new WordbookImpressLibrary.ViewModels.TutorialPageViewModel("登録する","このアプリで単語帳を登録","Tutorial_add.png","#FFFFFF"),
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