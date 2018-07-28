using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using WordbookImpressLibrary.Models;

namespace WordbookImpressLibrary.Storage
{
    public static class ConfigStorage
    {
        public static Config _Content;
        public static Config Content { get => _Content = _Content ?? new Config(); private set => _Content = value; }
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "config.xml");
        public static string PathBup { get; set; } = Path + ".bup";

        static System.Threading.SemaphoreSlim semaphore = new System.Threading.SemaphoreSlim(1, 1);

        public static async Task<Config> LoadLocalData()
        {
            if (!System.IO.File.Exists(Path)) return Content = new Config();
            try
            {
                await semaphore.WaitAsync();

                try
                {
                    Content = await Helper.SerializationHelper.DeserializeAsync<Config>(Path);
                    if (Content != null) { OnUpdated(); return Content; }
                }
                catch
                {
                }
                try
                {
                    Content = await Helper.SerializationHelper.DeserializeAsync<Config>(PathBup);
                    if (Content == null) return new Config();
                    if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
                    if (System.IO.File.Exists(PathBup)) { System.IO.File.Move(PathBup, Path); }
                }
                catch
                {
                    return new Config();
                }
                OnUpdated();
                return Content;
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static async void SaveLocalData()
        {
            if (_Content == null) return;
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
