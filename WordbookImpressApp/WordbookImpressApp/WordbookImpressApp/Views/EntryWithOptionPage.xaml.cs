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
        public EntryWithOptionViewModel Model => this.BindingContext as EntryWithOptionViewModel;
        public Xamarin.Forms.Keyboard Keyboard
        {
            get => MainEntry.Keyboard;
            set => MainEntry.Keyboard = value;
        }

        public EntryWithOptionPage ()
		{
			InitializeComponent ();

            this.BindingContextChanged += EntryWithOptionPage_BindingContextChanged;
		}

        private void EntryWithOptionPage_BindingContextChanged(object sender, EventArgs e)
        {
            if (Model == null) return;
        }

        public EntryWithOptionPage(EntryWithOptionViewModel model):this()
        {
            this.BindingContext = model;
        }

        public Task<bool> WaitEntry()
        {
            var waitHandle = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.AutoReset);
            this.Disappearing += (s, e2) => waitHandle.Set();
            return Task.Run(() => waitHandle.WaitOne());
        }


        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as EntryWithOptionViewModel.EntryWithOptionViewModelEntry;
            if(Model != null && item != null)
            {
                Model.ContentAsString = Model.GetString(item.Content);
            }
            if(sender is ListView)
            {
                ((ListView)sender).SelectedItem = null;
            }
        }
    }
}