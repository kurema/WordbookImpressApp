using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.Storage
{
    public class WordbooksImpressStorage
    {
        public static List<WordbookImpress> Content { get; private set; }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wordbooks_impress.xml");

        public static void Add(WordbookImpress item)
        {
            foreach(var wb in Content)
            {
                if (wb.Uri == item.Uri) return;
            }
            Content.Add(item);
        }

        public static async Task<WordbookImpress[]> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path))
            {
                Content = new List<WordbookImpress>();
                return new WordbookImpress[0];
            }
            var result = await Helper.SerializationHelper.DeserializeAsync<WordbookImpress[]>(Path);
            Content = new List<WordbookImpress>(result);
            OnUpdated();
            return result;
        }

        public static async Task SaveLocalData()
        {
            if (Content == null) return;
            await Helper.SerializationHelper.SerializeAsync(Content, Path);
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }
    }
}
