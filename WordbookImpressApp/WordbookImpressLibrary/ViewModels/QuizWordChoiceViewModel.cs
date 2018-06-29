using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;

namespace WordbookImpressLibrary.ViewModels
{
    public class QuizWordChoiceViewModel : BaseViewModel
    {
        public int Seed { get; set; }

        private ChoicesEnumerable ChoicesLast;
        public ChoicesEnumerable Choices
        {
            get
            {
                return ChoicesLast = new ChoicesEnumerable(WordbooksForChoice, Seed, ChoiceType, ChoiceCount, CurrentWord) { HighlightOn = CurrentQuizStatus == QuizStatus.Wrong };
            }
        }

        private TestResult[] TestResults;
        public enum TestResult
        {
            Correct,Wrong,Pass,Yet
        }

        public QuizResultViewModel.TestResultItemViewModel[] GetTestResults()
        {
            var list = new List<QuizResultViewModel.TestResultItemViewModel>();
            for(int i = 0; i < Math.Min( TestResults.Length,AnswerOrder.Length); i++)
            {
                list.Add(new QuizResultViewModel.TestResultItemViewModel() { Result = TestResults[i], Word = new WordViewModel(AnswerOrder[i], Record) });
            }
            return list.ToArray();
        }

        private Word[] answerOrder;
        private Word[] AnswerOrder => answerOrder ?? (answerOrder = GetAnswerOrder());
        private Word[] GetAnswerOrder()
        {
            var result = new List<Word>();
            var rand = new Random(Seed);
            var remain = new List<Word>();
            foreach (var w in WordbooksTarget)
            {
                if (w.Words.Length == 0) { continue; }
                remain.AddRange(w.Words);
            }
            while (remain.Count > 0)
            {
                int t = rand.Next(remain.Count);
                result.Add(remain[t]);
                remain.RemoveAt(t);
            }
            return result.ToArray();
        }

        public class ChoicesEnumerable : IEnumerable<ChoicesEnumerable.ChoicesEnumerableItem>, System.Collections.Specialized.INotifyCollectionChanged
        {
            private WordbookImpress[] wordbooks;
            private int seed;
            private ChoiceKind choiceKind;
            private int count;
            private Word answerWord;

            private bool highlightOn=false;
            public bool HighlightOn
            {
                get => highlightOn; set
                {
                    highlightOn = value;
                    //this.CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                    foreach(var item in Answers)
                    {
                        item.Highlight = true;
                    }
                }
            }


            public ChoicesEnumerable(WordbookImpress[] wordbook, int seed, ChoiceKind choiceKind, int count, Word answerWord)
            {
                this.wordbooks = wordbook;
                this.seed = seed;
                this.choiceKind = choiceKind;
                if (count <= 0) throw new ArgumentOutOfRangeException();
                this.count = count;
                this.answerWord = answerWord;
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;
            private void OnCollectionChanged(NotifyCollectionChangedAction action)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
            }

            private List<ChoicesEnumerableItem> Answers=new List<ChoicesEnumerableItem>();

