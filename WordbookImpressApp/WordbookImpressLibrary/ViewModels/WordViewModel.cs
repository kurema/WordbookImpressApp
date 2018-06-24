using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.ViewModels
{
    public class WordViewModel:BaseViewModel
    {
        private Word Word;
        private Record.WordStatus WordStatus;
        private Record Record;

        public string Head => Word?.Title ?? "";
        public string Description => Word?.Description ?? "";

        #region Visibility
        private bool isVisibleDescription = true;
        public bool IsVisibleDescription { get => isVisibleDescription; set { SetProperty(ref isVisibleDescription, value); if (!IsVisibleHead && !IsVisibleDescription) { IsVisibleHead = true; } } }

        private bool isVisibleHead = true;
        public bool IsVisibleHead { get => isVisibleHead; set { SetProperty(ref isVisibleHead, value); if (!IsVisibleHead && !IsVisibleDescription) { IsVisibleDescription = true; } } }

        public System.Windows.Input.ICommand SwitchVisibilityHeadCommand { get { return switchVisibilityHeadCommand ?? (switchVisibilityHeadCommand = new Helper.DelegateCommand((o) => true, (o) =>
        {
            IsVisibleHead = !IsVisibleHead;
            
        })); } }
        private System.Windows.Input.ICommand switchVisibilityHeadCommand;

        public System.Windows.Input.ICommand SwitchVisibilityDescriptionCommand { get { return switchVisibilityDescriptionCommand ?? (switchVisibilityDescriptionCommand = new Helper.DelegateCommand((o) => true, (o) =>
        {
            IsVisibleDescription = !isVisibleDescription;
            
        })); } }
        private System.Windows.Input.ICommand switchVisibilityDescriptionCommand;
        #endregion

        public WordViewModel() : this(new Word(), new Record()) { }


        public WordViewModel(Word word, Record record)
        {
            this.Word = word;
            this.WordStatus = record.GetWordStatusByHash(word.Hash);
            this.Record = record;
        }

        public int AnswerCountTotal
        {
            get => WordStatus.AnswerCountTotal;
            set
            {
                SetProperty(ref WordStatus.AnswerCountTotal, value);
                Record.SetWordStatusByHash(Word.Hash, WordStatus);
            }
        }

        public double AnswerCountCorrectPercentage
        {
            get => WordStatus.AnswerCountTotal == 0 ? 0 : WordStatus.AnswerCountCorrect / (double)WordStatus.AnswerCountTotal * 100;
        }

        public int AnswerCountCorrect
        {
            get => WordStatus.AnswerCountCorrect;
            set
            {
                SetProperty(ref WordStatus.AnswerCountCorrect, value);
                Record.SetWordStatusByHash(Word.Hash, WordStatus);
            }
        }

        public int AnswerCountPass
        {
            get => WordStatus.AnswerCountPass;
            set
            {
                SetProperty(ref WordStatus.AnswerCountPass, value);
                Record.SetWordStatusByHash(Word.Hash, WordStatus);
            }
        }

        public bool ExcludeRemembered
        {
            get => WordStatus.ExcludeRemembered;
            set
            {
                SetProperty(ref WordStatus.ExcludeRemembered, value);
                Record.SetWordStatusByHash(Word.Hash, WordStatus);
            }
        }

    }
}
