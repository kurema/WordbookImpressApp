using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.Storage
{
    public class StorageContent<T> where T : class , new()
    {
        private T _Content;
        public T Content
        {
            get => _Content = _Content ?? new T();
            set => _Content = value;
        }

        public bool IsLoaded { get; private set; } = false;

        public string Path { get; set; }
        public string PathBackup { get; set; }
        public void SetPath(string filename)
        {
            Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), filename);
            PathBackup = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), filename + ".bup");
        }

        private System.Threading.SemaphoreSlim semaphore = new System.Threading.SemaphoreSlim(1, 1);

        public void Init()
        {
            if (System.IO.File.Exists(PathBackup)) { System.IO.File.Delete(PathBackup); }
            if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
            Content = new T();
            OnUpdated();
        }

        public async Task<T> LoadLocalValue()
        {
            try
            {
                await semaphore.WaitAsync();

                try
                {
                    Content = await Helper.SerializationHelper.DeserializeAsync<T>(Path);
                    if (Content != null) return LoadLocalValueUpdatedReturn();
                }
                catch
                {
                }
                try
                {
                    Content = await Helper.SerializationHelper.DeserializeAsync<T>(PathBackup);
                    if (Content == null) return null;
                    if (System.IO.File.Exists(Path)) { System.IO.File.Delete(Path); }
                    if (System.IO.File.Exists(PathBackup)) { System.IO.File.Move(PathBackup, Path); }
                    return LoadLocalValueUpdatedReturn();
                }
                catch
                {
                    return null;
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private T LoadLocalValueUpdatedReturn()
        {
            IsLoaded = true;
            OnUpdated();
            return Content;
        }


        public StorageContent(string filename)
        {
            SetPath(filename);
        }

        private volatile bool DoubleSaveCheck = false;

        public async Task SaveLocalData()
        {
            if (_Content == null) return;
            try
            {
                DoubleSaveCheck = true;
                await semaphore.WaitAsync();
                //if DoubleSaveCheck is turned false while waiting. The latest value should be saved.
                if (DoubleSaveCheck == false) return;

                if (System.IO.File.Exists(PathBackup)) { System.IO.File.Delete(PathBackup); }
                if (System.IO.File.Exists(Path)) { System.IO.File.Move(Path, PathBackup); }
                DoubleSaveCheck = false;
                await Helper.SerializationHelper.SerializeAsync(Content, Path);
            }
            finally
            {
                semaphore.Release();
                OnUpdated();
            }
        }

        public event EventHandler Updated;
        public void OnUpdated()
        {
            Updated?.Invoke(null, new EventArgs());
        }

    }

    public class StorageContentArray<T>
    {
        private ObservableCollection<T> _Content;
        public ObservableCollection<T> Content
        {
            get => _Content = _Content ?? new ObservableCollection<T>(Storage.Content);
            set
            {
                if (_Content == null) { _Content = value; return; }
                _Content.Clear();
                foreach (var item in value) _Content.Add(item);
                //You have to access via Content. Not ObservableCollection<T> you substituted.
            }
        }

        public StorageContent<List<T>> Storage;

        public StorageContentArray(string filename)
        {
            Storage = new StorageContent<List<T>>(filename);
            Storage.Updated += (s, e) =>
            {
                Content = new ObservableCollection<T>(Storage.Content);
            };
        }
    }
}
