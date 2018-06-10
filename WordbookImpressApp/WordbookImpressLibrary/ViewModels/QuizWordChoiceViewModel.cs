using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.ViewModels
{
    public class QuizWordChoiceViewModel : BaseViewModel
    {
        public QuizWordChoiceViewModel()
        {
            Seed = new Random().Next();
        }

        public int Seed { get; set; }

        private ObservableCollection<string> choices;
        public ObservableCollection<string> Choices
        {
            get
            {
                //ToDo
                return null;
            }
        }
        private Word CurrentWord;
        private Record Record;
        private Wordbook WordbookTarget;
        private Wordbook[] WordbooksForChoice;
        public string CurrentWordDescription { get
            {
                switch (ChoiceType)
                {
                    case ChoiceKind.Title:return CurrentWord.Title;
                    case ChoiceKind.Description:return CurrentWord.Description;
                    default:return "";
                }
            }
        }
        private ChoiceKind choiceType=ChoiceKind.Description;
        public ChoiceKind ChoiceType
        {
            get => choiceType;
            set
            {
                SetProperty(ref choiceType, value);
                OnPropertyChanged(nameof(Choices));
                choices = null;
            }
        }

        public enum ChoiceKind
        {
            Title,Description
        }

        public QuizWordChoiceViewModel(int choiceCount, Wordbook wordbook, Wordbook[] wordbooksChoice)
        {
            WordbooksForChoice = wordbooksChoice;
            WordbookTarget = wordbook;
            this.choiceCount = choiceCount;
        }

        private int choiceCount;
        public int ChoiceCount
        {
            get => choiceCount;
            set => SetProperty(ref choiceCount, value);
        }

        public QuizStatus currentQuizStatus;
        public QuizStatus CurrentQuizStatus
        {
            get => currentQuizStatus;
            set => SetProperty(ref currentQuizStatus, value);
        }

        public enum QuizStatus
        {
            Choice,Correct,Wrong
        }

        public bool Choose(string choice)
        {
            var dic = new Dictionary<ChoiceKind, string>()
            {
                {ChoiceKind.Title,CurrentWord.Title},
                {ChoiceKind.Description,CurrentWord.Description }
            };
            if (choice == dic[ChoiceType])
            {
                var status = Record.GetWordStatusByHash(CurrentWord.Hash);
                status.AnswerCountCorrect++;
                status.AnswerCountTotal++;
                Record.SetWordStatusByHash(CurrentWord.Hash, status);

                CurrentQuizStatus = QuizStatus.Correct;
                return true;
            }
            else
            {
                var status = Record.GetWordStatusByHash(CurrentWord.Hash);
                status.AnswerCountTotal++;
                Record.SetWordStatusByHash(CurrentWord.Hash, status);

                CurrentQuizStatus = QuizStatus.Wrong;
                return false;
            }
        }
    }
}
