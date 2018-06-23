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
	public partial class QuizWordChoicePreparePage : ContentPage
	{
        QuizWordChoiceViewModel Model => this.BindingContext != null && this.BindingContext is QuizWordChoiceViewModel ? (QuizWordChoiceViewModel)this.BindingContext : null;

        public QuizWordChoicePreparePage ()
		{
			InitializeComponent ();
		}

        public QuizWordChoicePreparePage(QuizWordChoiceViewModel model) : this()
        {
            this.BindingContext = model;
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new Views.QuizWordChoicePage(Model));
        }
    }
}