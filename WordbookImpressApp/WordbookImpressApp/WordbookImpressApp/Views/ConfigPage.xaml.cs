using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.Runtime.CompilerServices;
using System.Collections.Generic;


namespace WordbookImpressApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfigPage : ContentPage
    {
        public ObservableCollection<SettingItems> Items { get; set; }

        public bool SaveOnDisappearing { get; set; } = false;

        protected override void OnDisappearing()
        {
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

            var storage = WordbookImpressLibrary.Storage.ConfigStorage.Content;
            Items = new ObservableCollection<SettingItems>
            {
                new SettingItems("クイズ設定")
                {
                    new SettingItem("選択肢", (w)=> "選択肢の数は"+storage.ChoiceCount+"個です。"){
                        Action=async (s)=>{
                           storage.ChoiceCount=await GetByActionSheet<int>("選択肢の数を選択してください。","キャンセル"
                               ,new Dictionary<string, int>{{ "2択",2}, { "3択", 3 }, { "4択", 4 }, { "5択", 5 }, { "6択", 6 }, { "7択", 7 }, { "8択", 8 }, }
                               ,storage.ChoiceCount);
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
                           storage.SkipMinCorrect=await GetByActionSheet<int>("スキップする正解数を選択してください。","キャンセル"
                               ,new Dictionary<string, int>{{ "スキップしない",int.MaxValue},{ "1回",1}, { "2回", 2 }, { "3回", 3 }, { "5回", 5 }, { "10回", 10 } }
                               ,storage.SkipMinCorrect);
                        }
                    },
                    new SettingItem("正解率", (w)=>storage.SkipMinRate==2?"正解率に関わらず出題します。": Math.Floor( storage.SkipMinRate*100)+ "%正解した単語をスキップします。"){
                        Action=async (s)=>{
                           storage.SkipMinRate=await GetByActionSheet<double>("スキップする正解率を選択してください。","キャンセル"
                               ,new Dictionary<string, double>{{ "スキップしない",2},{ "50%",0.5}, { "60%", 0.6 }, { "75%", 0.75 }, { "90%", 0.9 }, { "100%", 1.0 } }
                               ,storage.SkipMinRate);
                        }
                    },
                },
                new SettingItems("WordbookImpressについて")
                {
                    new SettingItem("ライセンス", ""),
                    new SettingItem("プロジェクトページ", "https://github.com/kurema/wordbookImpressApp"){ Action=async (w)=>{Device.OpenUri(new Uri("https://github.com/kurema/wordbookImpressApp/")); } },
                },
            };
			
			MyListView.ItemsSource = Items;
        }

        public async Task<T> GetByActionSheet<T>(string Message, string Cancel, Dictionary<string, T> Options, T currentValue)
        {
            var w = new List<string>();
            foreach (var item in Options) { w.Add(item.Key); }
            var result = await DisplayActionSheet(Message, Cancel, null, w.ToArray());
            if (string.IsNullOrEmpty(result) || result == Cancel) return currentValue;
            return Options[result];
        }

        public class SettingItems : ObservableCollection<SettingItem>
        {
            public string Title { get; set; }

            public SettingItems(string Title = "") { this.Title = Title; }
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
            public bool BoolValue { get => boolValue ?? false; set { SetProperty(ref boolValue, value); OnPropertyChanged(nameof(Detail));Action(this); } }
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
            if (item.Children != null) await Navigation.PushAsync(new ConfigPage(item.Children),true);
            item.SettingUpdate();
            Pushing = false;
        }
    }
}
