using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Schemas;

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

        public static async Task<info> Load()
        {
            var result = await LoadLocalData();
            if (result != null) return result;
            LoadRemoteData();
            //ToDo: LoadRemoteData終了時に通知。
            return null;
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
