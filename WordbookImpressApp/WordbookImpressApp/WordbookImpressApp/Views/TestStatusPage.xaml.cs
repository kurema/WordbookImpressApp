using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressLibrary.Models;
using WordbookImpressLibrary.ViewModels;
using WordbookImpressLibrary.Storage;

using WordbookImpressApp.Resx;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TestStatusPage : ContentPage
	{
        public TestStatusesViewModel Model => BindingContext as TestStatusesViewModel;

        public TestStatusPage ()
		{
			InitializeComponent ();

            ConfigStorage.Updated += (s, e) => UpdateCalendarGraph();
        }

        public TestStatusPage(TestStatusesViewModel model):this()
        {
            this.BindingContext = model;
        }

        protected override void OnBindingContextChanged()
        {
            if (Model == null) return;

            UpdateCalendarGraph();

            PieGraph.Title = AppResources.TestStatusPieGraphScore;
            PieGraph.Members = new List<PieGraphView.PieItem>()
                {
                    new PieGraphView.PieItem(){Color=new SkiaSharp.SKColor(0x52,0xb2,0xff),Rate=(float)Model.Total.AnswerCountCorrectRate },
                    new PieGraphView.PieItem(){Color=new SkiaSharp.SKColor(0xe8,0x6d,0x6b),Rate=(1- (float)Model.Total.AnswerCountCorrectRate) }
                };
            PieGraph.InvalidateSurface();

            base.OnBindingContextChanged();
        }

        private void UpdateCalendarGraph()
        {
            if (Model == null) return;

            CalendarGraph.ColorTable = Model.DailyStatuses.ToDictionary(
                (a) => a.Key, (b) =>
                {
                    var rate = Record.TestStatus.GetCorrectRate(b.Value.AnswerCountCorrect, b.Value.AnswerCountTotal);
                    return new SkiaSharp.SKColor((byte)(255 * (1 - rate)), (byte)(255 * rate), 0
                        , (byte)(255 * Math.Min((float)ConfigStorage.Content.MaxDailyTestCount, b.Value.AnswerCountTotal) / Math.Max(ConfigStorage.Content.MaxDailyTestCount, 1)));
                }
                );
#if DEBUG
            if (ConfigStorage.Content.DemoModeCalendar)
            {
                var rand = new Random();
                CalendarGraph.ColorTable = Enumerable.Range(0, 365).Select(a => DateTime.Now.AddDays(-a).Date).ToDictionary(a => a, a =>
                {
                    var rate = 0.5+ rand.NextDouble()/2.0;
                    var rates = rand.NextDouble();
                    return new SkiaSharp.SKColor((byte)(255 * (1 - rate)), (byte)(255 * rate), 0
                        , (byte)(255 * rates));
                });
            }
#endif
            CalendarGraph.InvalidateSurface();
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) return;
            ((ListView)sender).SelectedItem = null;
            await Navigation.PushAsync(new QuizResultPage(new QuizResultViewModel(((TestStatusViewModel)e.SelectedItem))));
        }
    }
}