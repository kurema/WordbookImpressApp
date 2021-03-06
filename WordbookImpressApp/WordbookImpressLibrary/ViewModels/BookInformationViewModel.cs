﻿using System;
using System.Collections.Generic;
using System.Text;

using Nager.AmazonProductAdvertising.Model;
using System.Collections.ObjectModel;
using System.Linq;

namespace WordbookImpressLibrary.ViewModels
{
    public class BookInformationViewModel:BaseViewModel
    {
        private string _Author="";
        public string Author { get => _Author; set => SetProperty(ref _Author, value); }

        private string _Title="";
        public string Title { get => _Title; set => SetProperty(ref _Title, value); }

        private string _CoverImage="";
        public string CoverImage { get => _CoverImage; set => SetProperty(ref _CoverImage, value); }

        private string _Price="";
        public string Price { get => _Price; set => SetProperty(ref _Price, value); }

        private ObservableCollection<string> _Similar=new ObservableCollection<string>();
        public ObservableCollection<string> Similar { get => _Similar; set => SetProperty(ref _Similar, value); }

        private ObservableCollection<BookInformationLinkViewModel> _Links=new ObservableCollection<BookInformationLinkViewModel>();
        public ObservableCollection<BookInformationLinkViewModel> Links { get => _Links; set => SetProperty(ref _Links, value); }

        private bool? _SpecialObtained = null;
        public bool SpecialObtained { get => _SpecialObtained ?? false; set { if (_SpecialObtained == null) return; SetProperty(ref _SpecialObtained, value); SetSpecialObtained(); } }

        private async void SetSpecialObtained()
        {
            var history= await Storage.PurchaseHistoryStorage.GetPurchaseHistory();
            if (history.SpecialObtainedUrl == null) history.SpecialObtainedUrl = new List<string>();
            if (ImpressOfficialPage == null || ImpressOfficialPage.Count == 0) return;
            if (SpecialObtained && !history.SpecialObtainedUrl.Any(a => ImpressOfficialPage.Any(b => a.ToLower() == b.ToLower()))) history.SpecialObtainedUrl.AddRange(ImpressOfficialPage);
            else if(!SpecialObtained) history.SpecialObtainedUrl.RemoveAll(a => ImpressOfficialPage.Contains(a));
            Storage.PurchaseHistoryStorage.SaveLocalData();
        }


        private async void GetSpecialObtained()
        {
            if (_SpecialObtained == true) return;
            var history = await Storage.PurchaseHistoryStorage.GetPurchaseHistory();
            if (history.SpecialObtainedUrl == null) return;
            if (ImpressOfficialPage == null || ImpressOfficialPage.Count == 0) return;
            if (true == ImpressOfficialPage.All(a => true == history?.SpecialObtainedUrl?.Any(b => a.ToLower() == b.ToLower()))) _SpecialObtained = true; else _SpecialObtained = false;
            OnPropertyChanged(nameof(SpecialObtained));
        }

        public BookInformationViewModel(Item item = null, Schemas.WordbookSuggestion.infoBooksBook book = null)
        {
            SetValue(book, item);
        }

        public BookInformationViewModel(Schemas.WordbookSuggestion.infoBooksBook book)
        {
            SetValue(book);
            System.Threading.Tasks.Task.Run(async () =>
            {
                var PreferPrintedBook = (WordbookImpressLibrary.Storage.ConfigStorage.Content?.StorePreferPrintedBook ?? true) ? "printed_book" : "ebook";
                var asins = book?.ids?.OrderBy(a => a.binding == PreferPrintedBook ? 0 : 2).Select(a => a.Value);
                if (asins != null && asins.Count() > 0)
                {
                    AmazonItemResponse result;
                    result = await Helper.Functions.TryFetch(async () => await Storage.AmazonStorage.AmazonWrapper.LookupAsync(asins.ToArray(), AmazonResponseGroup.Images | AmazonResponseGroup.OfferSummary | AmazonResponseGroup.ItemAttributes | AmazonResponseGroup.Similarities));
                    SetValue(book, result?.Items?.Item);
                }
            });
        }

        private List<string> ImpressOfficialPage;

        public void SetValue(Schemas.WordbookSuggestion.infoBooksBook book = null, params Item[] items)
        {
            ImpressOfficialPage = new List<string>();
            var item = items?.FirstOrDefault();
            this.Title = book?.title ?? item?.ItemAttributes?.Title;
            this.Author = string.Join(" ", item?.ItemAttributes?.Author ?? new string[0]);
            this.CoverImage = book?.images?.FirstOrDefault()?.src ?? item?.LargeImage?.URL ?? item?.MediumImage?.URL ?? item?.SmallImage?.URL;

            Links = new ObservableCollection<BookInformationLinkViewModel>();

            if (book?.links != null)
            {
                foreach (var link in book?.links)
                {
                    if (string.IsNullOrEmpty(link?.type)) continue;
                    switch (link.type)
                    {
                        case "impress":
                            Links.Add(new BookInformationLinkViewModel("インプレスブックス公式ページ", link?.@ref));
                            if (link?.@ref != null) ImpressOfficialPage.Add(link?.@ref);
                            break;
                    }
                }
            }
            GetSpecialObtained();
            if (items != null)
            {
                string price = null;
                foreach (var itemAmazon in items)
                {
                    string version = "";
                    if (!string.IsNullOrWhiteSpace(itemAmazon?.ItemAttributes?.Binding)) { version = " (" + itemAmazon?.ItemAttributes?.Binding + ")"; }
                    string priceItem =itemAmazon?.OfferSummary?.LowestNewPrice?.FormattedPrice ??
                        (itemAmazon?.OfferSummary?.LowestUsedPrice?.FormattedPrice != null ? "中古" + itemAmazon?.OfferSummary?.LowestUsedPrice?.FormattedPrice : null);
                    price = price ?? priceItem;
                    //if (priceItem != null) { price += priceItem + version + " "; }

                    if (itemAmazon?.DetailPageURL != null) Links.Add(new BookInformationLinkViewModel("Amazon 商品ページ" + version, itemAmazon?.DetailPageURL));
                }
                this.Price = price;
                foreach (var itemAmazon in items)
                {
                    string version = "";
                    if (!string.IsNullOrWhiteSpace(itemAmazon?.ItemAttributes?.Binding)) { version = " (" + itemAmazon?.ItemAttributes?.Binding + ")"; }

                    //if (itemAmazon?.ItemLinks != null)
                    //{
                    //    foreach (var link in itemAmazon.ItemLinks)
                    //    {
                    //        Links.Add(new BookInformationLinkViewModel(link?.Description + version, link?.URL));
                    //    }
                    //}

                }
            }
            if (item?.SimilarProducts != null)
            {
                Similar = new ObservableCollection<string>();
                foreach (var similar in item?.SimilarProducts)
                {
                    if (similar?.ASIN != null)
                        Similar.Add(similar?.ASIN);
                }
                SimilarPrepared?.Invoke(this, new EventArgs());
            }
        }

        public event EventHandler SimilarPrepared;

        public class BookInformationLinkViewModel : BaseViewModel
        {
            private string _Title;
            public string Title { get => _Title; set => SetProperty(ref _Title, value); }

            private string _Url;
            public string Url { get => _Url; set => SetProperty(ref _Url, value); }

            public BookInformationLinkViewModel(string Title,string Url="")
            {
                this.Title = Title;
                this.Url = Url;
            }
        }
    }
}
