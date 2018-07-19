using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

using System.Linq;

namespace WordbookImpressLibrary.ViewModels
{
    public class WordbookImpressViewModel : BaseViewModel
    {
        private string WordbooksTitle { get; set; } = "";
        private IWordbook[] wordbooks;
        public IWordbook[] Wordbooks { get => wordbooks ?? new IWordbook[] { wordbook }; }
        private IWordbook wordbook;
        public bool HasMultipleWordbook { get => wordbooks != null; }
        public bool HasWords
        {
            get {
                foreach(var item in Wordbooks)
                {
                    if (item.Words.Length > 0) return true;
                }
                return false;
            }
        }

        public String Uri => !CurrentTargetWordbookImpress ? "" : ((WordbookImpress)wordbook)?.Uri;
        public String UriLogo => Storage.ImageCacheStorage.GetImageUrl(!CurrentTargetWordbookImpress ? "" : ((WordbookImpress)wordbook)?.UriLogo)??"tango.jpg";
        public String WordbookTitle => String.IsNullOrEmpty(WordbookTitleUser) ? WordbookTitleHtml : WordbookTitleUser;
        public String WordbookTitleUser { get => HasMultipleWordbook ? WordbooksTitle : wordbook?.TitleUser;
            set { if (HasMultipleWordbook) return; wordbook.TitleUser = value; OnPropertyChanged();OnPropertyChanged(nameof(WordbookTitle)); } }
        public String WordbookTitleHtml => HasMultipleWordbook ? "" : wordbook?.Title;

        public String AuthenticationUserName => !CurrentTargetWordbookImpress ? "" : (((WordbookImpress)wordbook)?.Authentication?.UserName ?? "");
        public String AuthenticationPassword => !CurrentTargetWordbookImpress ? "" : (((WordbookImpress)wordbook)?.Authentication?.Password ?? "");

        public bool CurrentTargetWordbookImpress => !HasMultipleWordbook && wordbook is WordbookImpress;

        private ObservableCollection<IWordViewModel> GetWords()
        {
            if (HasMultipleWordbook)
            {
                var w = new List<IWordViewModel>();
                foreach (var wb in this.wordbooks)
                {
                    foreach (var item in wb.Words)
                    {
                        w.Add(new WordViewModel(item, Record));
                    }
                    foreach (var item in wb.QuizChoices)
                    {
                        w.Add(new QuizChoiceViewModel(item, Record));
                    }
                }
                return new ObservableCollection<IWordViewModel>(w);
            }
            else
            {
                var w = new List<IWordViewModel>();
                foreach (var item in wordbook.Words)
                {
                    w.Add(new WordViewModel(item, Record));
                }
                foreach (var item in wordbook.QuizChoices)
                {
                    w.Add(new QuizChoiceViewModel(item, Record));
                }
                return new ObservableCollection<IWordViewModel>(w);
            }
        }

        private ObservableCollection<IWordViewModel> _words;
        private ObservableCollection<IWordViewModel> words { get => _words = _words ?? GetWords(); }
        private ObservableCollection<IWordViewModel> WordsCache = null;
        private void WordsUpdate()
        {
            WordsCache = null;
            OnPropertyChanged(nameof(Words));
        }
        public ObservableCollection<IWordViewModel> Words
        {
            get
            {
                if (WordsCache != null) return WordsCache;
                if (String.IsNullOrEmpty(SearchWord))
                {
                    switch (SortKind.Kind)
                    {
                        case SortKindType.headword:
                            {
                                var linq = words.OrderBy((w) => w.Head);
                                return SortKind.Ascending ?
                                    new ObservableCollection<IWordViewModel>(linq) :
                                    new ObservableCollection<IWordViewModel>(linq.Reverse());
                            }
                        case SortKindType.score:
                            {
                                var linq = words
                                    .OrderBy((w) => (w.ExcludeRemembered))
                                    .ThenBy((w) => ((double)w.AnswerCountCorrect / Math.Max(1, w.AnswerCountTotal)))
                                    .ThenBy((w) => (w.AnswerCountCorrect));
                                return SortKind.Ascending ?
                                    new ObservableCollection<IWordViewModel>(linq) :
                                    new ObservableCollection<IWordViewModel>(linq.Reverse());
                            }
                        case SortKindType.random:
                            {
                                var random = new Random(unchecked((int)DateTime.Now.Date.Ticks));
                                var dic = new Dictionary<IWordViewModel, int>();
                                foreach (var item in words)
                                {
                                    dic.Add(item, random.Next());
                                }
                                var linq = words
                                    .OrderBy((w) => (dic[w]));
                                return SortKind.Ascending ?
                                    new ObservableCollection<IWordViewModel>(linq) :
                                    new ObservableCollection<IWordViewModel>(linq.Reverse());
                            }
                        case SortKindType.original:
                        default:
                            return SortKind.Ascending ? words : new ObservableCollection<IWordViewModel>(words.Reverse());
                    }
                }
                return WordsCache = new ObservableCollection<IWordViewModel>(words.Where((w) => w.Head.Contains(SearchWord) || w.Description.Contains(SearchWord)).OrderBy(w => (w.Head == SearchWord ? "0" : (w.Head.Contains(SearchWord) ? "1" : "2")) + w.Head));
            }
            set { SetProperty(ref _words, value); SearchWord = ""; }
        }

