using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.Storage
{
    public static class RecordStorage
    {
        private static Record _Content;
        public static Record Content { get => _Content = _Content ?? new Record(); private set => _Content = value; }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "record.xml");
        public static string PathBup { get; set; } = Path + ".bup";

        static System.Threading.SemaphoreSlim semaphore = new System.Threading.SemaphoreSlim(1, 1);

        public static async Task<Record> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path)) return Content = new Record();
            try
            {
                await semaphore.WaitAsync();

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
            finally
            {
                semaphore.Release();
            }
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
            try
            {
                await semaphore.WaitAsync();

                if (System.IO.File.Exists(PathBup)) { System.IO.File.Delete(PathBup); }
                if (System.IO.File.Exists(Path)) { System.IO.File.Move(Path, PathBup); }
                await Helper.SerializationHelper.SerializeAsync(Content, Path);
                OnUpdated();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }
    }
}
