using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Globalization;

using WordbookImpressApp.Resx;

namespace WordbookImpressApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigPage : ContentPage
    {
        public ObservableCollection<SettingItems> Items { get; set; }

        public bool SaveOnDisappearing { get; set; } = true;

        protected override void OnDisappearing()
        {
            if (SaveOnDisappearing)
                WordbookImpressLibrary.Storage.ConfigStorage.SaveLocalData();

            base.OnDisappearing();
        }

        public ConfigPage(ObservableCollection<SettingItems> items) {
            InitializeComponent();

            MyListView.ItemsSource = Items = items;
        }

        public ConfigPage()
        {
            InitializeComponent();

            SaveOnDisappearing = true;

            var licenseChildren = GetLicenseChildren(Navigation);

            var storage = WordbookImpressLibrary.Storage.ConfigStorage.Content;
            Items = new ObservableCollection<SettingItems>
            {
                new SettingItems(AppResources.SettingQuizTitle)
                {
                    new SettingItem(AppResources.SettingQuizChoiceTitle, (w)=> String.Format( AppResources.SettingQuizChoiceDetail,storage.ChoiceCount)){
                        Action=async (s)=>{
                           storage.ChoiceCount=await GetByActionSheet<int>(AppResources.SettingQuizChoiceMessage
                               ,new int[]{ 2,3,4,5,6,7,8}.ToDictionary(a=>String.Format(AppResources.SettingQuizChoiceChoice,a))
                               ,storage.ChoiceCount,false,(a)=>(a is int)&&(int)a>=2);
                        }
                    },
                },
                new SettingItems(AppResources.SettingQuizConditionTitle)
                {
                    new SettingItem(AppResources.SettingQuizConditionRememberedTitle, (w)=>storage.SkipChecked?AppResources.SettingQuizConditionRememberedDetailTrue:AppResources.SettingQuizConditionRememberedDetailFalse, ! storage.SkipChecked)
                    {
                        Action=(s)=>{storage.SkipChecked=! s.BoolValue; return Task.CompletedTask; }
                    },
                    new SettingItem(AppResources.SettingQuizConditionCorrectCountTitle,(w)=> storage.SkipMinCorrect==int.MaxValue?AppResources.SettingQuizConditionCorrectCountDetailTrue: String.Format(AppResources.SettingQuizConditionCorrectCountDetailFalse, storage.SkipMinCorrect)){
                        Action=async (s)=>{
                           storage.SkipMinCorrect=await GetByActionSheet<int>(AppResources.SettingQuizConditionCorrectCountMessage
                               ,new Dictionary<string, int>{{ AppResources.SettingQuizConditionWordNotSkip, int.MaxValue} }.Union(new int[]{ 1,2,3,5,10}.Select(a=>new KeyValuePair<string, int>(String.Format(AppResources.SettingQuizConditionCorrectCountChoice,a),a))).ToDictionary(a=>a.Key,b=>b.Value)
                               ,storage.SkipMinCorrect,true,(a)=>(a is int)&&(int)a>=1);
                        }
                    },
                    new SettingItem(AppResources.SettingQuizConditionCorrectRateTitle, (w)=>storage.SkipMinRate==2 || storage.SkipMinRateMinTotal==int.MaxValue ? AppResources.SettingQuizConditionCorrectRateDetailTrue: 
                    String.Format(AppResources.SettingQuizConditionCorrectRateDetailFalse,storage.SkipMinRateMinTotal,storage.SkipMinRate*100)
                    ){
                        Children=new ObservableCollection<SettingItems>(){
                            new SettingItems(AppResources.WordCommentary){
                                new SettingItem(AppResources.WordExplanation,(w)=>AppResources.SettingQuizConditionCorrectRatePageExplanationExplanationMessage){
                                    Action=async (s) =>
                                    {
                                        await DisplayAlert(AppResources.WordExplanation,AppResources.SettingQuizConditionCorrectRatePageExplanationExplanationAlert,AppResources.AlertConfirmed);
                                    }
                                }
                            },
                            new SettingItems(AppResources.SettingQuizConditionCorrectRatePageRequirementTitle){
                                new SettingItem(AppResources.WordCorrectRate,(w)=>storage.SkipMinRate==2?AppResources.SettingQuizConditionCorrectRatePageRequirementCorrectRateDetailTrue:String.Format(AppResources.SettingQuizConditionCorrectRatePageRequirementCorrectRateDetailFalse,Math.Floor( storage.SkipMinRate*100))){
                                    Action =async (s)=>{
                                        storage.SkipMinRate=await GetByActionSheet<double>(AppResources.SettingQuizConditionCorrectRatePageRequirementCorrectRateChoiceMessage
                                            ,new Dictionary<string, double>{{ AppResources.SettingQuizConditionWordNotSkip, 2} }.Union(new double[]{ 0.5,0.6,0.75,0.9,1.0}.ToDictionary(a=>String.Format(AppResources.SettingQuizConditionCorrectRatePageRequirementCorrectRateChoice,Math.Floor(a*100)))).ToDictionary(a=>a.Key,a=>a.Value)
                                            ,storage.SkipMinRate,true,(a)=>(a is double)&&(((double)a>0 &&(double)a<=1)||(double)a==2),new PercentageValueConverter());
                                    }
                                }
                                ,new SettingItem(AppResources.WordCorrectCount,(w)=>storage.SkipMinRateMinTotal==int.MaxValue?AppResources.SettingQuizConditionCorrectRatePageRequirementCorrectCountDetailTrue:String.Format(AppResources.SettingQuizConditionCorrectRatePageRequirementCorrectCountDetailFalse,storage.SkipMinRateMinTotal)){
                                    Action =async (s)=>{
                                        storage.SkipMinRateMinTotal=await GetByActionSheet<int>(AppResources.SettingQuizConditionCorrectRatePageRequirementCorrectCountChoiceMessage
                                            ,new Dictionary<string, int>{{ AppResources.SettingQuizConditionWordNotSkip, int.MaxValue } }.Union(new int[]{ 3,5,10,20,50}.ToDictionary(a=>String.Format(AppResources.SettingQuizConditionCorrectRatePageRequirementCorrectCountChoice,a))).ToDictionary(a=>a.Key,a=>a.Value)
                                            ,storage.SkipMinRateMinTotal,true,(a)=>(a is int)&&(int)a>=1);
                                    }
                                }
                            }
                        }
                    },
                    new SettingItem(AppResources.SettingQuizConditionIgnoreTimeSpanTitle,(w)=>storage.SkipVoidTicks==-1?AppResources.SettingQuizConditionIgnoreTimeSpanDetailTrue: String.Format(AppResources.SettingQuizConditionIgnoreTimeSpanDetailFalse,ValueConverters.TimeSpanFormatValueConverter.FormatTimeSpan(new TimeSpan(storage.SkipVoidTicks),AppResources.SettingQuizConditionIgnoreTimeSpanTimeSpanFormat)))
                    {
                        Action=async (s) =>
                        {
                            storage.SkipVoidTicks=await GetByActionSheet<long>(AppResources.SettingQuizConditionIgnoreTimeSpanChoiceMessage,
                                new Dictionary<string, long>{ { AppResources.SettingQuizConditionIgnoreTimeSpanChoiceNever, -1} }.Union(new TimeSpan[]{TimeSpan.FromMinutes(10),TimeSpan.FromHours(1),TimeSpan.FromDays(1),TimeSpan.FromDays(7) }.ToDictionary(a=>ValueConverters.TimeSpanFormatValueConverter.FormatTimeSpan(a,AppResources.SettingQuizConditionIgnoreTimeSpanTimeSpanFormat),a=>a.Ticks)).ToDictionary(a=>a.Key,a=>a.Value)
                                ,storage.SkipVoidTicks,true,(a)=>(a is long)&&((long)a>=TimeSpan.FromSeconds(1).Ticks||(long)a==-1),new TimeSpanTicksValueConverter(),Keyboard.Plain);
                        }
                    }
                },
                new SettingItems(AppResources.SettingStatisticsTitle)
                {
                    new SettingItem(AppResources.SettingStatisticsDailyQuestionTitle, (w) => String.Format( AppResources.SettingStatisticsDailyQuestionDetail, + storage.MaxDailyTestCount))
                    {
                        Action=async (s) =>
                        {
                            storage.MaxDailyTestCount=await GetByActionSheet<int>(AppResources.SettingStatisticsDailyQuestionChoiceMessage,
                                new int[]{10,50,100,500}.ToDictionary(a=>String.Format(AppResources.SettingStatisticsDailyQuestionChoice,a),a=>a)
                                ,storage.MaxDailyTestCount);
                        }
                    },
                    new SettingItem(AppResources.SettingStatisticsZeroAnswerTitle, (w) => storage.ShowStatisticsZeroAnswer?AppResources.SettingStatisticsZeroAnswerDetailTrue:AppResources.SettingStatisticsZeroAnswerDetailFalse,storage.ShowStatisticsZeroAnswer)
                    {
                        Action=(s) =>
                        {
                            storage.ShowStatisticsZeroAnswer=s.BoolValue;
                            return Task.CompletedTask;
                        }
                    },
                    //new SettingItem(AppResources.SettingStatisticsAmazonAssociateIdTitle, AppResources.SettingStatisticsAmazonAssociateIdDetail)
                    //{
                    //    Action =async (s)=>{
                    //        var dic=new Dictionary<string,string>();
                    //        dic.Add(AppResources.SettingStatisticsAmazonAssociateIdChoiceProgrammer,WordbookImpressLibrary.APIKeys.AmazonAssociateTag);
                    //        if(!string.IsNullOrWhiteSpace( storage.CustomAmazonAssociateTag) && storage.CustomAmazonAssociateTag!=WordbookImpressLibrary.APIKeys.AmazonAssociateTag) dic.Add(storage.CustomAmazonAssociateTag,storage.CustomAmazonAssociateTag);
                    //        storage.CustomAmazonAssociateTag=await GetByActionSheet<string>(AppResources.SettingStatisticsAmazonAssociateIdChoiceMessage,dic,storage.CustomAmazonAssociateTag,false);
                    //    }
                    //},
                },
                new SettingItems(AppResources.SettingStoreTitle)
                {
                    new SettingItem(AppResources.SettingStorePreferPrintedTitle, (w) => storage.StorePreferPrintedBook?AppResources.SettingStorePreferPrintedDetailTrue:AppResources.SettingStorePreferPrintedDetailFalse,storage.StorePreferPrintedBook)
                    {
                        Action= (s) =>
                        {
                            storage.StorePreferPrintedBook=s.BoolValue;
                            return Task.CompletedTask;
                        }
                    },
                    new SettingItem(AppResources.SettingStoreStoreTitle, AppResources.SettingStoreStoreDetail)
                    {
                        Action=async (s) =>
                        {
                            storage.StoreOpenBookLink=await GetByActionSheet<string>(AppResources.SettingStoreStoreChoiceMessage,
                                AppResources.SettingStoreStoreChoiceChoices
                                .Split(new[]{ "\n\n" },StringSplitOptions.None)
                                .ToDictionary(a=>a.Split(new[]{'\n' },2)[0],a=>a.Split(new[]{'\n' },2)[1])
                                ,storage.StoreOpenBookLink);
                        }
                    },
                    new SettingItem(AppResources.SettingStoreEnableImpressTitle, (w) => storage.EnableImpressBookFeature?AppResources.SettingStoreEnableImpressDetailTrue:AppResources.SettingStoreEnableImpressDetailFalse,storage.EnableImpressBookFeature)
                    {
                        Action= (s) =>
                        {
                            storage.EnableImpressBookFeature=s.BoolValue;
                            return Task.CompletedTask;
                        }
                    },
                },
#if DEBUG
                new SettingItems(AppResources.SettingDebugTitle)
                {
                    new SettingItem(AppResources.SettingDebugInitTutorialTitle, AppResources.SettingDebugInitTutorialDetail){
                        Action=async (s) =>
                        {
                            WordbookImpressLibrary.Storage.TutorialStorage.SetTutorialCompleted(false);
                            await DisplayAlert(AppResources.SettingDebugInitTutorialAlertTitle,String.Format(AppResources.SettingDebugInitTutorialAlertMessage, WordbookImpressLibrary.Storage.TutorialStorage.Path),AppResources.AlertConfirmed);
                        }
                    },
                    new SettingItem(AppResources.SettingDebugDemoCalendarTitle, AppResources.SettingDebugDemoCalendarDetail,storage.DemoModeCalendar){
                        Action=(s) =>
                        {
                            storage.DemoModeCalendar=s.BoolValue;return Task.CompletedTask;
                        }
                    },
                },
#endif
                new SettingItems(AppResources.SettingInitTitle)
                {
                    new SettingItem(AppResources.SettingInitWordbookName, AppResources.SettingInitWordbookDetail)
                    {
                        Action=async (s) =>
                        {
                            if( await DisplayAlert(AppResources.WordWarning, String.Format(AppResources.SettingInitConfirmation, AppResources.SettingInitWordbookName.ToLower()), AppResources.AlertYes, AppResources.AlertNo))
                                WordbookImpressLibrary.Storage.WordbooksImpressStorage.Init();
                        }
                    },
                    new SettingItem(AppResources.SettingInitAuthName, AppResources.SettingInitAuthDetail)
                    {
                        Action=async (s) =>
                        {
                            if( await DisplayAlert(AppResources.WordWarning, String.Format(AppResources.SettingInitConfirmation, AppResources.SettingInitAuthName.ToLower()), AppResources.AlertYes, AppResources.AlertNo))
                                WordbookImpressLibrary.Storage.WordbooksImpressInfoStorage.Init();
                        }
                    },
                    new SettingItem(AppResources.SettingInitStoreStatisticsName, AppResources.SettingInitStoreStatisticsDetail)
                    {
                        Action=async (s) =>
                        {
                            if( await DisplayAlert(AppResources.WordWarning, String.Format(AppResources.SettingInitConfirmation, AppResources.SettingInitStoreStatisticsName.ToLower()), AppResources.AlertYes, AppResources.AlertNo))
                                WordbookImpressLibrary.Storage.PurchaseHistoryStorage.Init();
                        }
                    },
                    new SettingItem(AppResources.SettingInitStatisticsName, AppResources.SettingInitStatisticsDetail)
                    {
                        Action=async (s) =>
                        {
                            if( await DisplayAlert(AppResources.WordWarning, String.Format(AppResources.SettingInitConfirmation, AppResources.SettingInitStatisticsName.ToLower()), AppResources.AlertYes, AppResources.AlertNo))
                                WordbookImpressLibrary.Storage.WordbooksImpressInfoStorage.Init();
                        }
                    },
                },
                new SettingItems(AppResources.SettingSupportTitle)
                {
                    new SettingItem(AppResources.SettingSupportTutorialTitle, AppResources.SettingSupportTutorialDetail){
                        Action=async (s) =>
                        {
                            var page= new TutorialsPage();
                            NavigationPage.SetHasNavigationBar(page,false);
                            await Navigation.PushAsync(page);
                        },
                    },
                    new SettingItem(AppResources.SettingSupportWikiTitle,AppResources.SettingSupportWikiDetail){
                        Action = (s) =>{ try{ Device.OpenUri(new Uri(AppResources.ProfileAppWikiUrl)); }catch{ } return Task.CompletedTask; }
                    },
                },
                new SettingItems(String.Format( AppResources.SettingAboutTitle,nameof(WordbookImpressApp)))
                {
                    new SettingItem(AppResources.SettingAboutLicenseOSSTitle, AppResources.SettingAboutLicenseOSSDetail){ Children=licenseChildren},
                    new SettingItem(AppResources.SettingAboutLicenseAppTitle, AppResources.SettingAboutLicenseAppDetail){
                        Action = async (a) =>
                        {
                            await Navigation.PushAsync(new LicenseInfoPage(new WordbookImpressLibrary.Models.License.NormalLicense(){
                                LicenseText =await Storage.LicenseStorage.LoadLicenseText(nameof(WordbookImpressApp))
                                ,Name=nameof(WordbookImpressApp)
                                ,ProjectName=nameof(WordbookImpressApp)
                                ,LicenseUrl=AppResources.ProfileAppLicenseUrl
                            }));
                        }
                    },
					new SettingItem(AppResources.SettingOpenPrivacyPolicyTitle, AppResources.SettingOpenPrivacyPolicyDetail,null){ Action= (w)=>{try{ Device.OpenUri(new Uri(AppResources.SettingOpenPrivacyPolicyUri)); }catch{ } return Task.CompletedTask; } },
					new SettingItem(AppResources.SettingAboutProjectPage, AppResources.ProfileAppProjectUrl,null){ Action= (w)=>{try{ Device.OpenUri(new Uri(AppResources.ProfileAppProjectUrl)); }catch{ } return Task.CompletedTask; } },
                },
            };
			
			MyListView.ItemsSource = Items;
        }

        public static ConfigPage GetLicensePage(INavigation navigation)
        {
            return new ConfigPage(GetLicenseChildren(navigation));
        }

        public static ObservableCollection<SettingItems> GetLicenseChildren(INavigation navigation)
        {
            var licenseChildren = new ObservableCollection<SettingItems>();
            {
                var licenseChildrenDic = new Dictionary<string, List<SettingItem>>();
                var datas = Storage.LicenseStorage.NugetDatas;
                foreach (var item in datas)
                {
                    if (!licenseChildrenDic.ContainsKey(item.ProjectName)) licenseChildrenDic.Add(item.ProjectName, new List<SettingItem>());
                    licenseChildrenDic[item.ProjectName].Add(new SettingItem(item.Name, item.Version)
                    {
                        Action = async (a) =>
                        {
                            await navigation.PushAsync(new LicenseInfoPage(item));
                        }
                    });
                }
                foreach (var item in licenseChildrenDic)
                {
                    licenseChildren.Add(new SettingItems(item.Value, item.Key));
                }
            }
            return licenseChildren;
        }

        public async Task<T> GetByActionSheet<T>(string Message, Dictionary<string, T> Options, T currentValue,bool isFirstSpecial=false, Func<object, bool> IsValid = null, WordbookImpressLibrary.ViewModels.EntryWithOptionViewModel.IValueConverter converter=null, Keyboard keyboard=null)
        {
            var w = new List<string>();
            string Cancel = AppResources.AlertCancel;
            string Custom = AppResources.AlertCustom;
            var options = new ObservableCollection<WordbookImpressLibrary.ViewModels.EntryWithOptionViewModel.EntryWithOptionViewModelEntry>();
            foreach (var item in Options) {
                w.Add(item.Key);
                options.Add(new WordbookImpressLibrary.ViewModels.EntryWithOptionViewModel.EntryWithOptionViewModelEntry(item.Key, item.Value));
            }
            if (isFirstSpecial && Options.Count > 0) options[0].IsSpecialValue = true;
            w.Add(Custom);
            var result = await DisplayActionSheet(Message, Cancel, null, w.ToArray());
            if (string.IsNullOrEmpty(result) || result == Cancel) return currentValue;
            if (result == Custom)
            {
                var vm = (new WordbookImpressLibrary.ViewModels.EntryWithOptionViewModel(Message, options, currentValue, IsValid, converter));
                var page = new EntryWithOptionPage(vm);
                var waitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
                page.Disappearing += (s, e) => waitHandle.Set();

                Keyboard keyboard_result = Keyboard.Plain;
                if (keyboard != null)
                {
                    keyboard_result = keyboard;
                }
                else if(typeof(T) == typeof(Int16) || typeof(T) == typeof(Int32) || typeof(T) == typeof(Int64))
                {
                    keyboard_result = Keyboard.Numeric;
                }
                else if (typeof(T) == typeof(float) || typeof(T) == typeof(double))
                {
                    keyboard_result = Keyboard.Numeric;
                }
                page.Keyboard = keyboard_result;

                await Navigation.PushAsync(page);
                await Task.Run(() => waitHandle.WaitOne());
                var tmp = vm.GetValue<T>();
                if (tmp.Item2) return tmp.Item1; else return currentValue;
            }
            else
            {
                return Options[result];
            }
        }

        public class PercentageValueConverter : WordbookImpressLibrary.ViewModels.EntryWithOptionViewModel.IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if(value is double) { return (((double)value) * 100).ToString(); }
                return 0.ToString();
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if(value is string)
                {
                    double result;
                    if(double.TryParse((string)value, out result))
                    {
                        return result/100.0;
                    }
                    return null;
                }
                return null;
            }
        }

        public class TimeSpanTicksValueConverter : WordbookImpressLibrary.ViewModels.EntryWithOptionViewModel.IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is long) { return new TimeSpan((long)value).ToString(); }
                return "";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string)
                {
                    TimeSpan timeSpan;
                    if (TimeSpan.TryParse((string)value, out timeSpan))
                    {
                        return timeSpan.Ticks;
                    }
                    return null;
                }
                return null;
            }
        }


        public class TimeSpanValueConverter : WordbookImpressLibrary.ViewModels.EntryWithOptionViewModel.IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is TimeSpan) { return ((TimeSpan)value).ToString(); }
                return "";
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string)
                {
                    TimeSpan timeSpan;
                    if(TimeSpan.TryParse((string)value, out timeSpan))
                    {
                        return timeSpan;
                    }
                    return null;
                }
                return null;
            }
        }

        public class SettingItems : ObservableCollection<SettingItem>
        {
            public string Title { get; set; }

            public SettingItems(string Title = "") { this.Title = Title; }
            public SettingItems(IEnumerable<SettingItem> items, string Title = "") : base(items) { this.Title = Title; }
        }

        public class SettingItem:WordbookImpressLibrary.ViewModels.BaseViewModel
        {
            private string text = string.Empty;
            public string Text { get => text; set => SetProperty(ref text, value); }
            private string detail;
            public string Detail { get => detail ?? DetailFunc?.Invoke(this) ?? ""; set => SetProperty(ref detail, value); }
            private Func<SettingItem, string> detailFunc;
            public Func<SettingItem, string> DetailFunc { get => detailFunc; set { SetProperty(ref detailFunc, value); OnPropertyChanged(nameof(Detail)); } }

            public bool BoolSetting { get => boolValue != null; }
            private bool? boolValue=null;
            public bool BoolValue { get => boolValue ?? false; set
                {
                    //Note: CachingStrategy="RecycleElement"にしておくと、IsVisibleのバインディングが評価される前に表示されているSwitchのIsToggledが往復してBoolSettingがtrueになってしまうようだ。
                    //Note: そんなん分からん。再現しづらいから気を付けよう。
                    if (boolValue == null)
                        return;
                    SetProperty(ref boolValue, value); OnPropertyChanged(nameof(Detail)); Action(this);
                } }
            private Func<SettingItem,Task> action;
            public Func<SettingItem,Task> Action { get => action; set => SetProperty(ref action, value); }
            private ObservableCollection<SettingItems> children;
            public ObservableCollection<SettingItems> Children { get => children; set => SetProperty(ref children, value); }

            public SettingItem(string Text, Func<SettingItem, string> Detail, bool? SwitchStatus = null)
            {
                this.Text = Text;
                this.DetailFunc = Detail;
                this.boolValue = SwitchStatus;
            }

            public SettingItem(string Text, string Detail, bool? SwitchStatus = null)
            {
                this.Text = Text;
                this.Detail = Detail;
                this.boolValue = SwitchStatus;
            }

            public void SettingUpdate()
            {
                OnPropertyChanged(nameof(Detail));
            }
        }

        private bool Pushing = false;
        private async void MyListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (Pushing) return;
            ((ListView)sender).SelectedItem = null;

            SettingItem item;
            if ((item = e.SelectedItem as SettingItem) == null) { return; }
            Pushing = true;
            if (item.Action != null) await item.Action?.Invoke(item);
            if (item.BoolSetting) item.BoolValue = !item.BoolValue;
            if (item.Children != null) {
                var page = new ConfigPage(item.Children);
                page.Disappearing += (s, ev) => item.SettingUpdate();
                await Navigation.PushAsync(page, true); }
            item.SettingUpdate();
            Pushing = false;
        }
    }
}
