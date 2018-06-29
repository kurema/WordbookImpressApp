using System;
using System.Collections.Generic;
using System.Text;

using WordbookImpressLibrary.Models;
using System.Collections.ObjectModel;

using System.Threading.Tasks;

namespace WordbookImpressApp.Storage
{
    public class LicenseStorage
    {
        public static ObservableCollection<License.NugetData> NugetDatas { get => nugetDatas = nugetDatas ?? GetNugetDatasTotal(); private set => nugetDatas = value; }
        public static bool IsLicenseTextLoaded = false;
        private static ObservableCollection<License.NugetData> nugetDatas;

        public static ObservableCollection<License.NugetData> GetNugetDatasTotal()
        {
            var result = GetNugetDatasCsv();
            foreach (var item in AdditonalLicense)
            {
                result.Add(item);
            }
            return result;
        }


        public static License.NugetData[] AdditonalLicense => new License.NugetData[]
        {
            new License.NugetData(){AllVersions=false,Id="Google Noto Fonts.Noto Sans CJK JP",LicenseUrl="http://scripts.sil.org/cms/scripts/page.php?site_id=nrsi&id=OFL",ProjectName="WordbookImpressApp",Version="v2017-06-01-serif-cjk-1-1"}
        };

        public static ObservableCollection<License.NugetData> GetNugetDatasCsv()
        {
            ObservableCollection<License.NugetData> nugets;
            try
            {
                using (var sr = new System.IO.StreamReader(typeof(LicenseStorage).Assembly.GetManifestResourceStream(nameof(WordbookImpressApp) + ".Licenses.nuget.csv")))
                {
                    nugets = License.GetNugetData(sr);
                }
            }
            catch
            {
                return new ObservableCollection<License.NugetData>();
            }
            return nugets;
        }

        public async static Task LoadNugetDatasLicenseText()
        {
            var nugets = NugetDatas;
            foreach (var item in nugets)
            {
                try
                {
                    using (var sr = new System.IO.StreamReader(typeof(LicenseStorage).Assembly.GetManifestResourceStream(nameof(WordbookImpressApp) + ".Licenses." + item.Id + ".txt")))
                    {
                        item.LicenseText = await sr.ReadToEndAsync();
                    }

                }
                catch (Exception e)
                {
                    item.LicenseText = item.LicenseUrl;
                }
            }
            IsLicenseTextLoaded = true;
            NugetDatas = nugets;
        }
    }
}
