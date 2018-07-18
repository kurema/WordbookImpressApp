using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

using System.Linq;

namespace WordbookImpressLibrary.Storage
{
    public static class WordbooksImpressStorage
    {
        private static ObservableCollection<IWordbook> content;
        public static ObservableCollection<IWordbook> Content { get => content; private set { content = value; content.CollectionChanged += (s, e) => OnUpdated(); } }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wordbooks_impress.xml");
        public static string PathBup { get; set; } = Path + ".bup";

        public class Serialization
        {
            public class WordbookItem
            {
                //ToDo: You have to fix this agter you implemented IWordbook;
                public WordbookImpress ContentImpress { get; set; } = null;
                public WordbookGeneral ContentGeneral { get; set; } = null;

                public IWordbook GetContent()
                {
                    if (ContentImpress != null)
                    {
                        return ContentImpress;
                    }
                    else if(ContentGeneral!=null)
                    {
                        return ContentGeneral;
                    }
                    return null;
                }

                public WordbookItem() { }
                public WordbookItem(IWordbook item)
                {
                    //https://ufcpp.net/study/csharp/datatype/typeswitch/?p=2
                    switch (item)
                    {
                        case WordbookImpress m: this.ContentImpress = m; break;
                        case WordbookGeneral l: this.ContentGeneral = l; break;
                    }
                }

                public static IWordbook[] Convert(WordbookItem[] arg)
                {
                    return arg.Select(s => s.GetContent()).ToArray();
                }

                public static WordbookItem[] ConvertBack(IWordbook[] arg)
                {
                    return arg.Select(a => new WordbookItem(a)).ToArray();
                }
            }
        }

        public static void Add(IWordbook item)
        {
            foreach(var wb in Content)
            {
                if (wb.Id == item.Id) return;
            }
            Content.Add(item);
        }

        public static async Task<IWordbook[]> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path))
            {
                Content = new ObservableCollection<IWordbook>();
                return new WordbookImpress[0];
            }
            IWordbook[] result;
            try
            {
                result = Serialization.WordbookItem.Convert(await Helper.SerializationHelper.DeserializeAsync<Serialization.WordbookItem[]>(Path));
            }
            catch
            {
                try
                {
                    result = Serialization.WordbookItem.Convert(await Helper.SerializationHelper.DeserializeAsync<Serialization.WordbookItem[]>(PathBup));
                    if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
                    if (System.IO.File.Exists(PathBup)) { System.IO.File.Move(PathBup, Path); }
                }
                catch
                {
                    result = new IWordbook[0];
                }
            }
            Content = new ObservableCollection<IWordbook>(result);
            OnUpdated();
            return result;
        }

        public static async Task SaveLocalData()
        {
            if (Content == null) return;
            if (System.IO.File.Exists(PathBup)) { System.IO.File.Delete(PathBup); }
            if (System.IO.File.Exists(Path)) { System.IO.File.Move(Path, PathBup); }
            OnUpdated();
            try
            {
                await Helper.SerializationHelper.SerializeAsync(Serialization.WordbookItem.ConvertBack(Content.ToArray()), Path);
            }
            catch
            {
            }
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }
    }
}
