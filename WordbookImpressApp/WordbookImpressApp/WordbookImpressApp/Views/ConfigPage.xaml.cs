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
                new SettingItems("クイズ設定")
                {
                    new SettingItem("選択肢", (w)=> "選択肢の数は"+storage.ChoiceCount+"個です。"){
                        Action=async (s)=>{
                           storage.ChoiceCount=await GetByActionSheet<int>("選択肢の数を選択してください。"
                               ,new Dictionary<string, int>{{ "2択",2}, { "3択", 3 }, { "4択", 4 }, { "5択", 5 }, { "6択", 6 }, { "7択", 7 }, { "8択", 8 }, }
                               ,storage.ChoiceCount,false,(a)=>(a is int)&&(int)a>=2);
                        }
                    },
                },
                new SettingItems("クイズ出題条件")
                {
                    new SettingItem("記憶済み", (w)=>storage.SkipChecked?"クイズで記憶済みにチェックした単語をスキップします。":"クイズで記憶済みにチェックした単語も出題します。", ! storage.SkipChecked)
                    {
                        Action=async (s)=>{storage.SkipChecked=! s.BoolValue; }
                    },
                    new SettingItem("正解数",(w)=> storage.SkipMinCorrect==int.MaxValue?"正解数に関わらず出題します。": storage.SkipMinCorrect+ "回正解した単語をスキップします。"){
                        Action=async (s)=>{
                           storage.SkipMinCorrect=await GetByActionSheet<int>("スキップする正解数を選択してください。"
                               ,new Dictionary<string, int>{{ "スキップしない",int.MaxValue},{ "1回",1}, { "2回", 2 }, { "3回", 3 }, { "5回", 5 }, { "10回", 10 } }
                               ,storage.SkipMinCorrect,true,(a)=>(a is int)&&(int)a>=1);
                        }
                    },
                    new SettingItem("正解率", (w)=>storage.SkipMinRate==2 || storage.SkipMinRateMinTotal==int.MaxValue ? "正解率に関わらず出題します。": storage.SkipMinRateMinTotal+"問以上出題した結果"+ Math.Floor( storage.SkipMinRate*100)+ "%正解した単語をスキップします。"){
                        Children=new ObservableCollection<SettingItems>(){
                            new SettingItems("解説"){
                                new SettingItem("説明",(w)=>"この条件に関して説明します。"){
                                    Action=async (s) =>
                                    {
                                        await DisplayAlert("説明","出題問題数が少ない間はたまたま正解してしまい正解率が高くなってしまう事があります。\n適切な正解率を得るには一定回数の出題が必要です。\nまた基本もおろそかにしない事が大事です。\n以下の条件を片方でも満たさない場合には出題します。","了解");
                                    }
                                }
                            },
                            new SettingItems("正解率条件"){
                                new SettingItem("正解率",(w)=>storage.SkipMinRate==2?"正解率に関わらず出題します。":"正解率が"+Math.Floor( storage.SkipMinRate*100)+"%未満の場合は出題します。"){
                                    Action =async (s)=>{
                                        storage.SkipMinRate=await GetByActionSheet<double>("スキップする正解率を選択してください。"
                                            ,new Dictionary<string, double>{{ "スキップしない",2},{ "50%",0.5}, { "60%", 0.6 }, { "75%", 0.75 }, { "90%", 0.9 }, { "100%", 1.0 } },storage.SkipMinRate,true,(a)=>(a is double)&&(((double)a>0 &&(double)a<=1)||(double)a==2),new PercentageValueConverter());
                                    }
                                }
                                ,new SettingItem("出題数",(w)=>storage.SkipMinRateMinTotal==int.MaxValue?"出題数に関わらず出題します。":"出題数が"+storage.SkipMinRateMinTotal+"問未満の場合は出題します。"){
                                    Action =async (s)=>{
                                        storage.SkipMinRateMinTotal=await GetByActionSheet<int>("スキップに必要な出題数を選択してください。"
                                            ,new Dictionary<string, int>{{ "スキップしない",int.MaxValue},{ "3問",3}, { "5問", 5 }, { "10問", 10 }, { "20問", 20 }, { "50問", 50 } },storage.SkipMinRateMinTotal,true,(a)=>(a is int)&&(int)a>=1);
                                    }
                                }
                            }
                        }
                    },
                    new SettingItem("条件無視期間",(w)=>storage.SkipVoidTicks==-1?"条件を満たさない場合は出題しません。":"最後に正解してから"+ ValueConverters.TimeSpanFormatValueConverter.FormatTimeSpan(new TimeSpan(storage.SkipVoidTicks),@"[if:Days:[Days]日][if:Hours:[Hours]時間][if:Minutes:[Minutes]分][if:Seconds:[Seconds]秒]") + "経過した場合には通常通り出題します。")
                    {
                        Action=async (s) =>
                        {
                            storage.SkipVoidTicks=await GetByActionSheet<long>("条件によらず出題されるようになるまでの期間を指定してください。",
                                new Dictionary<string, long>{ { "出題しない",-1},{ "10分",TimeSpan.FromMinutes(10).Ticks},{ "1時間",TimeSpan.FromHours(1).Ticks},{ "1日",TimeSpan.FromDays(1).Ticks},{ "7日",TimeSpan.FromDays(7).Ticks} },storage.SkipVoidTicks,true,(a)=>(a is long)&&((long)a>=TimeSpan.FromSeconds(1).Ticks||(long)a==-1),new TimeSpanTicksValueConverter(),Keyboard.Plain);
                        }
                    }
                },
                new SettingItems("成績一覧")
                {
                    new SettingItem("一日当たりの回答数目安", (w) => "成績一覧ページのカレンダーグラフに反映されます。現在" + storage.MaxDailyTestCount+"問。")
                    {
                        Action=async (s) =>
                        {
                            storage.MaxDailyTestCount=await GetByActionSheet<int>("回答数目安を選択してください。",
                                new Dictionary<string, int>{ { "10問",10},{ "50問",50},{ "100問",100},{ "500問",500} },storage.MaxDailyTestCount);
                        }
                    },
                    new SettingItem("無回答を表示", (w) => storage.ShowStatisticsZeroAnswer?"無回答でも成績一覧に表示します。":"無回答の場合成績一覧に表示しません。",storage.ShowStatisticsZeroAnswer)
                    {
                        Action=async (s) =>
                        {
                            storage.ShowStatisticsZeroAnswer=s.BoolValue;
                        }
                    },
                    new SettingItem("Amazon アソシエイトID", "スコア共有時に使用されるAmazon アソシエイトIDを設定します。")
                    {
                        Action =async (s)=>{
                            var dic=new Dictionary<string,string>();
                            dic.Add("アプリ製作者のID",WordbookImpressLibrary.APIKeys.AmazonAssociateTag);
                            if(!string.IsNullOrWhiteSpace( storage.CustomAmazonAssociateTag) && storage.CustomAmazonAssociateTag!=WordbookImpressLibrary.APIKeys.AmazonAssociateTag) dic.Add(storage.CustomAmazonAssociateTag,storage.CustomAmazonAssociateTag);
                            storage.CustomAmazonAssociateTag=await GetByActionSheet<string>("Amazon アソシエイトIDを選択してください。",dic,storage.CustomAmazonAssociateTag,false);
                        }
                    },
                },
