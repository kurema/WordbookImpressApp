using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.ComponentModel;
using System.Collections.ObjectModel;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MasterPage : ContentPage
	{
		public MasterPage ()
		{
			InitializeComponent ();

            this.BindingContext = new MasterViewModel();
		}

        public class MasterViewModel
        {
            public ObservableCollection<MasterMenuItem> MenuItems { get; }
            public MasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterMenuItem>(new[] {
                    new MasterMenuItem { TargetType=typeof(Views.WordbooksPage), Title = "単語帳",Description="登録済み単語帳",Icon="icon_wordbook.png" },
                    new MasterMenuItem { Title = "総合単語帳",Description="全ての単語帳を総合",Icon="icon_wordbook.png"  ,Replace=false,Action=async (p)=>{
                        await ((NavigationPage)p.Detail).PushAsync(new WordbookPage(new WordbookImpressLibrary.ViewModels.WordbookImpressViewModel(WordbookImpressLibrary.Storage.WordbooksImpressStorage.Content.ToArray(),WordbookImpressLibrary.Storage.RecordStorage.Content,"総合単語帳")));
                    }  },
                    new MasterMenuItem { Title = "成績表",Icon="icon_statistics_circle.png",Description="テスト結果を表示" ,Replace=false,Action=async (p)=>{
                        await ((NavigationPage)p.Detail).PushAsync(new TestStatusPage(){
                        BindingContext=new WordbookImpressLibrary.ViewModels.TestStatusesViewModel()
                        {  Target=new WordbookImpressLibrary.ViewModels.WordbookImpressViewModel(WordbookImpressLibrary.Storage.WordbooksImpressStorage.Content.ToArray(),WordbookImpressLibrary.Storage.RecordStorage.Content,"全体成績")} });
                    }  },
                    new MasterMenuItem { TargetType=typeof(StorePage),Title = "ストア",Description="書籍を購入" ,Icon="icon_store.png",Replace=false},
                    new MasterMenuItem { TargetType=typeof(ConfigPage), Title = "設定",Description="単語帳の設定" ,Replace=false,Icon="icon_config.png" },
                    new MasterMenuItem { TargetType=typeof(DeveloperInfoPage), Title = "kuremaについて",SimpleItem=true,Replace=false },
                });
            }
        }

        public class MasterMenuItem
        {
            public string Icon { get; set; } = "";
            public string Title { get; set; }
            public string Description { get; set; }
            public bool SimpleItem { get; set; } = false;

            public Type TargetType { get; set; }
            public Action<MasterDetailPage> Action { get; set; }
            public bool Replace { get; set; } = true;
        }

        private void ListViewMenuItems_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (!(e.SelectedItem is MasterMenuItem)) { return; }
            var item = (MasterMenuItem)e.SelectedItem;
            if (sender is ListView)
            {
                ((ListView)sender).SelectedItem = null;
            }
            if (this.Parent is MasterDetailPage)
            {
                if(((MasterDetailPage)this.Parent).Detail is NavigationPage)
                {
                    if (item == null) { }
                    else
                    {
                        if (item.TargetType != null)
                        {
                            if (item.Replace)
                                ((MasterDetailPage)this.Parent).Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                            else
                                ((NavigationPage)(((MasterDetailPage)this.Parent).Detail)).PushAsync((Page)Activator.CreateInstance(item.TargetType));
                        }
                        item.Action?.Invoke((MasterDetailPage)this.Parent);
                        ((MasterDetailPage)this.Parent).IsPresented=false;
                    }
                }
            }

        }
    }
}