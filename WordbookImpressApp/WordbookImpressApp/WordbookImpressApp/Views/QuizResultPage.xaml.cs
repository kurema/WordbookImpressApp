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
        private QuizResultViewModel Model
        {
            get
            {
                if (this.BindingContext == null || !(this.BindingContext is QuizResultViewModel)) return null;
                else return (QuizResultViewModel)BindingContext;
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

        public QuizResultPage(QuizResultViewModel model) :this()
        {
            this.BindingContext = model;
        }

        private void Button_Clicked_Tweet(object sender, EventArgs e)
        {
            string mon = "";
            var time= ValueConverters.TimeSpanFormatValueConverter.FormatTimeSpan(Model.ElapsedTime, "[if:TotalMinutesFloor:[TotalMinutesFloor:{0:00}]分][Seconds:{0:00}]秒");
            if (Model.AnswerCountTotal == 0)
            {
                mon = "1問も解答しませんでした！";
            }
            else if (Model.AnswerCountCorrect == 0)
            {
                mon = Model.AnswerCountTotal.ToString() + "問全て間違えました！";
            }
            else if (Model.AnswerCountTotal == Model.AnswerCountCorrect)
            {
                mon = Model.AnswerCountTotal.ToString() + "問全て正解しました！";
            }
            else
            {
                mon = Model.AnswerCountTotal.ToString() + "問中" + Model.AnswerCountCorrect.ToString() + "問正解しました！";
            }
            Device.OpenUri(new System.Uri("twitter://post?message=" + System.Web.HttpUtility.UrlEncode(Model.Wordbook.WordbookTitle + "のクイズを" + time + "で" + mon + "\n#wordbook_impress ")));
        }

        private bool Pushing;
        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null || !(e.SelectedItem is QuizResultViewModel.TestResultItemViewModel)) return;
            if (Pushing) return;
            Pushing = true;
            await WordsPageSemaphore.WaitAsync();
            var page = WordsPage;
            var selectTarget = ((QuizResultViewModel.TestResultItemViewModel)e.SelectedItem).Word;
            page.SelectedItem = selectTarget;
            await Navigation.PushModalAsync(page);
            WordsPageSemaphore.Release();
            (sender as ListView).SelectedItem = null;
            Pushing = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Task.Run(async () =>
            {
                await WordsPageSemaphore.WaitAsync();
                var temp = WordsPage;
                WordsPageSemaphore.Release();
            });
        }

        private WordsPage wordsPage;
        private WordsPage WordsPage => wordsPage = wordsPage ?? GetWordsPage();

        private WordsPage GetWordsPage()
        {
            var words = new System.Collections.ObjectModel.ObservableCollection<IWordViewModel>();
            foreach (var item in Model.Items)
            {
                words.Add(item.Word);
            }
            return new WordsPage(words);
        }

        public System.Threading.SemaphoreSlim WordsPageSemaphore = new System.Threading.SemaphoreSlim(1, 1);




        private async void Button_Clicked_Retry(object sender, EventArgs e)
        {
            //https://stackoverflow.com/questions/36892044/clear-xamarin-forms-modal-stack/36893908
            if (Model == null) return;
            Pushing = true;
            //workaround.
            OnBackButtonPressed();
            await Navigation.PushModalAsync(new QuizWordChoicePage(new QuizWordChoiceViewModel(Model.Wordbook , Model.Seed, Model.ChoiceKind) { RetryStatus=QuizWordChoiceViewModel.RetryStatusEnum.Retry}));
            Pushing = false;
        }

        private async void Button_Clicked_Continue(object sender, EventArgs e)
        {
            if (Model == null) return;
            Pushing = true;
            OnBackButtonPressed();
            await Navigation.PushModalAsync(new QuizWordChoicePage(Model.QuizWordChoice, true));
            Pushing = false;
        }
    }
}