        private String searchWord = "";
        public String SearchWord { get => searchWord; set { SetProperty(ref searchWord, value); WordsUpdate(); } }

        private SortKindInfo sortKind = new SortKindInfo(SortKindType.original, true);
        public SortKindInfo SortKind { get => sortKind; set { SetProperty(ref sortKind, value); WordsUpdate(); } }
        public struct SortKindInfo
        {
            public bool Ascending;
            public SortKindType Kind;

            public static SortKindInfo GetDefault()
            {
                return new SortKindInfo(SortKindType.original, true);
            }

            public SortKindInfo(SortKindType kind,bool ascending)
            {
                this.Ascending = ascending;
                this.Kind = kind;
            }
        }

        public enum SortKindType
        {
            original,headword,score,random
        }

        public bool IsValid => HasMultipleWordbook ? true : wordbook?.IsValid ?? false;

        public Record Record { get; private set; }

        #region Visibility
        private bool isVisibleDescription = true;
        public bool IsVisibleDescription { get => isVisibleDescription; set
            {
                SetProperty(ref isVisibleDescription, value);
                if (!IsVisibleHead && !IsVisibleDescription) { IsVisibleHead = true; }
                foreach (var item in Words)
                {
                    item.IsVisibleDescription = this.IsVisibleDescription;
                }
            }
        }

        private bool isVisibleHead = true;
        public bool IsVisibleHead { get => isVisibleHead; set
            {
                SetProperty(ref isVisibleHead, value);
                if (!IsVisibleHead && !IsVisibleDescription) { IsVisibleDescription = true; }
                foreach (var item in Words)
                {
                    item.IsVisibleHead = this.IsVisibleHead;
                }
            }
        }

        public System.Windows.Input.ICommand SwitchVisibilityHeadCommand
        {
            get
            {
                return switchVisibilityHeadCommand ?? (switchVisibilityHeadCommand = new Helper.DelegateCommand((o) => true, (o) =>
                {
                    IsVisibleHead = !IsVisibleHead;
                }));
            }
        }
        private System.Windows.Input.ICommand switchVisibilityHeadCommand;

        public System.Windows.Input.ICommand SwitchVisibilityDescriptionCommand
        {
            get
            {
                return switchVisibilityDescriptionCommand ?? (switchVisibilityDescriptionCommand = new Helper.DelegateCommand((o) => true, (o) =>
                {
                    IsVisibleDescription = !isVisibleDescription;
                }));
            }
        }
        private System.Windows.Input.ICommand switchVisibilityDescriptionCommand;
        #endregion

        public WordbookImpressViewModel():this(new WordbookImpress(),new Record())
        { }

        public WordbookImpressViewModel(IWordbook wordbook,Record record)
        {
            this.Record = record;
            this.wordbook = wordbook;

            this.SortKind = Storage.ConfigStorage.Content?.SortKind ?? SortKindInfo.GetDefault();
        }

        public WordbookImpressViewModel(IEnumerable<IWordbook> wordbooks, Record record,string Title)
        {
            WordbooksTitle = Title;
            this.Record = record;
            this.wordbooks = wordbooks.ToArray();

            this.SortKind = Storage.ConfigStorage.Content?.SortKind ?? SortKindInfo.GetDefault();
        }

        public async Task<(WordbookImpress wordbook, string html, string data,string format)> Reload()
        {
            if (HasMultipleWordbook)
            {
                IsBusy = true;
                var w = new List<IWordViewModel>();
                foreach (WordbookImpress wb in wordbooks)
                {
                    await wb.Reload();
                    foreach (var item in wb.Words)
                    {
                        w.Add(new WordViewModel(item, Record));
                    }
                    Words = new ObservableCollection<IWordViewModel>(w);
                }

                IsBusy = false;
                return (null, null, null,null);
            }
            else if(wordbook is WordbookImpress wbi)
            {
                IsBusy = true;
                var result = await wbi.Reload();
                OnPropertyChanged(nameof(UriLogo));
                OnPropertyChanged(nameof(WordbookTitle));

                var w = new List<IWordViewModel>();
                foreach (var item in result.wordbook.Words)
                {
                    w.Add(new WordViewModel(item, Record));
                }
                Words = new ObservableCollection<IWordViewModel>(w);

                IsBusy = false;
                return result;
            }
            else { return (null, null, null, null); }
        }

        public ReloadCommandClass ReloadCommand => new ReloadCommandClass(this);

        public class ReloadCommandClass : System.Windows.Input.ICommand
        {
            public event EventHandler CanExecuteChanged;

            public WordbookImpressViewModel Model { get; private set; }

            public ReloadCommandClass(WordbookImpressViewModel model)
            {
                this.Model = model;
                model.PropertyChanged += (s, e) => CanExecuteChanged(s, new EventArgs());
            }

            public bool CanExecute(object parameter)
            {
                return Model.IsValid;
            }

            public async void Execute(object parameter)
            {
                await Model.Reload();
                await Storage.WordbooksImpressStorage.SaveLocalData();
            }
        }

    }
}
