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
    }
}
