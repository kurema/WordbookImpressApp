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
        private enum TestResult
        {
            Correct,Wrong,Pass,Yet
        }

        private Word[] answerOrder;
        private Word[] AnswerOrder => answerOrder ?? (answerOrder = GetAnswerOrder());
        private Word[] GetAnswerOrder()
        {
            var rand = new Random(Seed);
            var remain = new List<Word>();
            foreach (var item in WordbookTarget.Words) remain.Add(item);
            int answer = rand.Next(WordbookTarget.Words.Length - 1);
            var result = new List<Word>();
            for (int i = 0; i < WordbookTarget.Words.Length; i++)
            {
                int t = rand.Next(remain.Count - 1);
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

                foreach (var wordbook in wordbooks)
                {
                    foreach (var item in wordbook.Words)
                    {
                        if (item.Hash == answerWord.Hash) shuffle = false;
                        else remain.Add(item);
                        if (shuffle) rand = new Random(rand.Next());
                    }
                }
                int answer = rand.Next(count - 1);
                for (int i = 0; i < count; i++)
                {
                    if (i == answer) {
                        var result= new ChoicesEnumerableItem() { Text = GetByChoiceKind(answerWord, choiceKind), Highlight = HighlightOn };
                        Answers.Add(result);
                        yield return result;
                    }
                    else
                    {
                        int target = rand.Next(remain.Count - 1);
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

        private Word CurrentWord { get => AnswerOrder[CurrentCount]; }
        private Record Record;
        private WordbookImpress WordbookTarget;
        private WordbookImpressViewModel wordbookTargetViewModel;
        public WordbookImpressViewModel WordbookTargetViewModel => wordbookTargetViewModel ?? (wordbookTargetViewModel = new WordbookImpressViewModel(WordbookTarget, Record));
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
                CurrentQuizStatus = QuizStatus.Choice;
            }
        }
        private DateTime DateTimeInitial;
        public void Start()
        {
            CurrentCount = 0;
            CurrentQuizStatus = QuizStatus.Choice;
            DateTimeInitial = DateTime.Now;
        }

        public void End()
        {
            int total = 0;
            int correct = 0;
            int pass = 0;
            for (int i = 0; i < WordbookTarget.Words.Length; i++)
            {
                var status = Record.GetWordStatusByHash(AnswerOrder[i].Hash);

                switch (TestResults[i]) {
                    case TestResult.Correct:
                        status.AnswerCountCorrect++;
                        status.AnswerCountTotal++;
                        correct++;
                        total++;
                        Record.SetWordStatusByHash(AnswerOrder[i].Hash, status);
                        break;
                    case TestResult.Wrong:
                        status.AnswerCountTotal++;
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

            Record.TestStatuses.Add(TestStatus = new Record.TestStatus() { ElapsedTime = DateTime.Now - DateTimeInitial, AnswerCountCorrect = correct, AnswerCountPass = pass, AnswerCountTotal = total, Key = WordbookTarget.Uri, Seed = this.Seed, DateTimeNative = DateTime.UtcNow, ChoiceKind = this.ChoiceType });
        }

        private Record.TestStatus testStatus;
        public Record.TestStatus TestStatus { get => testStatus; private set => SetProperty(ref testStatus, value); }

        private bool skipChecked=false;
        public bool SkipChecked { get => skipChecked; set => SetProperty(ref skipChecked, value); }

        private double skipMinRate=1.0;
        public double SkipMinRate { get => skipMinRate; set => SetProperty(ref skipMinRate, value); }

        private int skipMinCorrect=int.MaxValue;
        public int SkipMinCorrect { get => skipMinCorrect; set => SetProperty(ref skipMinCorrect, value); }

        public static bool GetSkipStatus(Word word,QuizWordChoiceViewModel model,Record record)
        {
            var info = record.GetWordStatusByHash(word.GetHash());
            if (model.SkipChecked && info.ExcludeRemembered)
            {
                return true;
            }
            if (info.AnswerCountTotal>0 &&  model.SkipMinRate<=((double)info.AnswerCountCorrect/ (double)info.AnswerCountTotal))
            {
                return true;
            }
            if (model.SkipMinCorrect!=-1 && model.SkipMinCorrect<=info.AnswerCountCorrect)
            {
                return true;
            }
            return false;
        }

        public int GetNextQuizCount()
        {
            int count = CurrentCount + 1;
            while (GetSkipStatus(AnswerOrder[count], this, Record))
            {
                if (count < 0 || count >= AnswerOrder.Length)
                {
                    return -1;
                }
                count++;
            }
            return count;
        }

        public int GetPreviousQuizCount()
        {
            int count = CurrentCount - 1;
            while (GetSkipStatus(AnswerOrder[count], this, Record))
            {
                if (count < 0 || count >= AnswerOrder.Length)
                {
                    return -1;
                }
                count--;
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

        public QuizWordChoiceViewModel():this(new WordbookImpressViewModel(),new ConfigViewModel())
        { }

        public QuizWordChoiceViewModel(WordbookImpressViewModel model, ConfigViewModel config, ChoiceKind choiceKind = ChoiceKind.Description, int choiceCount = 4) : this(choiceCount, model.Record, choiceKind, model.Wordbook, new WordbookImpress[] { model.Wordbook },config)
        {
        }

        public QuizWordChoiceViewModel(int choiceCount, Record Record,ChoiceKind choiceKind, WordbookImpress WordbookTarget, WordbookImpress[] WordbooksForChoice, ConfigViewModel config)
        {
            Seed = new Random().Next();
            this.choiceCount = choiceCount;
            this.Record = Record;
            this.WordbookTarget = WordbookTarget;
            this.WordbooksForChoice = WordbooksForChoice;
            this.ChoiceType = choiceKind;

            TestResults = new TestResult[WordbookTarget.Words.Length];
            for(int i=0;i< WordbookTarget.Words.Length; i++)
            {
                TestResults[i] = TestResult.Yet;
            }

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

        public void ApplyConfig(ConfigViewModel config)
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
