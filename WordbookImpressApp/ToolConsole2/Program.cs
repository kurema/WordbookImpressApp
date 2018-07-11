using System;

using WordbookImpressLibrary.Helper;
using WordbookImpressLibrary.Schemas;
using System.IO;

using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Nager.AmazonProductAdvertising;

namespace WordbookImpressApp.ToolsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //args = new[] { "xml", "wordbook", @"" , @"" };
            //The API Key is suspended.

            if (!CheckArgsCount(args, 1)) { return; }
            else if (args[0] == "xml")
            {
                if (!CheckArgsCount(args, 2)) { return; }
                if (args[1] == "directory")
                {
                    if (!CheckArgsCount(args, 4)) { return; }
                    string pathXml = args[3];
                    bool defaultObsolete = !File.Exists(pathXml);
                    var info = File.Exists(pathXml) ? SerializationHelper.DeserializeAsync<info>(pathXml).Result : new info();
                    string pathDir = args[2];
                    var book = info?.books?.book?.ToList() ?? new System.Collections.Generic.List<infoBooksBook>();
                    foreach (var item in Directory.GetDirectories(pathDir))
                    {
                        if (!Directory.Exists(item)) { continue; }
                        foreach (var file in Directory.GetFiles(item))
                        {
                            if (File.Exists(file) && Path.GetExtension(file) == ".pdf")
                            {
                                var title = Path.GetFileNameWithoutExtension(file);
                                if (title.Contains("(軽量版)")) continue;
                                if (Path.GetFileName(item) == "DOSV POWER REPORT 付録") continue;
                                if (book.Count((b) => b.title == title) == 0)
                                {
                                    var newbook = new infoBooksBook();
                                    newbook.title = title;
                                    newbook.special = new infoBooksBookSpecial() { ebook = new object[] { new object() } };
                                    newbook.genre = Path.GetFileName(item);
                                    newbook.special = new infoBooksBookSpecial();
                                    newbook.images = new infoBooksBookImage[0];
                                    newbook.links = new infoBooksBookLink[0];
                                    newbook.obsolete = defaultObsolete;
                                    newbook.date_pushSpecified = true;
                                    newbook.date_push = DateTime.Now.Date;
                                    book.Add(newbook);
                                }
                            }
                        }
                    }
                    info.books = new infoBooks() { book = book.ToArray() };

                    if (File.Exists(pathXml))
                    {
                        var oldPath = pathXml + ".old";
                        if (File.Exists(oldPath)) File.Delete(oldPath);
                        File.Move(pathXml, oldPath);
                    }
                    SerializationHelper.SerializeAsync(info, pathXml).Wait();
                }
                else if (args[1] == "wordbook")
                {
                    if (!CheckArgsCount(args, 4)) { return; }
                    string pathXml = args[3];
                    var info = File.Exists(pathXml) ? SerializationHelper.DeserializeAsync<info>(pathXml).Result : new info();
                    //var wb = info?.wordbooks?.ToList() ?? new List<infoWordbook>();
                    var wb = new List<infoWordbook>();
                    string text = "";
                    using (var sr = new StreamReader(args[2]))
                    {
                        text = sr.ReadToEnd();
                    }
                    var matches = new Regex(@"(.+)\n・URL：(.+)\n・ID：(.+)\n・パスワード：(.+)").Matches(text);
                    foreach (Match item in matches)
                    {
                        var title = item.Groups[1].Value.Replace("\n", "").Replace("\r", "");
                        var match = new Regex("^【.+】(.+)$").Match(title);
                        var url = item.Groups[2].Value.Replace("\n", "").Replace("\r", "");
                        var iwb = new infoWordbook();
                        if (match.Success)
                        {
                            iwb.title = new string[] { title, match.Groups[1].Value };
                        }
                        else
                        {
                            iwb.title = new string[] { title };
                        }
                        var match_id = new Regex(@"(\d+impress)").Match(url);
                        iwb.id = match_id.Success ? "quiz" + match_id.Value : url;
                        iwb.access = new infoWordbookAccess();
                        iwb.access.url = url;
                        wb.Add(iwb);
                    }
                    info.wordbooks = wb.ToArray();
                    SerializationHelper.SerializeAsync(info, pathXml).Wait();
                }
                else if (args[1] == "amazon")
                {
                    if (!CheckArgsCount(args, 5)) { return; }
                    string pathXml = args[2];
                    string AmazonAccessKey = args[3];
                    string AmazonSecretKey = args[4];

                    var info = File.Exists(pathXml) ? SerializationHelper.DeserializeAsync<info>(pathXml).Result : new info();
                    var book = info?.books?.book?.ToList();
                    if (book == null) return;

                    var authentification = new AmazonAuthentication();
                    authentification.AccessKey = AmazonAccessKey;
                    authentification.SecretKey = AmazonSecretKey;

                    var wrapper = new AmazonWrapper(authentification, AmazonEndpoint.JP, "kurema_wbimpress-22");

                    foreach (var item in book)
                    {
                        if (item.ids == null) item.ids = new infoBooksBookID[0];
                        if (item.ids?.Length > 0) continue;
                        var result= wrapper.Search(item.title, Nager.AmazonProductAdvertising.Model.AmazonSearchIndex.Books,
                            Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.Images|Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.Large);

                        int i = 0;
                        if (result?.Items?.Item == null) continue;
                        Console.WriteLine(item.title);
                        foreach(var res in result.Items.Item)
                        {
                            Console.WriteLine(i + ". " + res.ToString());
                            //Console.WriteLine(res.DetailPageURL);
                            Console.WriteLine(res.ItemAttributes.Binding);
                            i++;
                        }

                        int input;
                        var id_result = new List<infoBooksBookID>();
                        Console.WriteLine("書籍版を選択?");
                        if(int.TryParse(Console.ReadLine(), out input))
                        {
                            id_result.Add(new infoBooksBookID() { type = "ASIN", Value = result.Items.Item[input].ASIN, binding = "printed_book" });
                        }

                        Console.WriteLine("Kindle版を選択?");
                        if (int.TryParse(Console.ReadLine(), out input))
                        {
                            id_result.Add(new infoBooksBookID() { type = "ASIN", Value = result.Items.Item[input].ASIN, binding = "ebook" });
                        }

                        item.ids = id_result.ToArray();

                        info.books.book = book.ToArray();
                        SerializationHelper.SerializeAsync(info, pathXml).Wait();
                    }
                    info.books.book = book.ToArray();
                    SerializationHelper.SerializeAsync(info, pathXml).Wait();
                }
            }
        }

        static bool CheckArgsCount(string[] args, int count)
        {
            if (args.Length < count)
            {
                Console.WriteLine(count + " options are required.");
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
