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

        public static async Task<PurchaseHistory> GetPurchaseHistory()
        {
            if (Content != null) return Content;
            if (!System.IO.File.Exists(Path)) return Content = new PurchaseHistory();
            try
            {
                Content = await SerializationHelper.DeserializeAsync<PurchaseHistory>(Path);
            }
            catch
            {
                try
                {
                    Content = await SerializationHelper.DeserializeAsync<PurchaseHistory>(PathBup);
                    if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
                    if (System.IO.File.Exists(PathBup)) { System.IO.File.Move(PathBup, Path); }
                }
                catch
                {
                    Content = new PurchaseHistory();
                }
            }
            return Content;
        }

        public static async void SaveLocalData()
        {
            if (Content == null) return;
            if (System.IO.File.Exists(PathBup)) { System.IO.File.Delete(PathBup); }
            if (System.IO.File.Exists(Path)) { System.IO.File.Move(Path, PathBup); }
            await SerializationHelper.SerializeAsync(Content, Path);
        }
    }
}