            public IEnumerator<ChoicesEnumerable.ChoicesEnumerableItem> GetEnumerator()
            {
                Answers = new List<ChoicesEnumerableItem>();
                var rand = new Random(this.seed);
                rand = new Random(rand.Next());
                bool shuffle = true;
                var remain = new List<Word>();

                if (wordbooks == null || wordbooks.Length == 0) yield break;

                foreach (var wordbook in wordbooks)
                {
                    foreach (var item in wordbook?.Words)
                    {
                        if (item.Hash == answerWord.Hash) shuffle = false;
                        else remain.Add(item);
                        if (shuffle) rand = new Random(rand.Next());
                    }
                }
                int answer = rand.Next(Math.Min(count, remain.Count+1));
                for (int i = 0; i < count; i++)
                {
                    if (i == answer) {
                        var result= new ChoicesEnumerableItem() { Text = GetByChoiceKind(answerWord, choiceKind), Highlight = HighlightOn };
                        Answers.Add(result);
                        yield return result;
                    }
                    else
                    {
                        if (remain.Count == 0) yield break;
                        int target = rand.Next(remain.Count);
                        yield return new ChoicesEnumerableItem() { Text = GetByChoiceKind(remain[target], choiceKind) ,Highlight=false};
                        remain.RemoveAt(target);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public static string GetByChoiceKind(Word word, ChoiceKind choiceKind)
            {
                return WordTypeDictionary(word)[choiceKind];
            }

            public class ChoicesEnumerableItem:BaseViewModel
            {
                private string text="";
                public string Text { get => text; set => SetProperty(ref text, value); }

                private bool highlight=false;
                public bool Highlight { get => highlight; set => SetProperty(ref highlight, value); }
            }
        }

        public double Progress => (currentCount + 1.0) / Math.Max(1, AnswerOrder.Length);

        private Word CurrentWord { get => AnswerOrder.Length == 0 ? new Word() : AnswerOrder[CurrentCount]; }
        private Record Record;
        private WordbookImpress[] WordbooksTarget;
        private WordbookImpressViewModel wordbookTargetViewModel;
        public WordbookImpressViewModel WordbookTargetViewModel => wordbookTargetViewModel ?? (WordbooksTarget.Length==1? wordbookTargetViewModel = new WordbookImpressViewModel(WordbooksTarget[0], Record): wordbookTargetViewModel = new WordbookImpressViewModel(WordbooksTarget, Record,"総合単語帳"));
        private WordbookImpress[] WordbooksForChoice;
        private int currentCount=0;
        private int CurrentCount
        {
            get => currentCount;
            set
            {
                SetProperty(ref currentCount, value);
                OnPropertyChanged(nameof(this.CurrentWord));
                OnPropertyChanged(nameof(this.CurrentWordText));
                OnPropertyChanged(nameof(this.Choices));
                OnPropertyChanged(nameof(this.Progress));
                CurrentQuizStatus = QuizStatus.Choice;
            }
        }
        private DateTime DateTimeInitial;
        public void Start()
        {
            CurrentCount = -1;
            var count = GetNextQuizCount();
            CurrentCount = count == -1 ? 0 : count;
            CurrentQuizStatus = QuizStatus.Choice;
            DateTimeInitial = DateTime.Now;
            //RetryStatus = RetryStatusEnum.First;
        }

        private TimeSpan ElapsedTime = new TimeSpan();

        public void Continue()
        {
            RetryStatus = RetryStatusEnum.Continue;
            DateTimeInitial = DateTime.Now;
        }

        private RetryStatusEnum retryStatus=RetryStatusEnum.First;
        public RetryStatusEnum RetryStatus { get => retryStatus; set => SetProperty(ref retryStatus, value); }

        public enum RetryStatusEnum
        {
            First,Retry,Continue
        }

        public void End()
        {
            int total = 0;
            int correct = 0;
            int pass = 0;
            foreach (var wb in WordbooksTarget)
            {
                for (int i = 0; i < wb.Words.Length; i++)
                {
                    var status = Record.GetWordStatusByHash(AnswerOrder[i].Hash);

                    switch (TestResults[i])
                    {
                        case TestResult.Correct:
                            status.AnswerCountCorrect++;
                            status.AnswerCountTotal++;
                            status.LastCorrectDateTime = DateTime.Now;
                            status.LastAnswerDateTime = DateTime.Now;
                            correct++;
                            total++;
                            Record.SetWordStatusByHash(AnswerOrder[i].Hash, status);
                            break;
                        case TestResult.Wrong:
                            status.AnswerCountTotal++;
                            status.LastCorrectDateTime = DateTime.Now;
                            total++;
                            Record.SetWordStatusByHash(AnswerOrder[i].Hash, status);
                            break;
                        case TestResult.Pass:
                            pass++;
                            status.AnswerCountPass++;
                            Record.SetWordStatusByHash(AnswerOrder[i].Hash, status);
                            break;
                        case TestResult.Yet:
                            break;
                    }
                }
            }

            ElapsedTime += DateTime.Now - DateTimeInitial;
            Record.TestStatuses.Add(TestStatus = new Record.TestStatus() { RetryStatus=this.retryStatus, ElapsedTime =this.ElapsedTime, AnswerCountCorrect = correct, AnswerCountPass = pass, AnswerCountTotal = total, Key = WordbooksTarget.Length==1? WordbooksTarget[0].Uri:"[combined]", Seed = this.Seed, DateTimeNative = DateTime.UtcNow, ChoiceKind = this.ChoiceType });
        }

        private Record.TestStatus testStatus;
        public Record.TestStatus TestStatus { get => testStatus; private set => SetProperty(ref testStatus, value); }

        private bool skipChecked=false;
        public bool SkipChecked { get => skipChecked; set => SetProperty(ref skipChecked, value); }

        private double skipMinRate=1.0;
        public double SkipMinRate { get => skipMinRate; set => SetProperty(ref skipMinRate, value); }

        private int skipMinCorrect=int.MaxValue;
        public int SkipMinCorrect { get => skipMinCorrect; set => SetProperty(ref skipMinCorrect, value); }

        private int skipMinRateMinTotal = 10;
        public int SkipMinRateMinTotal { get => skipMinRateMinTotal; set => SetProperty(ref skipMinRateMinTotal, value); }

        private TimeSpan skipVoidTimeSpan = new TimeSpan(-1);
        public TimeSpan SkipVoidTimeSpan { get => skipVoidTimeSpan; set => SetProperty(ref skipVoidTimeSpan, value); }

        public static bool GetSkipStatus(Word word, QuizWordChoiceViewModel model, Record record)
        {
            var info = record.GetWordStatusByHash(word.GetHash());

            if(model.skipVoidTimeSpan.Ticks>0 && (info.LastCorrectDateTime+model.SkipVoidTimeSpan > DateTime.Now)) { return false; }
            if (model.SkipChecked && info.ExcludeRemembered)
            {
                return true;
            }
            if (info.AnswerCountTotal > 0 && (model.SkipMinRateMinTotal <= info.AnswerCountTotal && model.SkipMinRate <= ((double)info.AnswerCountCorrect / (double)info.AnswerCountTotal)))
            {
                return true;
            }
            if (model.SkipMinCorrect != -1 && model.SkipMinCorrect <= info.AnswerCountCorrect)
            {
                return true;
            }
            return false;
        }

        public int GetNextQuizCount()
        {
            int count = CurrentCount + 1;
            if (count < 0 || count >= AnswerOrder.Length)
            {
                return -1;
            }
            while (GetSkipStatus(AnswerOrder[count], this, Record))
            {
                count++;
                if (count < 0 || count >= AnswerOrder.Length)
                {
                    return -1;
                }
            }
            return count;
        }

        public int GetPreviousQuizCount()
        {
            int count = CurrentCount - 1;
            if (count < 0 || count >= AnswerOrder.Length)
            {
                return -1;
            }
            while (GetSkipStatus(AnswerOrder[count], this, Record))
            {
                count--;
                if (count < 0 || count >= AnswerOrder.Length)
                {
                    return -1;
                }
            }
            return count;
        }


        private System.Windows.Input.ICommand nextQuizCommand;
        public System.Windows.Input.ICommand NextQuizCommand
        {
            get
            {
                if (nextQuizCommand != null) return nextQuizCommand;
                var command = new Helper.DelegateCommand(
                    (a) => GetNextQuizCount() != -1, (s) => CurrentCount = Math.Max(0,GetNextQuizCount()));
                this.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(CurrentWord)) command?.OnCanExecuteChanged();
                };
                nextQuizCommand = command;
                return command;
            }
        }

