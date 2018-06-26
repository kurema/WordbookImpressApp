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

            Items = new ObservableCollection<SettingItems>
            {
                new SettingItems("端末"){
                new SettingItem("描画フレームレート","描画に必要なフレームレートを設定します。"),
                new SettingItem("食事療法","食事療法を考えてみます。",true),
                new SettingItem("安定性","システムの安定度を調整します",true,true),
                },
                new SettingItems("単語帳"){
                new SettingItem("描画フレームレート","描画に必要なフレームレートを設定します。"){Children=
                new ObservableCollection<SettingItems>
            {
                new SettingItems("端末"){
                new SettingItem("描画フレームレート","描画に必要なフレームレートを設定します。"),
                new SettingItem("食事療法","食事療法を考えてみます。",true)
                } }
                },
                new SettingItem("食事療法","食事療法を考えてみます。",true),
                }
            };
			
			MyListView.ItemsSource = Items;
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
            private string detail = string.Empty;
            public string Detail { get => detail; set => SetProperty(ref detail, value); }
            private bool boolSetting=false;
            public bool BoolSetting { get => boolSetting; set => SetProperty(ref boolSetting, value); }
            private bool boolValue=false;
            public bool BoolValue { get => boolValue; set => SetProperty(ref boolValue, value); }
            private Action<SettingItem> action;
            public Action<SettingItem> Action { get => action; set => SetProperty(ref action, value); }
            private ObservableCollection<SettingItems> children;
            public ObservableCollection<SettingItems> Children { get => children; set => SetProperty(ref children, value); }

            public SettingItem(string Text, string Detail, bool SwitchEnabled = false,bool SwitchStatus = false)
            {
                this.Text = Text;
                this.Detail = Detail;
                this.BoolSetting = SwitchEnabled;
                this.BoolValue = SwitchStatus;
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
            item.Action?.Invoke(item);
            if (item.BoolSetting) item.BoolValue = !item.BoolValue;
            if (item.Children != null) await Navigation.PushAsync(new ConfigPage(item.Children),true);
            Pushing = false;
        }
    }
}
