using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WordbookImpressLibrary.Models
{
    public interface IWordbook
    {
        bool IsValid { get; }
        string Title { get; }
        Word[] Words { get; }
        QuizChoice[] QuizChoices { get; }
        string Id { get; }
    }

    public class WordbookGeneral : IWordbook
    {
        public bool IsValid { get => true; }
        public string Title { get; set; }
        public Word[] Words { get; set; }
        public QuizChoice[] QuizChoices { get; set; }
        public string Id { get; set; }
    }
}
