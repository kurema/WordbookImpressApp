﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Models
{
    public class Config
    {
        public long SkipVoidTicks = -1;
        [System.Xml.Serialization.XmlIgnore()]
        public TimeSpan SkipVoidTimeSpan
        {
            get => new TimeSpan(SkipVoidTicks);
            set => SkipVoidTicks = value.Ticks;
        }
        public bool SkipChecked = false;
        public double SkipMinRate = 2.0;
        public int SkipMinRateMinTotal = 10;
        public int SkipMinCorrect = int.MaxValue;
        public int ChoiceCount = 4;
        public ViewModels.WordbookImpressViewModel.SortKindInfo SortKind = ViewModels.WordbookImpressViewModel.SortKindInfo.GetDefault();
        public ViewModels.WordbooksImpressViewModel.OrderStatus SortKindWordbooks;

        public int MaxDailyTestCount = 100;
        public bool ShowStatisticsZeroAnswer = true;
        public string CustomAmazonAssociateTag = "";

        public bool StorePreferPrintedBook = true;
        public string StoreOpenBookLink = "[DetailPageURL]";

        public bool EnableImpressBookFeature = true;

        public bool DemoModeCalendar = false;

        public Config()
        {
            this.EnableImpressBookFeature = System.Globalization.CultureInfo.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "ja" ? true : false;
        }
    }
}
