using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.ViewModels
{
    public class WordbookImpressViewModel : BaseViewModel
    {
        public WordbookImpress Wordbook { get; private set; }

        public String Uri => Wordbook?.Uri;
        public String UriLogo => Wordbook?.UriLogo;
        public String WordbookTitle => Wordbook?.Title;

        public String AuthenticationUserName => Wordbook?.Authentication?.UserName ?? "";
        public String AuthenticationPassword => Wordbook?.Authentication?.Password ?? "";

        private ObservableCollection<WordViewModel> words;
        public ObservableCollection<WordViewModel> Words { get => words; set => SetProperty(ref words, value); }

        public bool IsValid => Wordbook?.IsValid ?? false;

        public Record Record { get; private set; }

        #region Visibility
        private bool isVisibleDescription = true;
        public bool IsVisibleDescription { get => isVisibleDescription; set
            {
                SetProperty(ref isVisibleDescription, value);
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
                    if (!IsVisibleHead && !IsVisibleDescription) { IsVisibleDescription = true; }
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
                    if (!IsVisibleHead && !IsVisibleDescription) { IsVisibleHead = true; }
                }));
            }
        }
        private System.Windows.Input.ICommand switchVisibilityDescriptionCommand;
        #endregion


        public WordbookImpressViewModel(WordbookImpress wordbook,Record record)
        {
            this.Record = record;
            this.Wordbook = wordbook;
            var w = new List<WordViewModel>();
            foreach(var item in wordbook.Words)
            {
                w.Add(new WordViewModel(item, record));
            }
            words = new ObservableCollection<WordViewModel>(w);
        }

        public async Task<(WordbookImpress wordbook, string html, string data)> Reload()
        {
            IsBusy = true;
            var result = await Wordbook.Reload();
            OnPropertyChanged(nameof(UriLogo));
            OnPropertyChanged(nameof(WordbookTitle));
            IsBusy = false;
            return result;
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
            }
        }

    }
}
