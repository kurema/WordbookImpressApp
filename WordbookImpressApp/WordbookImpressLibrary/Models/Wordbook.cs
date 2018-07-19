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
        string TitleUser { get; set; }
        Word[] Words { get; }
        QuizChoice[] QuizChoices { get; }
        string Id { get; }
    }

    public class WordbookGeneral : IWordbook
    {
        public bool IsValid { get => true; }
        public string Title => TitleUser;
        public string TitleUser { get; set; }
        public Word[] Words { get; set; }
        public QuizChoice[] QuizChoices { get; set; }
        public string Id { get; set; }

        public static async Task<Dictionary<string, List<string>>> LoadFromCsvHttp(string url, Authentication authentication, Encoding encoding)
        {
            var req = System.Net.WebRequest.Create(new Uri(url));
            if (authentication != null && !authentication.IsEmpty)
                req.Credentials = new System.Net.NetworkCredential(authentication.UserName, authentication.Password);
            var webres = await req.GetResponseAsync();
            return Helper.Functions.GetCsvDictionary(new System.IO.StreamReader(webres.GetResponseStream()));
        }
    }
}
