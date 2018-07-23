using System;
using System.Collections.Generic;
using System.Text;

using System.Threading.Tasks;

using System.Text.RegularExpressions;

using System.Linq;

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

        public static List<KeyValuePair<string, List<string>>> GetCsvDictionary(System.IO.TextReader tr)
        {
            using (var reader = new CsvHelper.CsvReader(tr))
            {
                reader.Read();
                reader.ReadHeader();
                var headers = new List<string>();
                var result = new List<KeyValuePair<string, List<string>>>();
                foreach (var item in reader.Context.HeaderRecord)
                {
                    {
                        result.Add(new KeyValuePair<string, List<string>>(item, new List<string>()));
                        headers.Add(item);
                    }
                }
                while (reader.Read())
                {
                    for (int i = 0; i < Math.Min(headers.Count, reader.Context.Record.Length); i++)
                    {
                        result[i].Value.Add(reader.Context.Record[i]);
                    }
                }
                return result;
            }
        }

        public static (bool? isSmb,string url) DistinguishHttpCifs(string url,string ID="",string Password="")
        {
            ID = ID ?? "";
            Password = Password ?? "";
            if (Regex.Match(url, @"^https?\:\/\/|^ftp\:\/\/", RegexOptions.IgnoreCase).Success)
            {
                return (false, url);
            }
            else
            {
                {
                    var match = Regex.Match(url, @"^smb\:\/\/([^\/]+)\/(.*)$", RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        if (match.Groups[1].Value.Contains("@"))
                        {
                            return (true, url);
                        }
                        else
                        {
                            return (true, "smb://" + ID + ":" + Password + "@" + match.Groups[1].Value + "/" + match.Groups[2].Value);
                        }
                    }
                }
                if (url.StartsWith(@"\\"))
                {
                    return (true, "smb://" + ID + ":" + Password + "@" + url.Replace(@"\", "/").Substring(2));
                }
            }
            return (null, null);
        }
    }
}
