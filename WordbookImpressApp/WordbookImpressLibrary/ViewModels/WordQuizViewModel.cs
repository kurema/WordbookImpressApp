using System;
using System.Collections.Generic;
using System.Text;
using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.ViewModels
{
    public class WordQuizViewModel : WordViewModel
    {
        public WordQuizViewModel(Word word, Record record) : base(word, record) { }

        private bool isVisibleDescription=true;
        public bool IsVisibleDescription { get => isVisibleDescription; set => SetProperty(ref isVisibleDescription, value); }

        private bool isVisibleHead=true;
        public bool IsVisibleHead { get => isVisibleHead; set => SetProperty(ref isVisibleHead, value); }

        public System.Windows.Input.ICommand SwitchVisibilityHeadCommand { get { return switchVisibilityHeadCommand ?? (switchVisibilityHeadCommand = new Helper.DelegateCommand((o) => true, (o) => { IsVisibleHead = !IsVisibleHead; })); } }
        private System.Windows.Input.ICommand switchVisibilityHeadCommand;

        public System.Windows.Input.ICommand SwitchVisibilityDescriptionCommand { get { return switchVisibilityDescriptionCommand ?? (switchVisibilityDescriptionCommand = new Helper.DelegateCommand((o) => true, (o) => { IsVisibleDescription = !isVisibleDescription; })); } }
        private System.Windows.Input.ICommand switchVisibilityDescriptionCommand;

    }
}
