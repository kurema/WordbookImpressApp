﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.Storage
{
    public static class WordbooksImpressStorage
    {
        public static List<WordbookImpress> Content { get; private set; }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "wordbooks_impress.xml");
        public static string PathBup { get; set; } = Path + ".bup";

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
            WordbookImpress[] result;
            try
            {
                result = await Helper.SerializationHelper.DeserializeAsync<WordbookImpress[]>(Path);
            }
            catch
            {
                try
                {
                    result = await Helper.SerializationHelper.DeserializeAsync<WordbookImpress[]>(PathBup);
                    if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
                    if (System.IO.File.Exists(PathBup)) { System.IO.File.Move(PathBup, Path); }
                }
                catch
                {
                    result = new WordbookImpress[0];
                }
            }
            Content = new List<WordbookImpress>(result);
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