using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Schemas;

namespace WordbookImpressLibrary.Storage
{
    public class WordbookSuggestionStorage
    {
        public static wordbooks Content { get; private set; }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "WordbookSuggestion.xml");

        public static async Task<wordbooks> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path))
            {
                return Content = new wordbooks();
            }

            Content = await Helper.SerializationHelper.DeserializeAsync<wordbooks>(Path);
            OnUpdated();
            return Content;
        }

        public static void LoadRemoteData()
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.DownloadFileAsync(new Uri("https://kurema.github.io/api/impress/wordbooks.xml"), Path);
            }
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }
    }
}
