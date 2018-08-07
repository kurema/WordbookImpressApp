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
        public class Serialization
        {
            public class WordbookItem
            {
                //ToDo: You have to fix this agter you implemented IWordbook;
                public WordbookImpress ContentImpress { get; set; } = null;
                public WordbookGeneral ContentGeneral { get; set; } = null;
                public WordbookCsv ContentCsv { get; set; } = null;

                public IWordbook GetContent()
                {
                    if (ContentImpress != null)
                    {
                        return ContentImpress;
                    }
                    else if (ContentGeneral != null)
                    {
                        return ContentGeneral;
                    }
                    else if (ContentCsv != null)
                    {
                        return ContentCsv;
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
                        case WordbookCsv c: this.ContentCsv = c; break;
                        case WordbookGeneral l: this.ContentGeneral = l; break;
                    }
                }

                public static IWordbook[] Convert(WordbookItem[] arg)
                {
                    return arg.Select(s => s.GetContent()).Where(a => a != null).ToArray();
                }

                public static WordbookItem[] ConvertBack(IWordbook[] arg)
                {
                    return arg.Where(a => a != null).Select(a => new WordbookItem(a)).ToArray();
                }
            }
        }

        public static StorageContentConvert<List<Serialization.WordbookItem>, ObservableCollection<IWordbook>> Storage
            = new StorageContentConvert<List<Serialization.WordbookItem>, ObservableCollection<IWordbook>>("wordbooks_impress.xml"
                , a => new ObservableCollection<IWordbook>(Serialization.WordbookItem.Convert(a.ToArray()))
                , a => Serialization.WordbookItem.ConvertBack(a.ToArray())?.ToList()
                );
        public static ObservableCollection<IWordbook> Content
        {
            get => Storage.Converted;
            private set { Storage.Converted = value; Storage.Converted.CollectionChanged += (s, e) => Storage.OnUpdated(); }
        }
        public static string Path => Storage.Path;
        public static string PathBup => Storage.PathBackup;


        public static void Init()
        {
            Storage.Init();
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
            await Storage?.LoadLocalData();
            return Storage?.Converted?.ToArray();
        }

        public static async Task SaveLocalData()
        {
            await Storage?.SaveLocalData();
        }

        public static event EventHandler Updated
        {
            add => Storage.Updated += value;
            remove => Storage.Updated -= value;
        }
    }
}
