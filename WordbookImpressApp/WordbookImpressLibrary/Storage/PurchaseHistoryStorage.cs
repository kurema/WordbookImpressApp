using System;
using System.Collections.Generic;
using System.Text;

using WordbookImpressLibrary.Models;
using WordbookImpressLibrary.Helper;

using System.Threading.Tasks;

namespace WordbookImpressLibrary.Storage
{
    public class PurchaseHistoryStorage
    {
        private static PurchaseHistory Content;
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "history.xml");
        public static string PathBup { get; set; } = Path + ".bup";

        public static bool SetPurchaseHistory(PurchaseHistory content)
        {
            if (Content == null || content == null) return false;
            Content = content;
            return true;
        }

        static System.Threading.SemaphoreSlim semaphore = new System.Threading.SemaphoreSlim(1, 1);

        public static async Task<PurchaseHistory> GetPurchaseHistory()
        {
            try
            {
                await semaphore.WaitAsync();
                if (Content != null) return Content;
                if (!System.IO.File.Exists(Path)) return Content = new PurchaseHistory();
                try
                {
                    Content = await SerializationHelper.DeserializeAsync<PurchaseHistory>(Path);
                    if (Content != null) return Content;
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Failed to load'{0}'",Path));
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
                try
                {
                    Content = await SerializationHelper.DeserializeAsync<PurchaseHistory>(PathBup);
                    if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
                    if (System.IO.File.Exists(PathBup)) { System.IO.File.Move(PathBup, Path); }
                    return Content ?? new PurchaseHistory();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("Failed to load'{0}'", PathBup));
                    System.Diagnostics.Debug.WriteLine(e.Message);
                }
                return new PurchaseHistory();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static async void SaveLocalData()
        {
            if (Content == null) return;
            if (System.IO.File.Exists(PathBup)) { System.IO.File.Delete(PathBup); }
            if (System.IO.File.Exists(Path)) { System.IO.File.Move(Path, PathBup); }
            await SerializationHelper.SerializeAsync(Content, Path);
            OnUpdated();
        }

        public static event EventHandler Updated;
        public static void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }
    }
}
