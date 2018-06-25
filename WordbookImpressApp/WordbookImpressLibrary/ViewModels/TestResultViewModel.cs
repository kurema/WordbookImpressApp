using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;


namespace WordbookImpressLibrary.ViewModels
{
    public class TestResultViewModel : BaseViewModel
    {
        private Record.TestStatus TestStatus => QuizWordChoice.TestStatus;
        public WordbookImpressViewModel Wordbook => QuizWordChoice.WordbookTargetViewModel;
        private QuizWordChoiceViewModel QuizWordChoice { get; set; }

        public TestResultViewModel(QuizWordChoiceViewModel quiz) { QuizWordChoice = quiz; }

        public double AnswerCorrectPercentage => TestStatus.AnswerCountTotal == 0 ? 0 : (double)TestStatus.AnswerCountCorrect / TestStatus.AnswerCountTotal * 100.0;

        public ViewModels.QuizWordChoiceViewModel.ChoiceKind ChoiceKind => TestStatus.ChoiceKind;
        public int AnswerCountTotal => TestStatus.AnswerCountTotal;
        public int AnswerCountCorrect => TestStatus.AnswerCountCorrect;
        public int AnswerCountPass => TestStatus.AnswerCountPass;
        public TimeSpan ElapsedTime => TestStatus.ElapsedTime;
        public TimeSpan ElapsedTimeAverage => new TimeSpan(TestStatus.ElapsedTime.Ticks / Math.Max(1, TestStatus.AnswerCountTotal));
        public DateTime DateTimeEnd => TestStatus.DateTimeNative;
        public DateTime DateTimeStart => TestStatus.DateTimeNative-ElapsedTime;

        public TestResultViewModel():this(new QuizWordChoiceViewModel())
        {
        }

        private TestResultItemViewModel[] items;
        public TestResultItemViewModel[] Items => items = items ?? QuizWordChoice.GetTestResults();

        public class TestResultItemViewModel:BaseViewModel
        {
            private WordViewModel word;
            public WordViewModel Word { get => word; set => SetProperty(ref word, value); }
            private QuizWordChoiceViewModel.TestResult result;
            public QuizWordChoiceViewModel.TestResult Result { get => result; set => SetProperty(ref result, value); }
        }
    }
}
