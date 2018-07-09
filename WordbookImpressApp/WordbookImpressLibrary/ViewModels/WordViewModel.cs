using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WordbookImpressLibrary.Models;
using System.ComponentModel;

namespace WordbookImpressLibrary.ViewModels
{
    public interface IWordViewModel:INotifyPropertyChanged
    {
        string Head { get;}
        string Description { get; }
        bool IsVisibleDescription { get; set; }
        bool IsVisibleHead { get; set; }
        System.Windows.Input.ICommand SwitchVisibilityHeadCommand { get; }
        System.Windows.Input.ICommand SwitchVisibilityDescriptionCommand { get; }
        double AnswerCountCorrectPercentage { get; }
        int AnswerCountTotal { get; set; }
        int AnswerCountCorrect { get; set; }
        int AnswerCountPass { get; set; }
        bool ExcludeRemembered { get; set; }
    }

    public class QuizChoiceViewModel : WordBaseViewModel, IWordViewModel
    {
        private Models.QuizChoice QuizChoice;
        private Record.WordStatus WordStatus { get => Record.GetWordStatusByHash(QuizChoice.Hash); set => Record.SetWordStatusByHash(QuizChoice.Hash, value); }
        private Record Record;

        public QuizChoiceViewModel(QuizChoice QuizChoice, Record record)
        {
            this.QuizChoice = QuizChoice;
            this.Record = record;
        }

        public string Head => QuizChoice.Title;

        public string Description => QuizChoice.Answer+"\n\n"+ QuizChoice.Description;

        public int AnswerCountTotal
        {
            get => WordStatus.AnswerCountTotal;
            set
            {
                var w = WordStatus;
                SetProperty(ref w.AnswerCountTotal, value);
                WordStatus = w;
                Record.SetWordStatusByHash(QuizChoice.Hash, WordStatus);
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
                var w = WordStatus;
                SetProperty(ref w.AnswerCountCorrect, value);
                WordStatus = w;
                Record.SetWordStatusByHash(QuizChoice.Hash, WordStatus);
            }
        }

        public int AnswerCountPass
        {
            get => WordStatus.AnswerCountPass;
            set
            {
                var w = WordStatus;
                SetProperty(ref w.AnswerCountPass, value);
                WordStatus = w;
                Record.SetWordStatusByHash(QuizChoice.Hash, WordStatus);
            }
        }

        public bool ExcludeRemembered
        {
            get => WordStatus.ExcludeRemembered;
            set
            {
                var w = WordStatus;
                SetProperty(ref w.ExcludeRemembered, value);
                WordStatus = w;
                Record.SetWordStatusByHash(QuizChoice.Hash, WordStatus);
            }
        }
    }

    public class WordBaseViewModel : BaseViewModel
    {
        #region Visibility
        private bool isVisibleDescription = true;
        public bool IsVisibleDescription { get => isVisibleDescription; set { SetProperty(ref isVisibleDescription, value); if (!IsVisibleHead && !IsVisibleDescription) { IsVisibleHead = true; } } }

        private bool isVisibleHead = true;
        public bool IsVisibleHead { get => isVisibleHead; set { SetProperty(ref isVisibleHead, value); if (!IsVisibleHead && !IsVisibleDescription) { IsVisibleDescription = true; } } }

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
    }

    public class WordViewModel: WordBaseViewModel, IWordViewModel
    {
        private Word Word;
        private Record.WordStatus WordStatus { get => Record.GetWordStatusByHash(Word.Hash); set => Record.SetWordStatusByHash(Word.Hash, value); }
        private Record Record;

        public string Head => Word?.Title ?? "";
        public string Description => Word?.Description ?? "";

        public WordViewModel() : this(new Word(), new Record()) { }


        public WordViewModel(Word word, Record record)
        {
            this.Word = word;
            this.Record = record;
        }

        public int AnswerCountTotal
        {
            get => WordStatus.AnswerCountTotal;
            set
            {
                var w = WordStatus;
                SetProperty(ref w.AnswerCountTotal, value);
                WordStatus = w;
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
                var w = WordStatus;
                SetProperty(ref w.AnswerCountCorrect, value);
                WordStatus = w;
                Record.SetWordStatusByHash(Word.Hash, WordStatus);
            }
        }

        public int AnswerCountPass
        {
            get => WordStatus.AnswerCountPass;
            set
            {
                var w = WordStatus;
                SetProperty(ref w.AnswerCountPass, value);
                WordStatus = w;
                Record.SetWordStatusByHash(Word.Hash, WordStatus);
            }
        }

        public bool ExcludeRemembered
        {
            get => WordStatus.ExcludeRemembered;
            set
            {
                var w = WordStatus;
                SetProperty(ref w.ExcludeRemembered, value);
                WordStatus = w;
                Record.SetWordStatusByHash(Word.Hash, WordStatus);
            }
        }

    }
}
