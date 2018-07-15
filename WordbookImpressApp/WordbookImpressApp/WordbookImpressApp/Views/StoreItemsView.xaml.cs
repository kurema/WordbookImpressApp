using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;

using WordbookImpressLibrary.Storage;

namespace WordbookImpressApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreItemsView : ContentView
    {
        public double DefaultHeight { get; set; } = 200;

        public StoreItemsView ()
		{
			InitializeComponent ();
		}

        public StoreItemView Add(string title,ImageSource source,Action action=null)
        {
            this.HeightRequest = DefaultHeight;
            var result = new StoreItemView(title, source, action) { VerticalOptions = LayoutOptions.FillAndExpand };
            stackLayout.Children.Add(result);
            return result;
        }

        public void Clear()
        {
            this.HeightRequest = 0;
            stackLayout.Children.Clear();
        }

        public void AddAmazonItem(params Nager.AmazonProductAdvertising.Model.Item[] items)
        {
            foreach (var result in items)
            {
                try
                {
                    var child = Add(result.ItemAttributes.Title, ImageSource.FromUri(new Uri(result.LargeImage.URL)), () => { try { Device.OpenUri(new Uri(result.DetailPageURL)); } catch { } });
                }
                catch (Exception e) { }
            }
        }

        public async void AddASIN(params string[] asins)
        {
            Nager.AmazonProductAdvertising.Model.AmazonItemResponse results = null;
            for(int i = 0; i < 3; i++) {
                results = await AmazonStorage.AmazonWrapper.LookupAsync(asins.ToList());
                if (results != null) { break; }
                await Task.Delay(500);
            }
            if (results?.Items?.Item == null) return;

            AddAmazonItem(results.Items.Item);
        }
	}
}