﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Storage
{
    public static class ImageCacheStorage
    {
        public static string Path { get; set; } = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "img");

        public static string PlaceholderWordbook { get; set; }

        public static string GetHash(string url)
        {
            byte[] input = Encoding.ASCII.GetBytes(url);
            var sha = new System.Security.Cryptography.SHA256CryptoServiceProvider();
            byte[] hash = sha.ComputeHash(input);
            string result = "";

            for (int i = 0; i < Math.Min(hash.Length, 15); i++)
            {
                result = result + string.Format("{0:X2}", hash[i]);
            }
            return result;
        }

        public static void Clear()
        {
            try
            {
                if (System.IO.Directory.Exists(Path))
                {
                    var items = System.IO.Directory.GetFiles(Path);
                    foreach (var item in items)
                    {
                        System.IO.File.Delete(item);
                    }
                }
                else if(System.IO.File.Exists(Path))
                {

                }
                else
                {

                }
            }
            catch { }
        }

        public static string GetPath(string url)
        {
            return System.IO.Path.Combine(Path, GetHash(url));
        }

        public static string GetImageUrl(string url)
        {
            return url;
            // disabled image cache.
            //if (string.IsNullOrEmpty(url))
            //{
            //    return null;
            //}
            //var path = GetPath(url);
            //if (System.IO.File.Exists(path))
            //{
            //    return path;
            //}
            //else
            //{
            //    if (System.IO.Directory.Exists(Path))
            //    {
            //        System.IO.Directory.CreateDirectory(Path);
            //    }
            //    using (System.Net.WebClient wc = new System.Net.WebClient())
            //    {
            //        wc.DownloadFileAsync(new Uri(url), path);
            //    }
            //    return url;
            //}
        }

        public static void SaveImage(string url)
        {
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                wc.DownloadFileAsync(new Uri(url), GetPath(url));
            }
        }
    }
}
