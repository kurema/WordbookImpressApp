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

        public StoreItemView Add(string title,string price, ImageSource source,Action action=null)
        {
            this.HeightRequest = DefaultHeight;
            var result = new StoreItemView(title, price, source, action) { VerticalOptions = LayoutOptions.FillAndExpand };
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
                    //var child = Add(result.ItemAttributes.Title, result?.OfferSummary?.LowestNewPrice?.FormattedPrice ?? "-", ImageSource.FromUri(new Uri(result.LargeImage.URL)), () => { try { Device.OpenUri(new Uri(result.DetailPageURL)); } catch { } });
                    var child = Add(result?.ItemAttributes?.Title, result?.ItemAttributes?.ListPrice?.FormattedPrice ?? "" , ImageSource.FromUri(new Uri(result.LargeImage.URL)), () => { try { Device.OpenUri(new Uri(result.DetailPageURL)); } catch { } });
                }
                catch (Exception e) { }
            }
        }

        public static Nager.AmazonProductAdvertising.Model.AmazonResponseGroup AmazonRequiredResponse => Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.ItemAttributes | Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.Images | Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.OfferSummary;

        public async void AddASIN(params string[] asins)
        {
            try
            {
                var result = await TryFetch(async () => await AmazonStorage.AmazonWrapper?.LookupAsync(asins.ToList(), AmazonRequiredResponse));
                if (result?.Items?.Item == null) return;
                AddAmazonItem(result.Items.Item);
            }
            catch (Exception e) { }
        }

        public async void AddSearchResult(string keyword, Nager.AmazonProductAdvertising.Model.AmazonSearchIndex index = Nager.AmazonProductAdvertising.Model.AmazonSearchIndex.All)
        {
            try
            {
                var result = await TryFetch(async () => await AmazonStorage.AmazonWrapper?.SearchAsync(keyword, index, AmazonRequiredResponse));
                if (result?.Items?.Item == null) return;
                AddAmazonItem(result.Items.Item);
            }
            catch (Exception e) { }
        }

        public async Task<T> TryFetch<T>(Func<Task<T>> func,  int count=3, int waitMilliseconds = 500)
        {
            T result = default(T);
            for(int i = 0; i < count; i++)
            {
                result = await func();
                if (result != null && !result.Equals(default(T))) break;
                await Task.Delay(waitMilliseconds);
            }
            return result;
        }
	}
}