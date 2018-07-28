using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.Storage
{
    public static class ConfigStorage
    {
        public static StorageContent<Config> Storage=new StorageContent<Config>("config.xml");

        public static Config Content { get => Storage.Content; private set => Storage.Content = value; }
        public static string Path => Storage.Path;
        public static string PathBup => Storage.PathBackup;

        public static async Task<Config> LoadLocalData()
        {
             return await Storage.LoadLocalData();
        }

        public static async void SaveLocalData()
        {
            await Storage.SaveLocalData();
        }

        public static event EventHandler Updated
        {
            add=> Storage.Updated += value;
            remove => Storage.Updated -= value;
        }

        public static void Init()
        {
            Storage.Init();
        }
    }
}
