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

        public WordViewModel(Word word, Record record)
        {
            this.Word = word;
            this.WordStatus = record.GetWordStatusByHash(word.Hash);
        }

        public uint AnswerCountTotal
        {
            get => WordStatus.AnswerCountTotal;
            set
            {
                SetProperty(ref WordStatus.AnswerCountTotal, value);
                Record.SetWordStatusByHash(Word.Hash, WordStatus);
            }
        }

        public uint AnswerCountCorrect
        {
            get => WordStatus.AnswerCountCorrect;
            set
            {
                SetProperty(ref WordStatus.AnswerCountCorrect, value);
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
