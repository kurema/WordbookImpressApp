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
        private static ObservableCollection<WordbookImpressInfo> content;
        public static ObservableCollection<WordbookImpressInfo> Content { get => content; private set { content = value; content.CollectionChanged += (s, e) => OnUpdated(); } }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "impress_info.xml");
        public static string PathBup { get; set; } = Path + ".bup";

        public static void Add(WordbookImpressInfo item)
        {
            if (Content?.Count(a => a.Url == item.Url && a.ID == item.ID && a.Password == item.Password && a.Format == item.Format) > 0) return;
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
            OnUpdated();
            await Helper.SerializationHelper.SerializeAsync(Content, Path);
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }
    }
}
