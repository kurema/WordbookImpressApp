using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.ObjectModel;
using System.Globalization;

namespace WordbookImpressLibrary.ViewModels
{
    public class EntryWithOptionViewModel : BaseViewModel
    {
        private string contentAsString;
        public string ContentAsString
        {
            get => contentAsString;
            set => SetProperty(ref contentAsString, value);
        }

        public string GetString<T>(T value)
        {
            foreach (var item in this.Options)
            {
                if (item.IsSpecialValue && value.ToString() ==item.Content.ToString()) { return item.Message; }
            }
            if (ValueConverter != null)
            {
                return (string)ValueConverter.Convert(value, typeof(string), null, CultureInfo.CurrentCulture);
            }
            return value.ToString();
        }

        public (T,bool) GetValue<T>()
        {
            T result = default(T);//fixme

            foreach(var item in this.Options)
            {
                if( item.IsSpecialValue && ContentAsString == item.Message) { return ((T)item.Content,true); }
            }

            if (ValueConverter != null)
            {
                var tmp= ValueConverter.ConvertBack(ContentAsString, typeof(T), null, CultureInfo.CurrentCulture);
                if (tmp != null)
                {
                    var ans = (T)tmp;
                    if (IsValidChecker == null || IsValidChecker(ans)) return (ans,true);
                }
                if (this.Initial is T) return ((T)this.Initial,false);
                return (default(T),false);
            }
            var value = ContentAsString;
            try
            {
                var converter = System.ComponentModel.TypeDescriptor.GetConverter(typeof(T));
                if (converter != null)
                {
                    result = (T)converter.ConvertFromString(value);
                }
            }
            catch
            {
                if (this.Initial is T) return ((T)this.Initial,false);
            }
            if (IsValidChecker == null || IsValidChecker(result)) return (result,true);
            if (this.Initial is T) return ((T)this.Initial,false);
            return (default(T),false);
        }

        private string message;
        public string Message { get => message; set => SetProperty(ref message, value); }
        private ObservableCollection<EntryWithOptionViewModelEntry> options;
        public ObservableCollection<EntryWithOptionViewModelEntry> Options { get => options; set => SetProperty(ref options, value); }
        public Func<object, bool> IsValidChecker { get; private set; }
        public object Initial { get; private set; }
        public IValueConverter ValueConverter { get; private set; }

        public EntryWithOptionViewModel(string Message, ObservableCollection<EntryWithOptionViewModelEntry> Options, object Initial, Func<object, bool> IsValid = null, IValueConverter ValueConverter = null)
        {
            this.Message = Message;
            this.Options = Options;
            this.Initial = Initial;
            this.IsValidChecker = IsValid;
            this.ValueConverter = ValueConverter;
            this.ContentAsString = this.GetString(Initial);
        }

        public class EntryWithOptionViewModelEntry : BaseViewModel
        {
            private object content;
            public object Content { get => content; set => SetProperty(ref content, value); }

            private string message;
            public string Message { get => message; set => SetProperty(ref message, value); }
            private bool isSpecialValue = false;
            public bool IsSpecialValue { get => isSpecialValue; set => SetProperty(ref isSpecialValue, value); }

            public EntryWithOptionViewModelEntry(string Message, object content)
            {
                this.Message = Message;
                this.Content = content;
            }
        }

        public class DelegateValueConverter : IValueConverter
        {
            public Func<object, Type, object, CultureInfo, object> ConvertFunc;
            public Func<object, Type, object, CultureInfo, object> ConvertBackFunc;

            public DelegateValueConverter(Func<object, Type, object, CultureInfo, object> ConvertFunc, Func<object, Type, object, CultureInfo, object> ConvertBackFunc)
            {
                this.ConvertFunc = ConvertFunc;
                this.ConvertBackFunc = ConvertBackFunc;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ConvertFunc(value, targetType, parameter, culture);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return ConvertBackFunc(value, targetType, parameter, culture);
            }
        }

        public interface IValueConverter
        {
            object Convert(object value, Type targetType, object parameter, CultureInfo culture);
            object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);
        }
    }
}
