using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Schemas.WordbookSuggestion;
using WordbookImpressLibrary.Schemas.AuthorInformation;

using System.Linq;

namespace WordbookImpressLibrary.Storage
{
    public static class RemoteStorage
    {
        public static info WordbookSuggestion { get; private set; }
        public static string WordbookSuggestionPath { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WordbookSuggestion.xml");
        public static string WordbookSuggestionUrl => "https://kurema.github.io/api/impress/wordbooks.xml";

        public static author AuthorInformation { get; private set; }
        public static string AuthorInformationPath { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AuthorInformation.xml");
        public static string AuthorInformationUrl => "https://kurema.github.io/api/impress/author.xml";

        public static async Task<T> LoadLocalData<T>(string path)
        {
            if (!System.IO.File.Exists(path))
            {
                return default(T);
            }
            try
            {
                return await Helper.SerializationHelper.DeserializeAsync<T>(path);
            }
            catch
            {
                return default(T);
            }
        }

        public static async Task LoadRemoteDatas()
        {
            WordbookSuggestion = await LoadRemoteData<info>(WordbookSuggestionUrl, WordbookSuggestionPath) ?? await LoadLocalData<info>(WordbookSuggestionPath);
            AuthorInformation = await LoadRemoteData<author>(AuthorInformationUrl, AuthorInformationPath) ?? await LoadLocalData<author>(AuthorInformationPath);
            OnUpdated();
        }

        public static async Task<T> LoadRemoteData<T>(string url,string localPath)
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                var client = new System.Net.Http.HttpClient();
                try
                {
                    var str = await client.GetStringAsync(url);
                    var result = await Helper.SerializationHelper.DeserializeAsync<T>(new System.IO.StringReader(str));
                    if (System.IO.File.Exists(localPath)) System.IO.File.Delete(localPath);
                    using(var sw=new System.IO.StreamWriter(localPath))
                    {
                        await sw.WriteAsync(str);
                    }
                    return result;
                }
                catch
                {
                    return default(T);
                }
            }
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }

        public static string[] GetNameSuggestions(string url)
        {
            if (WordbookSuggestion == null) return new string[0];
            var s= WordbookSuggestion.wordbooks.Where((i) => i?.access?.url == url);
            var result = new List<string>();
            foreach(var item in s)
            {
                result.AddRange(item.title);
            }
            return result.ToArray();
        }

        public static infoBooksBook[] GetBookWithWordbook(string url)
        {
            if (WordbookSuggestion == null) return null;
            var s = WordbookSuggestion.wordbooks.Where((i) => i?.access?.url == url);
            var result = new List<infoBooksBook>();
            var result2 = new List<infoBooksBook>();
            foreach (var item in s)
            {
                var t = WordbookSuggestion?.books?.book?.Where((b) => b.obsolete != true && b?.special?.wordbook?.Count((w) => w.@ref == item.id) > 0);
                if (t?.Count() > 0)
                {
                    result.AddRange(t);
                }
                else
                {
                    t = WordbookSuggestion?.books?.book?.Where((b) => b?.special?.wordbook?.Count((w) => w.@ref == item.id) > 0);
                    if (t?.Count() > 0)
                    {
                        result2.AddRange(t);
                    }
                }
            }
            if (result.Count == 0) { return result2.ToArray(); }
            else { return result.ToArray(); }
        }

        public static string GetStringByMultilingal(IEnumerable<multilingalEntry> entry, System.Globalization.CultureInfo culture = null)
        {
            if (entry == null || entry.Count() == 0) return null;
            culture = culture ?? System.Globalization.CultureInfo.CurrentCulture;
            var w = entry.Where((e) => e.language == culture.Name || e.language == culture.Parent.Name);
            if (w.Count() > 0) { return w.First().Value; } else { return null; }
        }
    }
}
