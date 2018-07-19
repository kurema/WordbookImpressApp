using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms.Platform.Android;
using WordbookImpressApp.Droid.Renderer;
using Xamarin.Forms;
using Android.Content.Res;
using Android.Graphics.Drawables;
using Android.Text;

[assembly: ExportRenderer(typeof(WordbookImpressApp.Views.HumbleEntry), typeof(HumbleEntryRenderer))]
namespace WordbookImpressApp.Droid.Renderer
{
    public class HumbleEntryRenderer : EntryRenderer
    {
        public HumbleEntryRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                GradientDrawable gd = new GradientDrawable();
                gd.SetColor(Android.Graphics.Color.Transparent);
                this.Control.Background = gd;
                //this.Control.SetRawInputType(InputTypes.TextFlagNoSuggestions);
            }
        }
    }
}