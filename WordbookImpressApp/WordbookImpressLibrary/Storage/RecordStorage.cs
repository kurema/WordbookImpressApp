﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.Storage
{
    public static class RecordStorage
    {
        public static Record Content { get; private set; }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "record.xml");
        public static string PathBup { get; set; } = Path + ".bup";

        public static async Task<Record> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path)) return Content = new Record();
            try
            {
                Content = await Helper.SerializationHelper.DeserializeAsync<Record>(Path);
            }
            catch
            {
                try
                {
                    Content = await Helper.SerializationHelper.DeserializeAsync<Record>(PathBup);
                    if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
                    if (System.IO.File.Exists(PathBup)) { System.IO.File.Move(PathBup, Path); }
                }
                catch
                {
                    Content = new Record();
                }
            }
            OnUpdated();
            return Content;
        }

        public static async void SaveLocalData()
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