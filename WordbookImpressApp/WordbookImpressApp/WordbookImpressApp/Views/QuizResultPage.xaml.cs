using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressLibrary.ViewModels;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class QuizResultPage : ContentPage
	{
        private TestResultViewModel Model
        {
            get
            {
                if (this.BindingContext == null || !(this.BindingContext is TestResultViewModel)) return null;
                else return (TestResultViewModel)BindingContext;
            }
        }
        public QuizResultPage ()
		{
			InitializeComponent ();
		}

        public QuizResultPage(TestResultViewModel model) :this()
        {
            this.BindingContext = model;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new System.Uri("twitter://post?message=" + Model.AnswerCountCorrect.ToString() + "問正解しました#impress_book_app"));
        }
    }
}