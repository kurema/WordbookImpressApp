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
	public partial class QuizWordChoicePage : ContentPage
	{
        QuizWordChoiceViewModel Model => this.BindingContext != null && this.BindingContext is QuizWordChoiceViewModel ? (QuizWordChoiceViewModel)this.BindingContext : null;

        public QuizWordChoicePage ()
		{
			InitializeComponent ();
		}

        public QuizWordChoicePage(QuizWordChoiceViewModel model,bool Continue=false) : this()
        {
            this.BindingContext = model;
            if (!Continue)
            {
                model.Start();
            }
            else
            {
                model.Continue();
            }
        }

        protected override bool OnBackButtonPressed()
        {
            var result = base.OnBackButtonPressed();
            return result;
        }
        protected override void OnDisappearing()
        {
            Model.End();
            RecordStorage.SaveLocalData();
            var page = new QuizResultPage(new QuizResultViewModel(Model));
            NavigationPage.SetHasNavigationBar(page, false);
            Navigation.PushAsync(page);
            base.OnDisappearing();
        }

        private bool Pushing = false;
        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            if (Pushing) return;
            Pushing = true;
            if (Model.CurrentQuizStatus == QuizWordChoiceViewModel.QuizStatus.Choice)
            {
                Model.Choose(((QuizWordChoiceViewModel.ChoicesEnumerable.ChoicesEnumerableItem)e.SelectedItem).Text);
            }
            else if (Model.NextQuizCommand.CanExecute(null))
            {
                Model.NextQuizCommand.Execute(null);
                ((ListView)sender).SelectedItem = null;
            }
            else
            {
                await Navigation.PopAsync();
            }
            Pushing = false;
        }

        private async void ClickGestureRecognizer_Clicked(object sender, EventArgs e)
        {
            if (Pushing) return;
            Pushing = true;
            if (Model.CurrentQuizStatus == QuizWordChoiceViewModel.QuizStatus.Choice) { }
            else if (Model.NextQuizCommand.CanExecute(null))
            {
                Model.NextQuizCommand.Execute(null);
                ChoiceListView.SelectedItem = null;
            }
            else
            {
                await Navigation.PopAsync();
            }
            Pushing = false;
        }
    }
}