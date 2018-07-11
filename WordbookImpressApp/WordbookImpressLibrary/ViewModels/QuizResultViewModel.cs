using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;


namespace WordbookImpressLibrary.ViewModels
{
    public class QuizResultViewModel : BaseViewModel
    {
        private Record.TestStatus TestStatus => QuizWordChoice.TestStatus;
        public WordbookImpressViewModel Wordbook => QuizWordChoice.WordbookTargetViewModel;
        public QuizWordChoiceViewModel QuizWordChoice { get; private set; }

        public QuizResultViewModel(QuizWordChoiceViewModel quiz) { QuizWordChoice = quiz; }
        public QuizResultViewModel(TestStatusViewModel vm)
        {
            QuizWordChoice = new QuizWordChoiceViewModel(vm.Content);
        }

        public double AnswerCorrectPercentage => TestStatus.AnswerCountTotal == 0 ? 0 : (double)TestStatus.AnswerCountCorrect / TestStatus.AnswerCountTotal * 100.0;

        public ViewModels.QuizWordChoiceViewModel.ChoiceKind ChoiceKind => TestStatus.ChoiceKind;
        public int AnswerCountTotal => TestStatus.AnswerCountTotal;
        public int AnswerCountCorrect => TestStatus.AnswerCountCorrect;
        public int AnswerCountPass => TestStatus.AnswerCountPass;
        public TimeSpan ElapsedTime => TestStatus.ElapsedTime;
        public TimeSpan ElapsedTimeAverage => new TimeSpan(TestStatus.ElapsedTime.Ticks / Math.Max(1, TestStatus.AnswerCountTotal));
        public DateTime DateTimeEnd => TestStatus.DateTimeNative;
        public DateTime DateTimeStart => TestStatus.DateTimeNative-ElapsedTime;
        public DateTime DateTimeEndLocal => TestStatus.DateTimeNative.ToLocalTime();
        public DateTime DateTimeStartLocal => (TestStatus.DateTimeNative - ElapsedTime).ToLocalTime();
        public int Seed => TestStatus.Seed;
        public QuizWordChoiceViewModel.RetryStatusEnum RetryStatus => TestStatus.RetryStatus;

        public QuizResultViewModel():this(new QuizWordChoiceViewModel())
        {
        }

        private TestResultItemViewModel[] items;
        public TestResultItemViewModel[] Items => items = items ?? QuizWordChoice.GetTestResults();

        public class TestResultItemViewModel:BaseViewModel
        {
            private IWordViewModel word;
            public IWordViewModel Word { get => word; set => SetProperty(ref word, value); }
            private QuizWordChoiceViewModel.TestResult result;
            public QuizWordChoiceViewModel.TestResult Result { get => result; set => SetProperty(ref result, value); }
        }
    }
}
