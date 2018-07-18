using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;

using System.Linq;

namespace WordbookImpressLibrary.ViewModels
{
    public class QuizWordChoiceViewModel : BaseViewModel
    {
        public int Seed { get; set; }

        public string Description => currentModeWord ? "" : CurrentQuizChoice.Description;
        public bool DescriptionDisplay => this.CurrentQuizStatus != QuizStatus.Choice && !String.IsNullOrWhiteSpace(Description);

        private ChoicesEnumerable ChoicesLast;
        public ChoicesEnumerable Choices
        {
            get
            {
                return ChoicesLast = 
                    CurrentModeWord?
                    new ChoicesEnumerable(WordbooksForChoice, Seed, ChoiceType, ChoiceCount, CurrentWord) { HighlightOn = CurrentQuizStatus == QuizStatus.Wrong }:
                    new ChoicesEnumerable(CurrentQuizChoice,CurrentQuizChoiceCount) { HighlightOn = CurrentQuizStatus == QuizStatus.Wrong }
                    ;
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
            for (int i = 0; i < Math.Min(TestResults.Length, QuizOrder.Length); i++)
            {
                list.Add(new QuizResultViewModel.TestResultItemViewModel() { Result = TestResults[i+AnswerOrder.Length], Word = new QuizChoiceViewModel(QuizOrder[i], Record) });
            }
            return list.ToArray();
        }

        private QuizChoice[] quizOrder;
        private QuizChoice[] QuizOrder => quizOrder = quizOrder ?? GetQuizOrder();
        private QuizChoice[] GetQuizOrder()
        {
            var rand = new Random(unchecked(Seed + 1));
            var result = new List<QuizChoice>();
            var remain = new List<QuizChoice>();
            foreach (var w in WordbooksTarget)
            {
                if (w.QuizChoices.Length == 0) { continue; }
                remain.AddRange(w.QuizChoices);
            }
            while (remain.Count > 0)
            {
                int t = rand.Next(remain.Count);
                result.Add(remain[t]);
                remain.RemoveAt(t);
            }
            return result.ToArray();
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

        public class ChoicesEnumerable : IEnumerable<ChoicesEnumerable.ChoicesEnumerableItem>, INotifyCollectionChanged
        {
            private IWordbook[] wordbooks;
            private int seed;
            private ChoiceKind choiceKind;
            private int count;
            private Word answerWord;
            private QuizChoice quizChoice = null;
            private int shuffleCount = 0;

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


            public ChoicesEnumerable(IWordbook[] wordbook, int seed, ChoiceKind choiceKind, int count, Word answerWord)
            {
                this.wordbooks = wordbook;
                this.seed = seed;
                this.choiceKind = choiceKind;
                if (count <= 0) throw new ArgumentOutOfRangeException();
                this.count = count;
                this.answerWord = answerWord;
            }

            public ChoicesEnumerable(QuizChoice quizChoice,int shuffleCount)
            {
                this.quizChoice = quizChoice;
                this.shuffleCount = shuffleCount;
            }

            public event NotifyCollectionChangedEventHandler CollectionChanged;
            private void OnCollectionChanged(NotifyCollectionChangedAction action)
            {
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
            }

            private List<ChoicesEnumerableItem> Answers=new List<ChoicesEnumerableItem>();

            public IEnumerator<ChoicesEnumerable.ChoicesEnumerableItem> GetEnumerator()
            {
                if (quizChoice != null)
                {
                    Answers = new List<ChoicesEnumerableItem>();
                    var remain = new List<string>(quizChoice.Choices);
                    var rand = new Random(unchecked(this.seed + 3));
                    for (var i = 0; i < shuffleCount; i++) rand = new Random(rand.Next());

                    for (int i = 0; i < quizChoice.Choices.Length; i++)
                    {
                        if (remain.Count == 0) yield break;
                        int target = rand.Next(remain.Count);
                        var result= new ChoicesEnumerableItem() { Text = remain[target], Highlight = HighlightOn && remain[target] == quizChoice.Answer };
                        if (remain[target] == quizChoice.Answer) Answers.Add(result);
                        yield return result;
                        remain.RemoveAt(target);
                    }
                }
                else
                {
                    Answers = new List<ChoicesEnumerableItem>();
                    var rand = new Random(unchecked(this.seed + 2));
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
                    int answer = rand.Next(Math.Min(count, remain.Count + 1));
                    for (int i = 0; i < count; i++)
                    {
                        if (i == answer)
                        {
                            var result = new ChoicesEnumerableItem() { Text = GetByChoiceKind(answerWord, choiceKind), Highlight = HighlightOn };
                            Answers.Add(result);
                            yield return result;
                        }
                        else
                        {
                            if (remain.Count == 0) yield break;
                            int target = rand.Next(remain.Count);
                            yield return new ChoicesEnumerableItem() { Text = GetByChoiceKind(remain[target], choiceKind), Highlight = false };
                            remain.RemoveAt(target);
                        }
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

        public double Progress => CurrentModeWord ? (currentWordCount + 1.0) / Math.Max(1, AnswerOrder.Length + QuizOrder.Length) : (CurrentQuizChoiceCount + AnswerOrder.Length + 1.0) / Math.Max(1, AnswerOrder.Length + QuizOrder.Length);
        public string AnswerText => CurrentModeWord ? CurrentWord.Title : CurrentQuizChoice.Answer;

        private Word CurrentWord { get => AnswerOrder.Length == 0 || CurrentWordCount>=AnswerOrder.Length ? new Word() : AnswerOrder[CurrentWordCount]; }
        private QuizChoice CurrentQuizChoice { get => QuizOrder.Length == 0 || currentQuizChoiceCount >= QuizOrder.Length ? new QuizChoice() : QuizOrder[CurrentQuizChoiceCount]; }
        private Record Record;
        private IWordbook[] WordbooksTarget;
        private WordbookImpressViewModel wordbookTargetViewModel;
        public WordbookImpressViewModel WordbookTargetViewModel => wordbookTargetViewModel ?? (WordbooksTarget.Length==1? wordbookTargetViewModel = new WordbookImpressViewModel(WordbooksTarget[0], Record): wordbookTargetViewModel = new WordbookImpressViewModel(WordbooksTarget, Record,"総合単語帳"));
        private IWordbook[] WordbooksForChoice;
        private bool currentModeWord = true;
        private bool CurrentModeWord { get => AnswerOrder.Length == 0 ? false : currentModeWord; set
            {
                if (AnswerOrder.Length > 0 && value == true) { currentModeWord = true; }
                else if (QuizOrder.Length > 0 && value == false) { currentModeWord = false; }
            }
        }
        private int CurrentWordTotal => currentModeWord ? currentWordCount : AnswerOrder.Length + CurrentQuizChoiceCount;
        private int currentWordCount=0;
        private int CurrentWordCount
        {
            get => currentWordCount;
            set
            {
                currentModeWord = true;
                currentQuizChoiceCount = 0;
                SetProperty(ref currentWordCount, value);
                OnPropertyChanged(nameof(this.CurrentQuizChoice));
                OnPropertyChanged(nameof(this.CurrentWord));
                OnPropertyChanged(nameof(this.CurrentWordText));
                OnPropertyChanged(nameof(this.Choices));
                OnPropertyChanged(nameof(this.Progress));
                OnPropertyChanged(nameof(DescriptionDisplay));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(AnswerText));
                CurrentQuizStatus = QuizStatus.Choice;
            }
        }
        private int currentQuizChoiceCount = 0;
        private int CurrentQuizChoiceCount
        {
            get => currentQuizChoiceCount;
            set
            {
                currentModeWord = false;
                currentWordCount = 0;
                SetProperty(ref currentQuizChoiceCount, value);
                OnPropertyChanged(nameof(this.CurrentQuizChoice));
                OnPropertyChanged(nameof(this.CurrentWord));
                OnPropertyChanged(nameof(this.CurrentWordText));
                OnPropertyChanged(nameof(this.Choices));
                OnPropertyChanged(nameof(this.Progress));
                OnPropertyChanged(nameof(DescriptionDisplay));
                OnPropertyChanged(nameof(Description));
                OnPropertyChanged(nameof(AnswerText));
                CurrentQuizStatus = QuizStatus.Choice;
            }
        }
        private DateTime DateTimeInitial;
        public void Start()
        {
            currentWordCount = -1;
            currentQuizChoiceCount = -1;
            var countW = GetQuizCountNext();
            var countC = GetNextQuizChoiceCount();
            if (countW == -1 && countC == -1)
            {
                if (AnswerOrder.Length > 0)
                {
                    CurrentModeWord = true;
                    CurrentWordCount = 0;
                }
                else if (QuizOrder.Length > 0)
                {
                    CurrentModeWord = false;
                    CurrentQuizChoiceCount = 0;
                }
            }
            else if (countW == -1)
            {
                CurrentModeWord = false;
                CurrentQuizChoiceCount = countC;
            }
            else
            {
                CurrentModeWord = true;
                CurrentWordCount = countW;
            }
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
            for (int i = 0; i < AnswerOrder.Length; i++)
            {
                var status = Record.GetWordStatusByHash(AnswerOrder[i].Hash);
                EndSwitch(TestResults[i], AnswerOrder[i].Hash, ref correct, ref total, ref pass);
            }
            for (int i = 0; i < QuizOrder.Length; i++)
            {
                EndSwitch(TestResults[i + AnswerOrder.Length], QuizOrder[i].Hash, ref correct, ref total, ref pass);
            }

            ElapsedTime += DateTime.Now - DateTimeInitial;
            Record.TestStatuses.Add(TestStatus = new Record.TestStatus()
            {
                RetryStatus = this.retryStatus,
                ElapsedTime = this.ElapsedTime,
                AnswerCountCorrect = correct,
                AnswerCountPass = pass,
                AnswerCountTotal = total,
                Key = WordbooksTarget.Length == 1 ? WordbooksTarget[0].Id : WordbooksTarget.Length==Storage.WordbooksImpressStorage.Content.Count? Record.TestStatus.KeyAll: Record.TestStatus.KeyCombined,
                Seed = this.Seed,
                DateTimeNative = DateTime.UtcNow,
                ChoiceKind = this.ChoiceType
            });
        }

        private void EndSwitch(TestResult testResult,string hash, ref int correct,ref int total,ref int pass)
        {
            var status = Record.GetWordStatusByHash(hash);
            switch (testResult)
            {
                case TestResult.Correct:
                    status.AnswerCountCorrect++;
                    status.AnswerCountTotal++;
                    status.LastCorrectDateTime = DateTime.Now;
                    status.LastAnswerDateTime = DateTime.Now;
                    correct++;
                    total++;
                    Record.SetWordStatusByHash(hash, status);
                    break;
                case TestResult.Wrong:
                    status.AnswerCountTotal++;
                    status.LastCorrectDateTime = DateTime.Now;
                    total++;
                    Record.SetWordStatusByHash(hash, status);
                    break;
                case TestResult.Pass:
                    pass++;
                    status.AnswerCountPass++;
                    Record.SetWordStatusByHash(hash, status);
                    break;
                case TestResult.Yet:
                    break;
            }

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
            return GetSkipStatus(record.GetWordStatusByHash(word.GetHash()), model); ;
        }

        public static bool GetSkipStatus(QuizChoice word, QuizWordChoiceViewModel model, Record record)
        {
            return GetSkipStatus(record.GetWordStatusByHash(word.GetHash()), model); ;
        }

        public static bool GetSkipStatus(Record.WordStatus info, QuizWordChoiceViewModel model)
        {
            if (model.skipVoidTimeSpan.Ticks > 0 && (info.LastCorrectDateTime + model.SkipVoidTimeSpan > DateTime.Now)) { return false; }
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

        public int GetQuizCountNext()
        {
            return GetQuizCountBase((a) => a + 1);
        }

        public int GetQuizCountPrevious()
        {
            return GetQuizCountBase((a) => a - 1);
        }

        public int GetQuizCountBase(Func<int,int> func)
        {
            int count = func(CurrentWordCount);
            if (count < 0 || count >= AnswerOrder.Length)
            {
                return -1;
            }
            while (GetSkipStatus(AnswerOrder[count], this, Record))
            {
                count= func(count);
                if (count < 0 || count >= AnswerOrder.Length)
                {
                    return -1;
                }
            }
            return count;
        }

        public int GetNextQuizChoiceCount()
        {
            return GetQuizChoiceCountBase((a) => a + 1);
        }

        public int GetQuizChoiceCountPrevious()
        {
            return GetQuizChoiceCountBase((a) => a - 1);
        }

        public int GetQuizChoiceCountBase(Func<int, int> func)
        {
            int count = func(currentQuizChoiceCount);
            if (count < 0 || count >= QuizOrder.Length)
            {
                return -1;
            }
            while (GetSkipStatus(QuizOrder[count], this, Record))
            {
                count = func(count);
                if (count < 0 || count >= QuizOrder.Length)
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
                    (a) => (CurrentModeWord && GetQuizCountNext() != -1) || GetNextQuizChoiceCount() != -1, (s) => {
                        if (CurrentModeWord)
                        {
                            var w = GetQuizCountNext();
                            if (w != -1)
                            {
                                CurrentWordCount = w;
                                return;
                            }
                        }
                        var q = GetNextQuizChoiceCount();
                        if (q != -1)
                        {
                            CurrentQuizChoiceCount = q;
                        }
                    });
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
                    (a) => (CurrentModeWord && GetQuizCountPrevious() != -1) || GetQuizCountPrevious() != -1, (s) => CurrentWordCount = Math.Max(0, GetQuizCountPrevious()));
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
                if (CurrentModeWord)
                {
                    switch (ChoiceType)
                    {
                        case ChoiceKind.Title: return CurrentWord.Description;
                        case ChoiceKind.Description: return CurrentWord.Title;
                        default: return "";
                    }
                }
                else
                {
                    return CurrentQuizChoice.Title;
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
            Title,Description,Combined
        }

        public QuizWordChoiceViewModel(Record.TestStatus testStatus):this(testStatus, Storage.RecordStorage.Content, Storage.WordbooksImpressStorage.Content)
        { }

        public QuizWordChoiceViewModel(Record.TestStatus testStatus, Record Record,IEnumerable<IWordbook> wordbooks)
        {
            this.TestStatus = testStatus;

            this.Seed = testStatus.Seed;
            this.Record = Record;
            if (testStatus.Key == Record.TestStatus.KeyAll)
            {
                this.WordbooksForChoice = this.WordbooksTarget = wordbooks.ToArray();
            }
            else if (testStatus.Key == Record.TestStatus.KeyCombined)
            {
                this.WordbooksForChoice = this.WordbooksTarget = wordbooks.ToArray();
            }
            else
            {
                this.WordbooksForChoice = this.WordbooksTarget = new[] { wordbooks.First((w) => w.Id == testStatus.Key) };
            }
            if (this.WordbooksTarget == null)
            {
                this.WordbooksForChoice = this.WordbooksTarget = new WordbookImpress[0];
            }
            this.choiceType = testStatus.ChoiceKind;

            TestResults = GetInitialTestResults(WordbooksTarget);
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

        public QuizWordChoiceViewModel(Record Record, ChoiceKind choiceKind, IWordbook[] WordbookTarget, IWordbook[] WordbooksForChoice, Config config)
            :this(Record,choiceKind,WordbookTarget,WordbooksForChoice,config,new Random().Next()) { }

        public QuizWordChoiceViewModel(Record Record, ChoiceKind choiceKind, IWordbook[] WordbookTarget, IWordbook[] WordbooksForChoice, Config config, int Seed)
        {
            this.Seed = Seed;
            this.Record = Record;
            this.WordbooksTarget = WordbookTarget;
            this.WordbooksForChoice = WordbooksForChoice;
            this.ChoiceType = choiceKind;

            TestResults = GetInitialTestResults(WordbooksTarget);

            this.ApplyConfig(config);
        }

        private static TestResult[] GetInitialTestResults(IWordbook[] WordbooksTarget)
        {
            var tempResults = new List<TestResult>();
            foreach (var w in WordbooksTarget)
            {
                for (int i = 0; i < w.Words.Length; i++)
                {
                    tempResults.Add(TestResult.Yet);
                }
                for (int i = 0; i < w.QuizChoices.Length; i++)
                {
                    tempResults.Add(TestResult.Yet);
                }
            }
            return tempResults.ToArray();
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
                OnPropertyChanged(nameof(DescriptionDisplay));
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
            var answer = currentModeWord? WordTypeDictionary(CurrentWord)[ChoiceType]:CurrentQuizChoice.Answer;
            if (choice == answer)
            {
                //var status = Record.GetWordStatusByHash(CurrentWord.Hash);
                //status.AnswerCountCorrect++;
                //status.AnswerCountTotal++;
                //Record.SetWordStatusByHash(CurrentWord.Hash, status);

                TestResults[CurrentWordTotal] = TestResult.Correct;
                CurrentQuizStatus = QuizStatus.Correct;
                return true;
            }
            else
            {
                //var status = Record.GetWordStatusByHash(CurrentWord.Hash);
                //status.AnswerCountTotal++;
                //Record.SetWordStatusByHash(CurrentWord.Hash, status);

                TestResults[CurrentWordTotal] = TestResult.Wrong;
                CurrentQuizStatus = QuizStatus.Wrong;
                return false;
            }
        }
    }
}
