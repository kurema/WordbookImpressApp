using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.Storage
{
    public class WordbooksImpressInfoStorage
    {
        public static List<WordbookImpressInfo> Content { get; private set; }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "impress_info.xml");

        public static void Add(WordbookImpressInfo item)
        {
            Content.Add(item);
        }

        public static async Task<WordbookImpressInfo[]> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path))
            {
                Content = new List<WordbookImpressInfo>();
                return new WordbookImpressInfo[0];
            }
            var result = await Helper.SerializationHelper.DeserializeAsync<WordbookImpressInfo[]>(Path);
            Content = new List<WordbookImpressInfo>(result);
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
