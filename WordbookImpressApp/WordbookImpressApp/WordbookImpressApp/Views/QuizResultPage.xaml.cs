using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressLibrary.ViewModels;
using WordbookImpressApp.Resx;

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
                PieGraph.Title = AppResources.StatisticsPieGraphScore;
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

        private async void Button_Clicked_Tweet(object sender, EventArgs e)
        {
            string mon = "";
            var time= ValueConverters.TimeSpanFormatValueConverter.FormatTimeSpan(Model.ElapsedTime, AppResources.StatisticsTwitterTimeFormat);
            if (Model.AnswerCountTotal == 0)
            {
                mon = AppResources.StatisticsTwitterMessageNoAnswer;
            }
            else if (Model.AnswerCountCorrect == 0)
            {
                mon = String.Format(AppResources.StatisticsTwitterMessageAllWrong, Model.AnswerCountTotal);
            }
            else if (Model.AnswerCountTotal == Model.AnswerCountCorrect)
            {
                mon =String.Format(AppResources.StatisticsTwitterMessageAllCorrect,  Model.AnswerCountTotal);
            }
            else
            {
                mon = string.Format(AppResources.StatisticsTwitterMessageAnswerCount, Model.AnswerCountTotal, Model.AnswerCountCorrect, Model.AnswerCorrectPercentage);
            }

            var wb = WordbookImpressLibrary.Storage.RemoteStorage.GetBookWithWordbook(Model.Wordbook.Uri);
            /*string adTextAmazon = "";
            if (wb.Count() > 0) {
                Random rd = new Random();
                var id1 = wb[rd.Next(wb.Count())]?.ids?.Where((t) => t.type == "ASIN" && t.binding== "printed_book");
                Nager.AmazonProductAdvertising.Model.AmazonItemResponse itemResponse = null;
                if (id1.Count() > 0 && id1.First().Value != null)
                {
                    itemResponse = await WordbookImpressLibrary.Storage.AmazonStorage.AmazonWrapperShare.LookupAsync(id1.First().Value);
                }
                else
                {
                    var id2 = wb[0]?.ids?.Where((t) => t.type == "ASIN" && t.binding == "ebook");
                    if (id2.Count() > 0 && id2.First().Value != null)
                    {
                        itemResponse = await WordbookImpressLibrary.Storage.AmazonStorage.AmazonWrapperShare.LookupAsync(id2.First().Value);
                    }
                }
                if (itemResponse?.Items?.Item?.Length > 0)
                {
                    var url = Uri.EscapeUriString(itemResponse.Items.Item[0].DetailPageURL);
                    //I thought it's good idea to percent-encode Amazon Associate Tag which is Ascii for security, but RFC3986 says no. Maybe that's why the easy method is not provided.
                    //url = url.Replace(WordbookImpressLibrary.APIKeys.AmazonAssociateTagShare, System.Web.HttpUtility.UrlEncode(WordbookImpressLibrary.APIKeys.AmazonAssociateTagShare));
                    adTextAmazon = "\n\n" + String.Format(AppResources.StatisticsTwitterMessageAd, wb[0].title,url);
                }
            }*/
            string message = System.Web.HttpUtility.UrlEncode(
                String.Format(AppResources.StatisticsTwitterMessageTotal, Model.Wordbook.WordbookTitle, time, mon, string.Empty));

            try
            {
                Device.OpenUri(new Uri("twitter://post?message=" + message));
            }
            catch {
                try
                {
                    Device.OpenUri(new Uri("http://twitter.com/?status=" + message));
                }
                catch { }
            }
        }

        private bool Pushing;
        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null || !(e.SelectedItem is QuizResultViewModel.TestResultItemViewModel)) return;
            if (Pushing) return;
            Pushing = true;
            try
            {
                await WordsPageSemaphore.WaitAsync();
                var page = WordsPage;
                var selectTarget = ((QuizResultViewModel.TestResultItemViewModel)e.SelectedItem).Word;
                page.SelectedItem = selectTarget;
                NavigationPage.SetHasNavigationBar(page, false);
                await Navigation.PushAsync(page);
            }
            catch { }
            finally
            {
                WordsPageSemaphore.Release();
            }
            (sender as ListView).SelectedItem = null;
            Pushing = false;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Task.Run(async () =>
            {
                try
                {
                    await WordsPageSemaphore.WaitAsync();
                    //var temp = WordsPage;
                }
                catch { }
                finally
                {
                    WordsPageSemaphore.Release();
                }
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
            var page = new QuizWordChoicePage(new QuizWordChoiceViewModel(Model.Wordbook, Model.Seed, Model.ChoiceKind) { RetryStatus = QuizWordChoiceViewModel.RetryStatusEnum.Retry });
            NavigationPage.SetHasNavigationBar(page, false);
            await Navigation.PushAsync(page);
            Pushing = false;
        }

        private async void Button_Clicked_Continue(object sender, EventArgs e)
        {
            if (Model == null) return;
            Pushing = true;
            OnBackButtonPressed();
            var page = new QuizWordChoicePage(Model.QuizWordChoice, true);
            NavigationPage.SetHasNavigationBar(page, false);
            await Navigation.PushAsync(page);
            Pushing = false;
        }
    }
}