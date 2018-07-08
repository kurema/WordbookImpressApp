using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Helper
{
    public static class Functions
    {
        public static string QuickEscape(string str)
        {
            return str.Replace(@"\", @"\\").Replace("\n", @"\n");
        }
    }
}
