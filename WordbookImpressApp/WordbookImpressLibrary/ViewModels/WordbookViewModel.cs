using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.ViewModels
{
    public class WordbookViewModel : BaseViewModel
    {
        private Wordbook Wordbook;

        public String Uri => Wordbook?.Uri?.ToString();
        public String UriLogo => Wordbook?.UriLogo?.ToString();
        public String WordbookTitle => Wordbook?.Title;

        public String AuthenticationUserName => Wordbook?.Authentication?.UserName ?? "";
        public String AuthenticationPassword => Wordbook?.Authentication?.Password ?? "";

        private ObservableCollection<WordViewModel> words;
        public ObservableCollection<WordViewModel> Words { get => words; set => SetProperty(ref words, value); }

        public bool IsValid => Wordbook?.IsValid ?? false;

        public WordbookViewModel(Wordbook wordbook,Record record)
        {
            this.Wordbook = wordbook;
            var w = new List<WordViewModel>();
            foreach(var item in wordbook.Words)
            {
                w.Add(new WordViewModel(item, record));
            }
            words = new ObservableCollection<WordViewModel>(w);
        }

        public async Task<(Wordbook wordbook, string html, string data)> Reload()
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

            public WordbookViewModel Model { get; private set; }

            public ReloadCommandClass(WordbookViewModel model)
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
