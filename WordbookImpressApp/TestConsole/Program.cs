using System;
using Nager.AmazonProductAdvertising;
using WordbookImpressLibrary;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

using System.Linq;
using System.Collections.Generic;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "";
            string id = "";
            string pw = "";

            var req = System.Net.WebRequest.Create(url);

            req.Credentials = new System.Net.NetworkCredential(id, pw);
            var webres = req.GetResponse();
            string result = "";
            using (var stream = webres.GetResponseStream())
            {
                using (var sr = new System.IO.StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }

            }
        }

        static void Main5(string[] args)
        {
            var client = new System.Net.Http.HttpClient();
            {
                var result = client.GetStringAsync("").Result;
                var match = System.Text.RegularExpressions.Regex.Match(result, @"(\{.+\})", System.Text.RegularExpressions.RegexOptions.Multiline);
                var json = JsonConvert.DeserializeObject(match.Groups[1].Value) as Newtonsoft.Json.Linq.JObject;
                var model = JsonConvert.DeserializeObject<Config>(match.Groups[1].Value);
                var jr = new JsonTextReader(new System.IO.StringReader(match.Groups[1].Value));
            }
            {
                var result = client.GetStringAsync("").Result;
                var match = System.Text.RegularExpressions.Regex.Match(result, @"(\[.+\])", System.Text.RegularExpressions.RegexOptions.Multiline);
                var json = JsonConvert.DeserializeObject(match.Groups[1].Value) as Newtonsoft.Json.Linq.JObject;
                var model = JsonConvert.DeserializeObject<string[][]>(match.Groups[1].Value);
                var jr = new JsonTextReader(new System.IO.StringReader(match.Groups[1].Value));
            }

        }

        public class Config
        {
            public Setting settings { get; set; }
            public List<Question> questions { get; set; }

            public class Setting
            {
                public string title { get; set; }
            }

            public class Question
            {
                public string question { get; set; }
                public string[] choice { get; set; }
                public string answer { get; set; }
                public string[] feedback { get; set; }
            }
        }

        static void Main4(string[] args)
        {
            using (var reader = new CsvHelper.CsvReader(new System.IO.StringReader("column1,column2\n001,33\navb,aa\n")))
            {
                reader.Read();
                reader.ReadHeader();
                while (reader.Read())
                {
                    var s= reader.GetField(1);
                    var t = reader.Context.HeaderRecord;
                }
            }
        }

        static void Main3(string[] args)
        {
            var authentification = new AmazonAuthentication
            {
                AccessKey = APIKeys.AmazonAccessKey,
                SecretKey = APIKeys.AmazonSecretKey
            };
            var w = new AmazonWrapper(authentification, AmazonEndpoint.JP, "kurema-22");
            //var AmazonRequiredResponse = Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.ItemAttributes | Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.Images | Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.OfferSummary;
            var s = w.Search("倫理 哲学", responseGroup: Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.Large);
        }

        static void Main2(string[] args)
        {
            var sr = new System.IO.StreamReader(@"D:\temp\WordbookImpressApp\res\nuget\list.csv");
            sr.ReadLine();
            var csv = new CsvHelper.CsvReader(sr);
            csv.Configuration.RegisterClassMap<AccountMap>();
            var records = csv.GetRecords<NugetData>();
            foreach (var item in records)
            {
                Console.WriteLine(item.Id+" "+item.Version+" "+item.LicenseUrl);
            }
            Console.ReadLine();
        }

        public sealed class AccountMap : CsvHelper.Configuration.ClassMap<NugetData>
        {
            public AccountMap()
            {
                Map(x => x.ProjectName).Name("ProjectName");
                Map(x => x.Id).Name("Id");
                Map(x => x.Version).Name("Version");
                Map(x => x.AllVersions).Name("AllVersions");
                Map(x => x.LicenseUrl).Name("LicenseUrl");
            }
        }

        public class NugetData
        {
            public string ProjectName { get; set; }
            public string Id { get; set; }
            public string Version { get; set; }
            public bool AllVersions { get; set; }
            public string LicenseUrl { get; set; }
        }
    }
}
