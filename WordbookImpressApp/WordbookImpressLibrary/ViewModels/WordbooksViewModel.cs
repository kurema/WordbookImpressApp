using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.ViewModels
{
    public class WordbooksViewModel : BaseViewModel
    {
        private ObservableCollection<WordbookViewModel> wordbooks;
        public ObservableCollection<WordbookViewModel> Wordbooks
        {
            get => wordbooks;
            set => SetProperty(ref wordbooks, value);
        }
    }
}