        private System.Windows.Input.ICommand previousQuizCommand;
        public System.Windows.Input.ICommand PreviousQuizCommand {
            get
            {
                if (previousQuizCommand != null) return previousQuizCommand;
                var command = new Helper.DelegateCommand(
                    (a) => GetPreviousQuizCount() != -1, (s) => CurrentCount = Math.Max(0, GetPreviousQuizCount()));
                this.PropertyChanged += (s, e) =>
                  {
                      if (e.PropertyName == nameof(CurrentWord)) command?.OnCanExecuteChanged();
                  };
                previousQuizCommand = command;
                return command;
            }
        }

        public string CurrentWordText { get
            {
                switch (ChoiceType)
                {
                    case ChoiceKind.Title:return CurrentWord.Description;
                    case ChoiceKind.Description:return CurrentWord.Title;
                    default:return "";
                }
            }
        }
        private ChoiceKind choiceType=ChoiceKind.Description;
        public ChoiceKind ChoiceType
        {
            get => choiceType;
            set
            {
                SetProperty(ref choiceType, value);
                OnPropertyChanged(nameof(Choices));
            }
        }

        public enum ChoiceKind
        {
            Title,Description
        }

        public QuizWordChoiceViewModel():this(new WordbookImpressViewModel())
        {
        }

