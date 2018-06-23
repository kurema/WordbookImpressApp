using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.ViewModels
{
    public class WordbooksImpressViewModel : BaseViewModel
    {
        private ObservableCollection<WordbookImpressViewModel> wordbooks;
        public ObservableCollection<WordbookImpressViewModel> Wordbooks
        {
            get => wordbooks;
            set => SetProperty(ref wordbooks, value);
        }

        public WordbooksImpressViewModel(Record record)
        {
            this.wordbooks = new ObservableCollection<WordbookImpressViewModel>();
        }

        public WordbooksImpressViewModel(IEnumerable<WordbookImpress> arg, Record record)
        {
            if (arg == null) { this.wordbooks = new ObservableCollection<WordbookImpressViewModel>(); return; }
            var result = new List<WordbookImpressViewModel>();
            foreach(var item in arg)
            {
                result.Add(new WordbookImpressViewModel(item, record));
            }
            this.wordbooks = new ObservableCollection<WordbookImpressViewModel>(result);
        }

        private System.Windows.Input.ICommand reloadCommand;
        public System.Windows.Input.ICommand ReloadCommand
        {
            get
            {
                var result = new Helper.DelegateCommand((o) => !this.IsBusy, async (o) =>
                {
                    this.IsBusy = true;
                    foreach (var item in wordbooks)
                    {
                        await item.Reload();
                    }
                    this.IsBusy = false;
                });
                this.PropertyChanged += (o, e) =>
                {
                    if (e.PropertyName == nameof(this.IsBusy))
                    {
                        result.OnCanExecuteChanged();
                    }
                };
                reloadCommand = result;
                return result;
            }
        }
    }
}
