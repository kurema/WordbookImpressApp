using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WordbookImpressLibrary.Models
{
    public class Wordbook
    {
        public Uri Uri { get; private set; }
        public Uri UriLogo { get; private set; }
        public string Title { get; private set; }
        public Word[] Words { get; private set; }

        public Authentication Authentication { get; private set; }

        public bool IsValid => Uri != null || Words == null;

        public async Task<(Wordbook wordbook, string html, string data)> Reload()
        {
            var result = await Load(this.Uri, this.Authentication);
            Reload(result.wordbook);
            return result;
        }

        public void Reload(Wordbook wordbook)
        {
            this.UriLogo = wordbook.UriLogo;
            this.Title = wordbook.Title;
            this.Words = wordbook.Words;
        }

        public static async Task<(Wordbook wordbook,string html,string data)> Load(Uri uri, Authentication authentication)
        {
            var result = new Wordbook
            {
                Uri = uri,
                Authentication = authentication
            };
            string html;
            string dataJs;

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
                        Uri.TryCreate(uri, GetImage(html), out Uri logoUri);
                        result.UriLogo = logoUri;
                    }
                }
            }

            {
                Uri.TryCreate(uri, "data.js", out Uri uriData);
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
            return (result, html, dataJs);
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
        public string Title;
        public string Description;

        private string hash;
        public string Hash { get => hash ?? (hash = GetHash()); set => hash = value; }

        public static Word GetWordUnescape(string title,string description)
        {
            return new Word() { Title = Regex.Unescape(title), Description = Regex.Unescape(description) };
        }

        private static string QuickEscape(string str)
        {
            return str.Replace(@"\", @"\\").Replace("\n", @"\n");
        }

        public string GetHash() {
            byte[] input = Encoding.ASCII.GetBytes(QuickEscape(Title) + "\n" + QuickEscape(Description));
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
