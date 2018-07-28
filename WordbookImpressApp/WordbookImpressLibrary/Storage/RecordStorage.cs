using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.Storage
{
    public static class RecordStorage
    {
        public static StorageContent<Record> Storage = new StorageContent<Record>("record.xml");
        public static Record Content { get => Storage.Content; private set => Storage.Content = value; }
        public static string Path => Storage.Path;
        public static string PathBup => Storage.PathBackup;

        public static async Task<Record> LoadLocalData()
        {
            return await Storage.LoadLocalValue();
        }

        public static async void SaveLocalData()
        {
            await Storage.SaveLocalData();
        }

        public static event EventHandler Updated
        {
            add => Storage.Updated += value;
            remove => Storage.Updated -= value;
        }

        public static void Init()
        {
            Storage.Init();
        }
    }
}
