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

        public QuizWordChoicePage(QuizWordChoiceViewModel model) : this()
        {
            this.BindingContext = model;
            model.Start();
        }

        protected override bool OnBackButtonPressed()
        {
            Model.End();
            RecordStorage.SaveLocalData();
            return base.OnBackButtonPressed();
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            if (Model.CurrentQuizStatus == QuizWordChoiceViewModel.QuizStatus.Choice) Model.Choose(((QuizWordChoiceViewModel.ChoicesEnumerable.ChoicesEnumerableItem)e.SelectedItem).Text);
            else if (Model.NextQuizCommand.CanExecute(null))
            {
                Model.NextQuizCommand.Execute(null);
                ((ListView)sender).SelectedItem = null;
            }
            else await Navigation.PopModalAsync();
        }

        private async void ClickGestureRecognizer_Clicked(object sender, EventArgs e)
        {
            if (Model.CurrentQuizStatus == QuizWordChoiceViewModel.QuizStatus.Choice) { }
            else if (Model.NextQuizCommand.CanExecute(null))
            {
                Model.NextQuizCommand.Execute(null);
                ChoiceListView.SelectedItem = null;
            }
            else await Navigation.PopModalAsync();
        }
    }
}