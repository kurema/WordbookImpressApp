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

using System.Text.RegularExpressions;

using WordbookImpressLibrary.Storage;

namespace WordbookImpressApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreItemsView : ContentView
    {
        public double DefaultHeight { get; set; } = 180;

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
                    var child = Add(result?.ItemAttributes?.Title,
                        result?.OfferSummary?.LowestNewPrice?.FormattedPrice ?? (result?.OfferSummary?.LowestUsedPrice?.FormattedPrice != null ? "中古" + result?.OfferSummary?.LowestUsedPrice?.FormattedPrice : null) ?? "",
                        ImageSource.FromUri(new Uri(result.LargeImage.URL)), async () =>
                        {
                            try { Device.OpenUri(new Uri(GetBookDetailUrl(result))); } catch { }
                            var history = await PurchaseHistoryStorage.GetPurchaseHistory();
                            if (history != null && !history.ClickedASIN.Contains(result.ASIN)) history.ClickedASIN.Add(result.ASIN);
                            PurchaseHistoryStorage.SaveLocalData();
                        });
                    //if (result?.LargeImage?.Width != null && result?.LargeImage?.Height != null && result.LargeImage.Height.Units == result.LargeImage.Width.Units)
                    //{
                    //    child.WidthRequest = DefaultHeight / (double)result.LargeImage.Height.Value * (double)result.LargeImage.Width.Value;
                    //}
                }
                catch { }
            }
        }

        public static string GetBookDetailUrl(Nager.AmazonProductAdvertising.Model.Item item)
        {
            var dic = new Dictionary<string, string>()
            {
                { "DetailPageURL", item?.DetailPageURL },
                { "Title", System.Web.HttpUtility.UrlEncode(item?.ItemAttributes?.Title??"") },
                { "Author", System.Web.HttpUtility.UrlEncode(string.Join(" ", item?.ItemAttributes?.Author??new string[0])??"") },
                { "ISBN", item?.ItemAttributes?.ISBN },
                { "EAN", item?.ItemAttributes?.EAN },
                { "ASIN", item?.ASIN },
            };

            var result = WordbookImpressLibrary.Storage.ConfigStorage.Content.StoreOpenBookLink ?? "[DetailPageURL]";
            result = Regex.Replace(result, @"\[([a-zA-Z,]+)\]", a =>
             {
                 var keys = a.Groups[1].Value.Split(',');
                 foreach (var key in keys)
                 {
                     if (dic.ContainsKey(key) && !String.IsNullOrWhiteSpace(dic[key])) return dic[key];
                 }
                 return a.Value;
             });

            return result;
        }

        public static Nager.AmazonProductAdvertising.Model.AmazonResponseGroup AmazonRequiredResponse => Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.ItemAttributes | Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.Images | Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.OfferSummary;

        public async Task<Nager.AmazonProductAdvertising.Model.AmazonItemResponse> AddASIN(params string[] asins)
        {
            try
            {
                var result = await TryFetch(async () => await AmazonStorage.AmazonWrapper?.LookupAsync(asins.Take(10).ToList(), AmazonRequiredResponse));
                if (result?.Items?.Item == null) return null;
                AddAmazonItem(result.Items.Item);
                return result;
            }
            catch (Exception e) { return null; }
        }

        public async Task<(Nager.AmazonProductAdvertising.Model.AmazonItemResponse, Nager.AmazonProductAdvertising.Model.AmazonItemResponse)> AddRelaed(params string[] asins)
        {
            try
            {
                var result = await TryFetch(async () => await AmazonStorage.AmazonWrapper?.LookupAsync(asins.ToList(), Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.Similarities | Nager.AmazonProductAdvertising.Model.AmazonResponseGroup.ItemAttributes));
                if (result?.Items?.Item == null) return (null, null);
                var related = result?.Items?.Item?.SelectMany((w) => w?.SimilarProducts)?.Where((w) => w != null)?.Select((w) => w?.ASIN)?.ToArray();
                var result2 = await AddASIN(related);
                return (result,result2);
            }
            catch { return (null, null); }
        }


        public async Task<Nager.AmazonProductAdvertising.Model.AmazonItemResponse> AddSearchResult(string keyword, Nager.AmazonProductAdvertising.Model.AmazonSearchIndex index = Nager.AmazonProductAdvertising.Model.AmazonSearchIndex.All)
        {
            try
            {
                var result = await TryFetch(async () => await AmazonStorage.AmazonWrapper?.SearchAsync(keyword, index, AmazonRequiredResponse));
                if (result?.Items?.Item == null) return null;
                AddAmazonItem(result.Items.Item);
                return result;
            }
            catch (Exception e) { return null; }
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