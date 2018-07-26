using System;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressApp.Resx;

namespace WordbookImpressApp.Extensions
{
    [ContentProperty(nameof(Key))]
    public class TranslateExtension : IMarkupExtension
    {
        public CultureInfo CurrentCulture => CultureInfo.CurrentCulture;

        public string Key { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Key == null)
                return string.Empty;

            var translation = AppResources.ResourceManager.GetString(Key, CurrentCulture);
            if (translation == null)
            {
#if DEBUG
                translation = string.Format("Key '{0}' for '{1}'", Key, CurrentCulture.Name);
#else
				translation = Text;
#endif
            }
            return translation;
        }
    }
}