#if DEBUG
                new SettingItems("デバッグ")
                {
                    new SettingItem("チュートリアル初期設定", "チュートリアル完了ファイルを削除します。"){
                        Action=async (s) =>
                        {
                            WordbookImpressLibrary.Storage.TutorialStorage.SetTutorialCompleted(false);
                            await DisplayAlert("削除",WordbookImpressLibrary.Storage.TutorialStorage.Path+"を削除しました。","OK");
                        }
                    },

                },
#endif
                new SettingItems("サポート")
                {
                    new SettingItem("チュートリアル", "初回チュートリアルを表示します。"){
                        Action=async (s) =>
                        {
                            await Navigation.PushModalAsync(new TutorialsPage());
                        },
                    },
                    //new SettingItem("不具合報告", "不具合報告の選択肢を表示します。"){
                    //    Children=new ObservableCollection<SettingItems>(){
                    //        new SettingItems("不具合報告"){
                    //            new SettingItem("Github Issues","公式の不具合報告アドレス。"){
                    //                Action=async (s) =>Device.OpenUri(new Uri("https://github.com/kurema/WordbookImpressApp/issues"))
                    //            },
                    //            new SettingItem("メール","作者に報告"){
                    //                Action=async (s) =>Device.OpenUri(new Uri("kurema_makoto_software@yahoo.co.jp"]))
                    //            },
                    //        },
                    //    }
                    //},
                    new SettingItem("Wiki","このアプリのWikiを開きます。"){
                        Action =async (s) =>Device.OpenUri(new Uri("https://github.com/kurema/WordbookImpressApp/wiki"))
                    },
                },
                new SettingItems("WordbookImpressについて")
                {
                    new SettingItem("オープンソースライセンス", "オープンソースソフトウェアに関するライセンスの詳細"){ Children=licenseChildren},
                    new SettingItem("ライセンス", "このアプリのライセンスを表示。"){
                        Action = async (a) =>
                        {
                            await Navigation.PushAsync(new LicenseInfoPage(new WordbookImpressLibrary.Models.License.NormalLicense(){
                                LicenseText =await Storage.LicenseStorage.LoadLicenseText("WordbookImpress")
                                ,Name="WordbookImpress"
                                ,ProjectName="WordbookImpress"
                                ,LicenseUrl="https://github.com/kurema/WordbookImpressApp/blob/master/LICENSE"
                            }));
                        }
                    },
                    new SettingItem("プロジェクトページ", "https://github.com/kurema/wordbookImpressApp",null){ Action=async (w)=>{Device.OpenUri(new Uri("https://github.com/kurema/wordbookImpressApp/")); } },
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
                            //await DisplayAlert("ライセンス", item.LicenseText, "ok");
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
            string Cancel = "キャンセル";
            string Custom = "カスタム";
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
                return "0";
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

        private void Cancel_Clicked(object sender, EventArgs e)
        {

        }
    }
}
