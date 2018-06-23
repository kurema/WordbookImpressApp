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
                    await Task.Run(() => xs.Serialize(sw, data));
                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        public static async Task<T> DeserializeAsync<T>(string path)
        {
            await semaphore.WaitAsync();
            try
            {
                var xs = new XmlSerializer(typeof(T));
                using (var sr = new StreamReader(path))
                using (var xr = XmlReader.Create(sr, new XmlReaderSettings() { CheckCharacters = false }))
                {
                    return await Task.Run(() => (T)xs.Deserialize(xr));
                }
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
