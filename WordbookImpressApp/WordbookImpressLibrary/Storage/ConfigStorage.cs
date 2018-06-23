using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.Storage
{
    public class ConfigStorage
    {
        public static Config Content { get; private set; }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.xml");

        public static async Task<Config> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path)) return Content = new Config();
            Content = await Helper.SerializationHelper.DeserializeAsync<Config>(Path);
            OnUpdated();
            return Content;
        }

        public static async void SaveLocalData()
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
