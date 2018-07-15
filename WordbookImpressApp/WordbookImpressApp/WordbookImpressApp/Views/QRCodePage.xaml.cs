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
            //var barcodeWriter = new BarcodeWriter<Xamarin.Forms.ImageSource>()
            //{
            //    Format = BarcodeFormat.QR_CODE,
            //    Options = new ZXing.QrCode.QrCodeEncodingOptions
            //    {
            //        ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H,
            //        CharacterSet = "ISO-8859-1",
            //        Height = 160,
            //        Width = 160,
            //        Margin = 4
            //    }
            //};
            ////var stream = new System.IO.MemoryStream(barcodeWriter.Write(text).Pixels);
            ////stream.Position = 0;
            ////QRCanvas.Source = ImageSource.FromStream(() => stream);
            //QRCanvas.Source = barcodeWriter.Write(text);

            barcode.BarcodeOptions =
                new ZXing.QrCode.QrCodeEncodingOptions
                {
                    ErrorCorrection = ZXing.QrCode.Internal.ErrorCorrectionLevel.H,
                    //CharacterSet = "ISO-8859-1",
                    Height = 300,
                    Width = 300,
                    Margin = 10,
                };
            barcode.BarcodeFormat = ZXing.BarcodeFormat.QR_CODE;
            barcode.BackgroundColor = Color.Transparent;
            barcode.BarcodeValue = text;
            QRText.Text = text;
        }
	}
}