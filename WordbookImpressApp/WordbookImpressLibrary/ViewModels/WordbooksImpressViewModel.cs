using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

using System.Linq;

namespace WordbookImpressLibrary.ViewModels
{
    public class WordbooksImpressViewModel : BaseViewModel
    {
        private ObservableCollection<WordbookImpressViewModel> wordbooks;
        public ObservableCollection<WordbookImpressViewModel> Wordbooks
        {
            get => new ObservableCollection<WordbookImpressViewModel>(Order.Apply(wordbooks));
            set => SetProperty(ref wordbooks, value);
        }

        public WordbooksImpressViewModel()
        {
            this.wordbooks = new ObservableCollection<WordbookImpressViewModel>();
        }

        public enum OrderKind
        {
            Default=0,Title,Url
        }

        public struct OrderStatus
        {
            public bool Reversed { get; set; }
            public OrderKind Kind { get; set; }

            public IEnumerable<WordbookImpressViewModel> Apply(IList<WordbookImpressViewModel> w)
            {
                if (Reversed) return ApplyKind(w);
                else return ApplyKind(w).Reverse();
            }


            public IEnumerable<WordbookImpressViewModel> ApplyKind(IList<WordbookImpressViewModel> w)
            {
                switch (Kind)
                {
                    case OrderKind.Title:return w.OrderBy(a => a.WordbookTitle);
                    case OrderKind.Url:return w.OrderBy(a => a.Uri);
                    default: case OrderKind.Default: return w;
                }
            }
        }

        private OrderStatus _Order;
        public OrderStatus Order { get => _Order; set { SetProperty(ref _Order, value); OnPropertyChanged(nameof(Wordbooks)); } }


        public WordbooksImpressViewModel(Record record)
        {
            this.wordbooks = new ObservableCollection<WordbookImpressViewModel>();
        }

        public WordbooksImpressViewModel(IEnumerable<IWordbook> arg, Record record)
        {
            if (arg == null) { this.wordbooks = new ObservableCollection<WordbookImpressViewModel>(); return; }
            var result = new List<WordbookImpressViewModel>();
            foreach(var item in arg)
            {
                if (item is WordbookImpress)
                    result.Add(new WordbookImpressViewModel((WordbookImpress)item, record));
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
                    await Storage.WordbooksImpressStorage.SaveLocalData();
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
