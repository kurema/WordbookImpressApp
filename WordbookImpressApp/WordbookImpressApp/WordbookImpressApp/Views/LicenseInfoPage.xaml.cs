using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LicenseInfoPage : ContentPage
	{
        WordbookImpressLibrary.Models.License.ILicenseEntry Model => (this.BindingContext as WordbookImpressLibrary.Models.License.ILicenseEntry);

        public LicenseInfoPage ()
		{
			InitializeComponent ();
		}

        public LicenseInfoPage(WordbookImpressLibrary.Models.License.ILicenseEntry entry)
        {
            InitializeComponent();

            this.BindingContext = entry;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            try
            {
                Device.OpenUri(new Uri(Model.ProjectUrl));
            }
            catch { }
        }

        private void Button_Clicked_1(object sender, EventArgs e)
        {
            try
            {
                Device.OpenUri(new Uri(Model.LicenseUrl));
            }
            catch { }
        }
    }
}