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
        private WordbookImpress[] wordbooks;
        public WordbookImpress[] Wordbooks { get => wordbooks ?? new WordbookImpress[] { wordbook }; }
        private WordbookImpress wordbook;
        public bool HasMultipleWordbook { get => wordbooks != null; }

        public String Uri => HasMultipleWordbook ? "" : wordbook?.Uri;
        public String UriLogo => Storage.ImageCacheStorage.GetImageUrl(HasMultipleWordbook ? "":wordbook?.UriLogo);
        public String WordbookTitle => String.IsNullOrEmpty(WordbookTitleUser) ? WordbookTitleHtml : WordbookTitleUser;
        public String WordbookTitleUser { get => HasMultipleWordbook ? WordbooksTitle : wordbook?.TitleUser; set { if (HasMultipleWordbook) return; wordbook.TitleUser = value; OnPropertyChanged(); } }
        public String WordbookTitleHtml => HasMultipleWordbook ? "" : wordbook?.Title;

        public String AuthenticationUserName => HasMultipleWordbook ? "" : (wordbook?.Authentication?.UserName ?? "");
        public String AuthenticationPassword => HasMultipleWordbook ? "" : (wordbook?.Authentication?.Password ?? "");

        private ObservableCollection<WordViewModel> words;
        public ObservableCollection<WordViewModel> Words
        {
            get
            {
                if (String.IsNullOrEmpty(SearchWord))
                {
                    switch (SortKind.Kind)
                    {
                        case SortKindType.headword:
                            {
                                var linq = words.OrderBy((w) => w.Head);
                                return SortKind.Ascending ?
                                    new ObservableCollection<WordViewModel>(linq) :
                                    new ObservableCollection<WordViewModel>(linq.Reverse());
                            }
                        case SortKindType.score:
                            {
                                var linq = words
                                    .OrderBy((w) => (w.ExcludeRemembered))
                                    .ThenBy((w) => ((double)w.AnswerCountCorrect / Math.Max(1, w.AnswerCountTotal)))
                                    .ThenBy((w) => (w.AnswerCountCorrect));
                                return SortKind.Ascending ?
                                    new ObservableCollection<WordViewModel>(linq) :
                                    new ObservableCollection<WordViewModel>(linq.Reverse());
                            }
                        case SortKindType.original:
                        default:
                            return SortKind.Ascending ? words : new ObservableCollection<WordViewModel>(words.Reverse());
                    }
                }
                return new ObservableCollection<WordViewModel>(words.Where((w) => w.Head.Contains(SearchWord) || w.Description.Contains(SearchWord)).OrderBy(w => (w.Head == SearchWord ? "0" : (w.Head.Contains(SearchWord) ? "1" : "2")) + w.Head));
            }
            set { SetProperty(ref words, value); SearchWord = ""; }
        }

        private String searchWord = "";
        public String SearchWord { get => searchWord; set { SetProperty(ref searchWord, value);OnPropertyChanged(nameof(Words)); } }

        private SortKindInfo sortKind = new SortKindInfo(SortKindType.original, true);
        public SortKindInfo SortKind { get => sortKind;set { SetProperty(ref sortKind, value); OnPropertyChanged(nameof(Words)); } }
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
            original,headword,score
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

        public WordbookImpressViewModel(WordbookImpress wordbook,Record record)
        {
            this.Record = record;
            this.wordbook = wordbook;
            var w = new List<WordViewModel>();
            foreach(var item in wordbook.Words)
            {
                w.Add(new WordViewModel(item, record));
            }
            words = new ObservableCollection<WordViewModel>(w);

            this.SortKind = Storage.ConfigStorage.Content?.SortKind ?? SortKindInfo.GetDefault();
        }

        public WordbookImpressViewModel(WordbookImpress[] wordbooks, Record record,string Title)
        {
            WordbooksTitle = Title;
            this.Record = record;
            this.wordbooks = wordbooks;
            var w = new List<WordViewModel>();
            foreach (var wb in this.wordbooks)
            {
                foreach (var item in wb.Words)
                {
                    w.Add(new WordViewModel(item, record));
                }
            }
            words = new ObservableCollection<WordViewModel>(w);

            this.SortKind = Storage.ConfigStorage.Content?.SortKind ?? SortKindInfo.GetDefault();
        }

        public async Task<(WordbookImpress wordbook, string html, string data)> Reload()
        {
            if (HasMultipleWordbook)
            {
                IsBusy = true;
                var w = new List<WordViewModel>();
                foreach (var wb in wordbooks)
                {
                    await wb.Reload();
                    foreach (var item in wb.Words)
                    {
                        w.Add(new WordViewModel(item, Record));
                    }
                    Words = new ObservableCollection<WordViewModel>(w);
                }

                IsBusy = false;
                return (null, null, null);
            }
            else
            {
                IsBusy = true;
                var result = await wordbook.Reload();
                OnPropertyChanged(nameof(UriLogo));
                OnPropertyChanged(nameof(WordbookTitle));

                var w = new List<WordViewModel>();
                foreach (var item in result.wordbook.Words)
                {
                    w.Add(new WordViewModel(item, Record));
                }
                Words = new ObservableCollection<WordViewModel>(w);

                IsBusy = false;
                return result;
            }
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
