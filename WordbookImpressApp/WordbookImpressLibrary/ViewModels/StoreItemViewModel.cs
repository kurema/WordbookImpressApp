using System;
using System.Collections.Generic;
using System.Text;

using Nager.AmazonProductAdvertising;

namespace WordbookImpressLibrary.ViewModels
{
    public class StoreItemViewModel:BaseViewModel
    {
        //private string imageLarge;
        //private string imageMedium;
        //private string imageSmall;
        //public string ImageLarge { get => imageLarge; set => SetProperty(ref imageLarge, value); }
        //public string ImageMedium { get => imageSmall; set => SetProperty(ref imageMedium, value); }
        //public string ImageSmall { get => imageSmall; set => SetProperty(ref imageSmall, value); }
        private string title;
        public string Title { get => title; set => SetProperty(ref title, value); }

        public static StoreItemViewModel GetFromASIN(string asin)
        {
            var result = Storage.AmazonStorage.AmazonWrapper.Lookup(asin);
            return new StoreItemViewModel()
            {
            };
        }
    }
}
