using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Schemas;

using System.Linq;

namespace WordbookImpressLibrary.Storage
{
    public static class WordbookSuggestionStorage
    {
        public static info Content { get; private set; }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WordbookSuggestion.xml");

        public static async Task<info> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path))
            {
                return null;
            }
            try
            {
                Content = await Helper.SerializationHelper.DeserializeAsync<info>(Path);
            }
            catch
            {
                return null;
            }
            OnUpdated();
            return Content;
        }

        public static async Task LoadRemoteData()
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                var client = new System.Net.Http.HttpClient();
                try
                {
                    var str = await client.GetStringAsync("https://kurema.github.io/api/impress/wordbooks.xml");
                    Content = await Helper.SerializationHelper.DeserializeAsync<info>(new System.IO.StringReader(str));
                    if (System.IO.File.Exists(Path)) System.IO.File.Delete(Path);
                    using(var sw=new System.IO.StreamWriter(Path))
                    {
                        await sw.WriteAsync(str);
                    }
                }
                catch
                {
                    await LoadLocalData();
                }
            }
            OnUpdated();
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }

        public static string[] GetNameSuggestions(string url)
        {
            if (Content == null) return new string[0];
            var s= Content.wordbooks.Where((i) => i?.access?.url == url);
            var result = new List<string>();
            foreach(var item in s)
            {
                result.AddRange(item.title);
            }
            return result.ToArray();
        }

        public static infoBooksBook[] GetBookWithWordbook(string url)
        {
            if (Content == null) return null;
            var s = Content.wordbooks.Where((i) => i?.access?.url == url);
            var result = new List<infoBooksBook>();
            foreach(var item in s)
            {
                var t = Content?.books?.book?.Where((b) => b?.special?.wordbook?.Count((w) => w.@ref == item.id) > 0);
                if (t?.Count() > 0)
                {
                    result.AddRange(t);
                }
            }
            return result.ToArray();
        }
    }
}
