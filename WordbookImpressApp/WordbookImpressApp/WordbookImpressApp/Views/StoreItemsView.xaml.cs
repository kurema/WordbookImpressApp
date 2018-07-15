using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;


using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StoreItemsView : ContentView
	{
        ////This is not bindable. If you want.
        ////https://github.com/xamarin/Xamarin.Forms/blob/56c89628d6a6b3f4ec778f288c7f6457d0a45dcb/Xamarin.Forms.Core/ItemsView.cs
        //public DataTemplate ItemTemplate { get; set; }

		public StoreItemsView ()
		{
			InitializeComponent ();

		}
	}
}