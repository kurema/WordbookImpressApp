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

            this.BindingContextChanged += (s, e) =>
            {
                PieGraph.Title = "スコア";
                PieGraph.Members = new List<PieGraphView.PieItem>()
                {
                    new PieGraphView.PieItem(){Color=new SkiaSharp.SKColor(0x52,0xb2,0xff),Rate=(float)Model.AnswerCorrectPercentage/100.0f },
                    new PieGraphView.PieItem(){Color=new SkiaSharp.SKColor(0xe8,0x6d,0x6b),Rate=(1- (float)Model.AnswerCorrectPercentage/100.0f) }
                };
                PieGraph.InvalidateSurface();
            };
		}

        public QuizResultPage(TestResultViewModel model) :this()
        {
            this.BindingContext = model;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            Device.OpenUri(new System.Uri("twitter://post?message=" + System.Web.HttpUtility.UrlEncode(Model.Wordbook.WordbookTitle+"のクイズで"+Model.AnswerCountTotal.ToString()+"問中"+ Model.AnswerCountCorrect.ToString() + "問正解しました！\n#wordbook_impress ")));
        }

        private bool Pushing;
        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null || !(e.SelectedItem is TestResultViewModel.TestResultItemViewModel)) return;
            if (Pushing) return;
            Pushing = true;
            var page = new WordPage(((TestResultViewModel.TestResultItemViewModel)e.SelectedItem).Word);
            await Navigation.PushModalAsync(page);

            (sender as ListView).SelectedItem = null;
            Pushing = false;
        }
    }
}