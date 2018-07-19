using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WordbookImpressLibrary.Models
{
    public class WordbookImpress:IWordbook
    {
        private Uri uri;
        public string Uri { get => uri?.ToString()??""; set { if (value != null && value != "")
                {
                    value = value.ToLower();
                    if (!value.EndsWith("/")) value += "/";
                    uri = new Uri(value);
                } } }

        public string Id => Uri;

        private Uri uriLogo;
        public string UriLogo { get => uriLogo?.ToString()??""; set { if (value != null && value != "") uriLogo = new Uri(value); } }
        public string Title { get; set; } = "";
        public string TitleUser { get; set; } = "";
        public Word[] Words { get; set; } = new Word[0];

        public Authentication Authentication { get; set; }

        public bool IsValid => uri != null || Words == null;

        public QuizChoice[] QuizChoices { get; set; } = new QuizChoice[0];

        public async Task<(WordbookImpress wordbook, string html, string data, string format)> Reload()
        {
            var result = await Load(this.uri, this.Authentication);
            Reload(result.wordbook);
            return result;
        }

        public void Reload(WordbookImpress wordbook)
        {
            this.uriLogo = wordbook.uriLogo;
            this.Title = wordbook.Title;
            this.Words = wordbook.Words;
            this.QuizChoices = wordbook.QuizChoices;
        }

        public static async Task<(WordbookImpress wordbook, string html, string data,string format)> Load(WordbookImpressLibrary.Models.WordbookImpressInfo info)
        {
            return await Load(new Uri( info.Url), new Authentication() { Password = info.Password, UserName = info.ID });
        }

        public static async Task<(WordbookImpress wordbook,string html,string data, string format)> Load(Uri uri, Authentication authentication)
        {
            var result = new WordbookImpress
            {
                uri = uri,
                Authentication = authentication
            };
            string html;
            string dataJs;
            string format = "";

            {
                var req = System.Net.WebRequest.Create(uri);
                if (authentication != null && !authentication.IsEmpty)
                    req.Credentials = new System.Net.NetworkCredential(authentication.UserName, authentication.Password);
                var webres = await req.GetResponseAsync();
                using (var stream = webres.GetResponseStream())
                {
                    using (var sr = new System.IO.StreamReader(stream))
                    {
                        html = await sr.ReadToEndAsync();
                        result.Title = GetTitle(html);
                        System.Uri.TryCreate(uri, GetImage(html), out Uri logoUri);
                        result.uriLogo = logoUri;
                    }
                }
            }
            try
            {
                format = WordbookImpressInfo.Formats.DataJs;
                System.Uri.TryCreate(uri, "data.js", out Uri uriData);
                var req = System.Net.WebRequest.Create(uriData);
                if (authentication == null || authentication.IsEmpty)
                    req.Credentials = new System.Net.NetworkCredential(authentication.UserName, authentication.Password);
                var webres = await req.GetResponseAsync();
                using (var stream = webres.GetResponseStream())
                {
                    using (var sr = new System.IO.StreamReader(stream))
                    {
                        dataJs = await sr.ReadToEndAsync();
                        result.Words = GetWords(dataJs);
                    }
                }
            }
            catch
            {
                format = WordbookImpressInfo.Formats.ConfigJs;
                System.Uri.TryCreate(uri, "config.js", out Uri uriData);
                var req = System.Net.WebRequest.Create(uriData);
                if (authentication == null || authentication.IsEmpty)
                    req.Credentials = new System.Net.NetworkCredential(authentication.UserName, authentication.Password);
                var webres = await req.GetResponseAsync();
                using (var stream = webres.GetResponseStream())
                {
                    using (var sr = new System.IO.StreamReader(stream))
                    {
                        dataJs = await sr.ReadToEndAsync();
                        result.QuizChoices = GetWordsConfig(dataJs);
                    }
                }
            }
            return (result, html, dataJs,format);
        }

        public static QuizChoice[] GetWordsConfig(string text)
        {
            //JSONのライブラリを使った方が手っ取り早いだろう。
            //"\[\s*\"([^\"]*)\"\s*,\s*\"([^\"]*)\"\s*\]"
            var reg = new Regex("\\{\\s*\\\"question\\\"\\s*:\\s*\\\"([^\\\"]*)\\\"\\s*,\\s*\\\"choice\\\":\\[([^\\[\\]]*)\\]\\s*,\\s*\\\"feedback\\\"\\s*:\\s*\\[\\\"([^\\\"]*)\\\"\\]\\s*,\\s*\\\"answer\\\":\\\"([^\\\"]*)\\\"\\}");
            var matches = reg.Matches(text);
            var words = new List<QuizChoice>();
            foreach (Match match in matches)
            {
                var choices = new Regex("\\\"([^\\\"]*)\\\"").Matches(match.Groups[2].Value);
                var choiceList = new List<string>();
                foreach (Match choice in choices)
                {
                    choiceList.Add(choice.Groups[1].Value);
                }
                words.Add(QuizChoice.GetQuizChoiceUnescape(match.Groups[1].Value, match.Groups[3].Value, choiceList.ToArray(), match.Groups[4].Value));
            }
            return words.ToArray();
        }

        public static Word[] GetWords(string text)
        {
            //"\[\s*\"([^\"]*)\"\s*,\s*\"([^\"]*)\"\s*\]"
            var reg = new Regex("\\[\\s*\\\"([^\\\"]*)\\\"\\s*,\\s*\\\"([^\\\"]*)\\\"\\s*\\]");
            var matches = reg.Matches(text);
            var words = new List<Word>();
            foreach (Match match in matches)
            {
                words.Add(Word.GetWordUnescape(match.Groups[1].Value, match.Groups[2].Value));
            }
            return words.ToArray();
        }

        public static string GetTitle(string text)
        {
            var reg = new Regex(@"<title>([^<>]*)</title>",RegexOptions.IgnoreCase);
            var matches = reg.Matches(text);
            if (matches.Count > 0 && matches[0].Groups.Count>0)
            {
                return matches[0].Groups[1].Value;
            }
            return null;
        }
        public static string GetImage(string text)
        {
            var reg = new Regex("<img [^<>]*id=[\\\"']start_image[\\\"'][^<>]*>", RegexOptions.IgnoreCase);
            var match = reg.Match(text);
            var reg2 = new Regex("src=[\\\"']([^\\\"']+)[\\\"']", RegexOptions.IgnoreCase);
            if (match.Success)
            {
                var match2 = reg2.Match(match.Value);
                if (match2.Success && match2.Groups.Count>0)
                {
                    return match2.Groups[1].Value;
                }
            }
            return null;
        }
    }

    public class Word
    {
        public string Title="";
        public string Description="";

        private string hash;
        public string Hash { get => hash ?? (hash = GetHash()); set => hash = value; }

        public static string GetAsText(string item)
        {
            var brReg = new Regex(@"<br[^<>]*\/?>");
            var tags = new Regex(@"<[^<>]+>");
            return tags.Replace(brReg.Replace(item, "\n"),"");
        }

        public static Word GetWordUnescape(string title,string description)
        {
            return new Word() { Title = GetAsText(Regex.Unescape(title)), Description = GetAsText(Regex.Unescape(description)) };
        }

        public string GetHash() {
            byte[] input = Encoding.ASCII.GetBytes(Helper.Functions.QuickEscape(Title) + "\n" + Helper.Functions.QuickEscape(Description));
            var sha = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(input);
            string result = "";

            for (int i = 0; i < hash.Length; i++)
            {
                result = result + string.Format("{0:X2}", hash[i]);
            }
            return result;
        }
    }

    public class QuizChoice
    {
        public string Title = "";
        public string[] Choices = new string[0];
        public string Answer = "";
        public string Description = "";

        private string hash;
        public string Hash { get => hash ?? (hash = GetHash()); set => hash = value; }

        public static QuizChoice GetQuizChoiceUnescape(string title,string description,string[] choices,string answer)
        {
            var choicesList = new List<string>();
            foreach(var item in choices)
            {
                choicesList.Add(Word.GetAsText(Regex.Unescape(item)));
            }
            return new QuizChoice() { Title = Word.GetAsText(Regex.Unescape(title)), Description = Word.GetAsText(Regex.Unescape(description)), Answer = Word.GetAsText(Regex.Unescape(answer)), Choices = choicesList.ToArray() };
        }

        public string GetHash()
        {
            string choiceText = "";
            foreach(var item in Choices)
            {
                choiceText += Helper.Functions.QuickEscape(item) + "\n";
            }
            byte[] input = Encoding.ASCII.GetBytes(Helper.Functions.QuickEscape(Title) + "\n" + Helper.Functions.QuickEscape(Description) + "\n\n" + choiceText);
            var sha = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(input);
            string result = "";

            for (int i = 0; i < hash.Length; i++)
            {
                result = result + string.Format("{0:X2}", hash[i]);
            }
            return result;
        }

    }

    public class Authentication
    {
        public string UserName;
        public string Password;

        public bool IsEmpty => string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password);
    }
}
