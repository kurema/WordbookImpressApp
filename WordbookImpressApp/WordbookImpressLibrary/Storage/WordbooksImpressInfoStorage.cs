using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.Storage
{
    public static class WordbooksImpressInfoStorage
    {
        private static ObservableCollection<WordbookImpressInfo> content;
        public static ObservableCollection<WordbookImpressInfo> Content { get => content; private set { content = value; content.CollectionChanged += (s, e) => OnUpdated(); } }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "impress_info.xml");
        public static string PathBup { get; set; } = Path + ".bup";

        public static void Add(WordbookImpressInfo item)
        {
            Content.Add(item);
        }

        public static async Task<WordbookImpressInfo[]> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path))
            {
                Content = new ObservableCollection<WordbookImpressInfo>();
                return new WordbookImpressInfo[0];
            }
            WordbookImpressInfo[] result;
            try
            {
                result = await Helper.SerializationHelper.DeserializeAsync<WordbookImpressInfo[]>(Path);
            }
            catch
            {
                try
                {
                    result = await Helper.SerializationHelper.DeserializeAsync<WordbookImpressInfo[]>(PathBup);
                    if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
                    if (System.IO.File.Exists(PathBup)) { System.IO.File.Move(PathBup, Path); }
                }
                catch
                {
                    result = new WordbookImpressInfo[0];
                }
            }

            Content = new ObservableCollection<WordbookImpressInfo>(result);
            OnUpdated();
            return result;
        }

        public static async Task SaveLocalData()
        {
            if (Content == null) return;
            if (System.IO.File.Exists(PathBup)) { System.IO.File.Delete(PathBup); }
            if (System.IO.File.Exists(Path)) { System.IO.File.Move(Path, PathBup); }
            await Helper.SerializationHelper.SerializeAsync(Content, Path);
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }
    }
}
