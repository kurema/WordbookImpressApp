﻿using System;
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
            var csv = new CsvHelper.CsvReader(new CsvHelper.CsvParser(sr, System.Globalization.CultureInfo.InvariantCulture));
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

        public interface ILicenseEntry
        {
            string ProjectName { get; }
            string Name { get; }
            string Version { get; }
            string LicenseUrl { get; }
            string LicenseText { get; set; }
            string ProjectUrl { get; }
        }

        public class NormalLicense : ILicenseEntry
        {
            public string ProjectName { get; set; }
            public string Name { get; set; }
            public string Version { get; set; }
            public string LicenseUrl { get; set; }
            public string LicenseText { get; set; }
            public string ProjectUrl { get; set; }
        }

        public class NugetData:ILicenseEntry
        {
            public string ProjectName { get; set; }
            public string Id { get; set; }
            public string Version { get; set; }
            public bool AllVersions { get; set; }
            public string LicenseUrl { get; set; }
            public string LicenseText { get; set; }
            public string Name => Id;
            public string ProjectUrl => "https://www.nuget.org/packages/" + Id;
        }
    }
}
