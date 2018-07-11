using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.ViewModels
{
    public class TestStatusViewModel : BaseViewModel
    {
        public Models.Record.TestStatus Content;

        public QuizWordChoiceViewModel.ChoiceKind ChoiceKind => Content.ChoiceKind;
        public QuizWordChoiceViewModel.RetryStatusEnum RetryStatus => Content.RetryStatus;
        public string Key => Content.Key;
        public int AnswerCountTotal => Content.AnswerCountTotal;
        public int AnswerCountCorrect => Content.AnswerCountCorrect;
        public int AnswerCountPass => Content.AnswerCountPass;
        public double AnswerCountCorrectRate => Models.Record.TestStatus.GetCorrectRate(AnswerCountCorrect, AnswerCountTotal);
        public double AnswerCountCorrectPercentage => AnswerCountCorrectRate * 100;
        public TimeSpan ElapsedTime => Content.ElapsedTime;
        public TimeSpan ElapsedTimeAverage => new TimeSpan(Content.ElapsedTime.Ticks / Math.Max(1, AnswerCountTotal));
        public int Seed => Content.Seed;
        public DateTime DateTimeStartLocal => Content.DateTimeNative.ToLocalTime();
        public DateTime DateTimeEndLocal => Content.DateTimeNative.ToLocalTime() + ElapsedTime;
        private WordbookImpressViewModel target;
        public WordbookImpressViewModel Target
        {
            get
            {
                if (target != null) return target;
                var result = new List<Models.WordbookImpress>();
                foreach(var item in Storage.WordbooksImpressStorage.Content)
                {
                    if(Models.Record.TestStatus.KeyEqual(item.Uri, Key))
                    {
                        result.Add(item);
                    }
                }
                if (result.Count == 1)
                {
                    return target = new WordbookImpressViewModel(result[0], Storage.RecordStorage.Content);
                }
                else {
                    return target = new WordbookImpressViewModel(result, Storage.RecordStorage.Content, "複数単語帳");
                }
            }
        }

        public TestStatusViewModel()
        {
            this.Content = new Models.Record.TestStatus();
        }

        public TestStatusViewModel(Models.Record.TestStatus Content)
        {
            this.Content = Content;
        }

        public static ObservableCollection<TestStatusViewModel> GetTestStatusesByKey(string Key)
        {
            return GetTestStatusesGeneral((s) => Models.Record.TestStatus.KeyEqual(s.Key, Key));
        }

        public static ObservableCollection<TestStatusViewModel> GetTestStatusesTotal()
        {
            return GetTestStatusesGeneral((s) => true);
        }

        public static ObservableCollection<TestStatusViewModel> GetTestStatusesGeneral(Func<Models.Record.TestStatus,bool> check)
        { 
            var result = new List<TestStatusViewModel>();
            foreach(var item in Storage.RecordStorage.Content.TestStatuses)
            {
                if (check(item))
                {
                    result.Add(new TestStatusViewModel(item));
                }
            }
            result.Reverse();
            return new ObservableCollection<TestStatusViewModel>(result);
        }
    }
}
