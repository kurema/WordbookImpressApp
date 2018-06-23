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

        public class MasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MasterMenuItem> MenuItems { get; }
            public MasterViewModel()
            {
                MenuItems = new ObservableCollection<MasterMenuItem>(new[] {
                    new MasterMenuItem { TargetType=typeof(Views.WordbooksPage), Title = "単語帳",Description="登録済み単語帳" },
                    new MasterMenuItem { TargetType=typeof(Views.WordbooksPage), Title = "ストア",Description="書籍を購入" },
                });
            }
            public event PropertyChangedEventHandler PropertyChanged;
        }


        public class MasterMenuItem
        {
            public string Title { get; set; }
            public string Description { get; set; }

            public Type TargetType { get; set; }
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
                        ((MasterDetailPage)this.Parent).Detail = new NavigationPage((Page)Activator.CreateInstance(item.TargetType));
                        //((NavigationPage)(((MasterDetailPage)this.Parent).Detail)).PushAsync((Page)Activator.CreateInstance(item.TargetType));
                        ((MasterDetailPage)this.Parent).IsPresented=false;
                    }
                }
            }

        }
    }
}