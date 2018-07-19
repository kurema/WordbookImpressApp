using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

namespace WordbookImpressLibrary.Helper
{
    public static class Functions
    {
        public static string QuickEscape(string str)
        {
            return str.Replace(@"\", @"\\").Replace("\n", @"\n");
        }

        public static async Task<T> TryFetch<T>(Func<Task<T>> func, int count = 3, int waitMilliseconds = 500)
        {
            T result = default(T);
            for (int i = 0; i < count; i++)
            {
                result = await func();
                if (result != null) break;
                await Task.Delay(waitMilliseconds);
            }
            return result;
        }

        public static Dictionary<string, List<string>> GetCsvDictionary(System.IO.TextReader tr) {
            using (var reader = new CsvHelper.CsvReader(tr))
            {
                reader.Read();
                reader.ReadHeader();
                var headers = new List<string>();
                var result = new Dictionary<string, List<string>>();
                foreach (var item in reader.Context.HeaderRecord)
                {
                    if (result.ContainsKey(item))
                    {
                        int i = 0;
                        for (; result.ContainsKey(item + i); i++) { }
                        result.Add(item + i, new List<string>());
                        headers.Add(item + i);
                    }
                    else
                    {
                        result.Add(item, new List<string>());
                        headers.Add(item);
                    }
                }
                while (reader.Read())
                {
                    for (int i = 0; i < Math.Min(headers.Count, reader.Context.Record.Length); i++)
                    {
                        result[headers[i]].Add(reader.Context.Record[i]);
                    }
                }
                return result;
            }
        }
    }
}
