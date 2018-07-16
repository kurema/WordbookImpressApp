using System;
using Nager.AmazonProductAdvertising;
using WordbookImpressLibrary;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var authentification = new AmazonAuthentication();
            authentification.AccessKey = APIKeys.AmazonAccessKey;
            authentification.SecretKey = APIKeys.AmazonSecretKey;
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