        public QuizWordChoiceViewModel(WordbookImpressViewModel model, ChoiceKind choiceKind = ChoiceKind.Description) : this( model.Record, choiceKind, model.Wordbooks, model.Wordbooks,Storage.ConfigStorage.Content)
        {
        }

        public QuizWordChoiceViewModel(WordbookImpressViewModel model, int Seed, ChoiceKind choiceKind = ChoiceKind.Description) : this(model.Record, choiceKind, model.Wordbooks, model.Wordbooks, Storage.ConfigStorage.Content, Seed)
        {
        }

        public QuizWordChoiceViewModel(Record Record, ChoiceKind choiceKind, WordbookImpress[] WordbookTarget, WordbookImpress[] WordbooksForChoice, Config config)
            :this(Record,choiceKind,WordbookTarget,WordbooksForChoice,config,new Random().Next()) { }

        public QuizWordChoiceViewModel(Record Record, ChoiceKind choiceKind, WordbookImpress[] WordbookTarget, WordbookImpress[] WordbooksForChoice, Config config, int Seed)
        {
            this.Seed = Seed;
            this.Record = Record;
            this.WordbooksTarget = WordbookTarget;
            this.WordbooksForChoice = WordbooksForChoice;
            this.ChoiceType = choiceKind;

            var tempResults = new List<TestResult>();
            foreach (var w in WordbooksTarget)
            {
                for (int i = 0; i < w.Words.Length; i++)
                {
                    tempResults.Add(TestResult.Yet);
                }
            }
            TestResults= tempResults.ToArray();
            this.ApplyConfig(config);
        }

        private int choiceCount;
        public int ChoiceCount
        {
            get => choiceCount;
            set {
                SetProperty(ref choiceCount, value);
                OnPropertyChanged(nameof(Choices));
            }
        }

        private QuizStatus currentQuizStatus = QuizStatus.Choice;
        public QuizStatus CurrentQuizStatus
        {
            get => currentQuizStatus;
            set
            {
                SetProperty(ref currentQuizStatus, value);
                //OnPropertyChanged(nameof(Choices));
                if (value == QuizStatus.Wrong) ChoicesLast.HighlightOn = true;
                else ChoicesLast.HighlightOn = false;
            }
        }

        public enum QuizStatus
        {
            Choice,Correct,Wrong
        }

        private static Dictionary<ChoiceKind, string> WordTypeDictionary(Word w)
        {
            return new Dictionary<ChoiceKind, string>()
            {
                {ChoiceKind.Title,w.Title},
                {ChoiceKind.Description,w.Description }
            };
        }

        public void ApplyConfig(Config config)
        {
            this.SkipChecked = config.SkipChecked;
            this.SkipMinCorrect = config.SkipMinCorrect;
            this.SkipMinRate = config.SkipMinRate;
            this.ChoiceCount = config.ChoiceCount;
        }

        public bool Choose(string choice)
        {
            var dic = WordTypeDictionary(CurrentWord);
            if (choice == dic[ChoiceType])
            {
                //var status = Record.GetWordStatusByHash(CurrentWord.Hash);
                //status.AnswerCountCorrect++;
                //status.AnswerCountTotal++;
                //Record.SetWordStatusByHash(CurrentWord.Hash, status);

                TestResults[CurrentCount] = TestResult.Correct;
                CurrentQuizStatus = QuizStatus.Correct;
                return true;
            }
            else
            {
                //var status = Record.GetWordStatusByHash(CurrentWord.Hash);
                //status.AnswerCountTotal++;
                //Record.SetWordStatusByHash(CurrentWord.Hash, status);

                TestResults[CurrentCount] = TestResult.Wrong;
                CurrentQuizStatus = QuizStatus.Wrong;
                return false;
            }
        }
    }
}
