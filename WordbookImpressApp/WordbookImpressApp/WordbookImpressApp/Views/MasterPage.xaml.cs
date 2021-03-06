﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.ComponentModel;
using System.Collections.ObjectModel;

using WordbookImpressApp.Resx;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MasterPage : ContentPage
	{
		public MasterPage ()
		{
			InitializeComponent ();

            this.BindingContext = new MasterViewModel();
            UpdateEnableImpress();
            WordbookImpressLibrary.Storage.ConfigStorage.Updated += (s, e) => UpdateEnableImpress();
		}

        public class MasterViewModel:INotifyPropertyChanged
        {
            private MasterMenuItem[] menuItemsOriginal;
            public event PropertyChangedEventHandler PropertyChanged;
            public MasterMenuItem[] MenuItems
            {
                get => menuItemsOriginal.Where(a => string.IsNullOrEmpty(a.Id) || (!ExcludedIds?.Contains(a.Id)?? true)).ToArray();
                set
                {
                    menuItemsOriginal = value;
                    OnPropertyChanged(nameof(MenuItems));
                    excludedIds = new string[0];
                    OnPropertyChanged(nameof(ExcludedIds));
                }
            }
            private string[] excludedIds;
            public string[] ExcludedIds { get => excludedIds; set { excludedIds = value; OnPropertyChanged(nameof(ExcludedIds));OnPropertyChanged(nameof(MenuItems)); } }

            protected void OnPropertyChanged(string name)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }

            public MasterViewModel()
            {
                menuItemsOriginal = new[] {
                    new MasterMenuItem { TargetType=typeof(Views.WordbooksPage), Title = AppResources.MasterWordbooksTitle,Description=AppResources.MasterWordbooksDetail,Icon="icon_wordbook.png" },
                    new MasterMenuItem { Title = AppResources.MasterWordbooksCombinedTitle,Description=AppResources.MasterWordbooksCombinedDetail,Icon="icon_wordbook.png"  ,Replace=false,Action=async (p)=>{
                        await ((NavigationPage)p.Detail).PushAsync(new WordbookPage(new WordbookImpressLibrary.ViewModels.WordbookImpressViewModel(WordbookImpressLibrary.Storage.WordbooksImpressStorage.Content.ToArray(),WordbookImpressLibrary.Storage.RecordStorage.Content,AppResources.WordbookCombinedTitle)));
                    }  },
                    new MasterMenuItem { Title = AppResources.MasterStatisticsTitle,Icon="icon_statistics_circle.png",Description=AppResources.MasterStatisticsDetail ,Replace=false,Action=async (p)=>{
                        await ((NavigationPage)p.Detail).PushAsync(new TestStatusPage(){
                        BindingContext=new WordbookImpressLibrary.ViewModels.TestStatusesViewModel()
                        {  Target=new WordbookImpressLibrary.ViewModels.WordbookImpressViewModel(WordbookImpressLibrary.Storage.WordbooksImpressStorage.Content.ToArray(),WordbookImpressLibrary.Storage.RecordStorage.Content,AppResources.WordbookCombinedTitle)} });
                    }  },
                    new MasterMenuItem { TargetType = typeof(StorePage), Title = AppResources.MasterStoreTitle, Description = AppResources.MasterStoreDetail, Icon = "icon_store.png", Replace = false ,Id="Store"},
                    new MasterMenuItem { TargetType=typeof(ConfigPage), Title = AppResources.MasterSettingTitle,Description=AppResources.MasterSettingDetail ,Replace=false,Icon="icon_config.png" },
                    new MasterMenuItem { TargetType=typeof(DeveloperInfoPage), Title = String.Format(AppResources.MasterDeveloperProfileTitle,AppResources.ProfileDeveloperName),SimpleItem=true,Replace=false },
                };
            }
        }

        public void UpdateEnableImpress()
        {
            var enabled = WordbookImpressLibrary.Storage.ConfigStorage.Content?.EnableImpressBookFeature;
            if (enabled == null) return;
            if (!(this.BindingContext is MasterViewModel model)) return;
            model.ExcludedIds = enabled.Value ? new string[0] : new string[] { "Store" };
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
            public string Id { get; set; } = null;

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