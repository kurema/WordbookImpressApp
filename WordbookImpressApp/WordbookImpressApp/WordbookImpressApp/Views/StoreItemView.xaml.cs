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
	public partial class StoreItemView : ContentView
	{
        public ImageSource ItemImageSource { get => image.Source; set { if (image != null) image.Source = value; } }
        public string ItemText { get => label.Text; set { if (label != null) label.Text = value; } }
        public string Price { get => labelPrice.Text; set
            {
                if (string.IsNullOrWhiteSpace(value)) grid.RowDefinitions[2].Height = 0;
                if (labelPrice != null) labelPrice.Text = value;
            } }

        public Action Action { get; set; }

		public StoreItemView ()
		{
			InitializeComponent ();
		}

        public StoreItemView(string title, string price, ImageSource source, Action action = null) : this()
        {
            this.ItemText = title;
            this.Price = price;
            this.ItemImageSource = source;
            this.Action = action;
        }

        private void ClickGestureRecognizer_Clicked(object sender, EventArgs e)
        {
            this.Action?.Invoke();
        }
    }

    public class HumbleWidthLabel : Label
    {
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var result = base.OnMeasure(widthConstraint, heightConstraint);
            return new SizeRequest(new Size(0, result.Request.Height), result.Minimum);
        }
    }
}