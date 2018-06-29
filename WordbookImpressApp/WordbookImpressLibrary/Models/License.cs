using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.Models
{
    public class License
    {
        public static ObservableCollection<NugetData> GetNugetData(System.IO.StreamReader sr)
        {
            sr.ReadLine();
            var csv = new CsvHelper.CsvReader(sr);
            csv.Configuration.RegisterClassMap<AccountMap>();
            var records = csv.GetRecords<NugetData>();
            var result = new ObservableCollection<NugetData>();
            foreach (var item in records)
            {
                result.Add(item);
            }
            return result;
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
            public string LicenseText { get; set; }
        }
    }
}
