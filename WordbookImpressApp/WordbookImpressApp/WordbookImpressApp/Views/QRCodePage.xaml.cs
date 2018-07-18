using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using ZXing.Mobile;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class QRCodePage : ContentPage
	{
		public QRCodePage ()
		{
			InitializeComponent ();
		}

        public QRCodePage(string text) : this()
        {
            barcode.BarcodeOptions =
                new ZXing.QrCode.QrCodeEncodingOptions
                {
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.M,
                    Height = 300,
                    Width = 300,
                    Margin = 0,
                    //CharacterSet= "Shift_JIS",
                    };
            barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            barcode.BackgroundColor = Color.Transparent;
            barcode.BarcodeValue = text;
            QRText.Text = text;
        }
	}
}