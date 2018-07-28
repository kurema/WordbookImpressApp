using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

using System.Linq;

namespace WordbookImpressLibrary.Storage
{
    public static class WordbooksImpressInfoStorage
    {
        public static StorageContent<ObservableCollection<WordbookImpressInfo>> Storage = new StorageContent<ObservableCollection<WordbookImpressInfo>>("impress_info.xml");

        public static ObservableCollection<WordbookImpressInfo> Content
        {
            get => Storage.Content;
            private set { Storage.Content = value; Storage.Content.CollectionChanged += (s, e) => Storage.OnUpdated(); }
        }
        public static string Path => Storage.Path;
        public static string PathBup => Storage.PathBackup;

        public static void Add(WordbookImpressInfo item)
        {
            if (Content?.Count(a => a.Url == item.Url && a.ID == item.ID && a.Password == item.Password && a.Format == item.Format) > 0) return;
            Content.Add(item);
        }

        public static void Init()
        {
            Storage.Init();
        }

        public static async Task<WordbookImpressInfo[]> LoadLocalData()
        {
            return (await Storage.LoadLocalData()).ToArray();
        }

        public static async Task SaveLocalData()
        {
            await Storage.SaveLocalData();
        }

        public static event EventHandler Updated
        {
            add => Storage.Updated += value;
            remove => Storage.Updated -= value;
        }
    }
}
