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
                if (Content != null)
                {
                    OnUpdated();
                    return Content;
                }
            }
            catch
            {
            }
            try
            {
                Content = await Helper.SerializationHelper.DeserializeAsync<Record>(PathBup);
                if (Content == null) return new Record();
                if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
                if (System.IO.File.Exists(PathBup)) { System.IO.File.Move(PathBup, Path); }
            }
            catch
            {
                return new Record();
            }
            OnUpdated();
            return Content;
        }

        public static void Init()
        {
            if (System.IO.File.Exists(PathBup)) { System.IO.File.Delete(PathBup); }
            if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
            Content = new Record();
            OnUpdated();
        }

        public static async void SaveLocalData()
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
