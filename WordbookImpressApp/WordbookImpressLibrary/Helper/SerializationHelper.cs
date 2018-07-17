using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace WordbookImpressLibrary.Helper
{
    public class SerializationHelper
    {
        static System.Threading.SemaphoreSlim semaphore = new System.Threading.SemaphoreSlim(1, 1);

        public static async Task SerializeAsync<T>(T data,string path)
        {
            await semaphore.WaitAsync();

            try
            {
                var xs = new XmlSerializer(typeof(T));
                using (var sw = new StreamWriter(path))
                {
                    await Task.Run(() => { try { xs.Serialize(sw, data); } catch { } });
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static async Task<T> DeserializeAsync<T>(string path)
        {
            try
            {
                using (var sr = new StreamReader(path))
                {
                    return await DeserializeAsync<T>(sr);
                }
            }
            finally
            {
            }
        }

        public static async Task<T> DeserializeAsync<T>(TextReader sr)
        {
            await semaphore.WaitAsync();
            try
            {
                var xs = new XmlSerializer(typeof(T));
                using (var xr = XmlReader.Create(sr, new XmlReaderSettings() { CheckCharacters = false }))
                {
                    return await Task.Run<T>(() => { try { return (T)xs.Deserialize(xr); } catch { return default(T); } });
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
