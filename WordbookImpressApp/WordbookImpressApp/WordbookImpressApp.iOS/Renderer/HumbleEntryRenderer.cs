using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

using Xamarin.Forms.Platform.iOS;
using WordbookImpressApp.iOS.Renderer;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(WordbookImpressApp.Views.HumbleEntry), typeof(HumbleEntryRenderer))]
namespace WordbookImpressApp.iOS.Renderer
{
    public class HumbleEntryRenderer : EntryRenderer
    {
        public HumbleEntryRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.BorderStyle = UITextBorderStyle.None;
            }
        }
    }
}