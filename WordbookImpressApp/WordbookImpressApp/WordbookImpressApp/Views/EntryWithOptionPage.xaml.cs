using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressLibrary.ViewModels;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EntryWithOptionPage : ContentPage
	{
        public EntryWithOptionPage ()
		{
			InitializeComponent ();
		}

        public void SetViewModel<T>(EntryWithOptionViewModel<T> model)
        {
            this.BindingContext = model;
            this.T = typeof(T);
        }

        public Type T;

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
        }
    }
}