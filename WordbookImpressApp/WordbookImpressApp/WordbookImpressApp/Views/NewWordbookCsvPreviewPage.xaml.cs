using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressLibrary.ViewModels;

namespace WordbookImpressApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewWordbookCsvPreviewPage : ContentPage
    {
        public RegisterWordbookCsvViewModel ModelCsv
        {
            get => this.BindingContext as RegisterWordbookCsvViewModel;
            set => this.BindingContext = value;
        }

        public SpreadSheetImageView SpreadSheetImageView => this.sheetImgCsv;

        public NewWordbookCsvPreviewPage()
        {
            InitializeComponent();
        }

        public NewWordbookCsvPreviewPage(RegisterWordbookCsvViewModel model) : this()
        {
            this.BindingContext = model;
        }
    }
}