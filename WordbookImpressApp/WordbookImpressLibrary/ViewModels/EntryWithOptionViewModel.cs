using System;
using System.Collections.Generic;
using System.Text;

using System.Collections.ObjectModel;

namespace WordbookImpressLibrary.ViewModels
{
    public class EntryWithOptionViewModel<T> : BaseViewModel
    {
        private T content;
        public T Content { get => content; set { SetProperty(ref content, value); OnPropertyChanged(nameof(ContentAsString)); } }

        public string ContentAsString
        {
            get => Content.ToString();
            set
            {
                T result = Content;
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
                }
                if (IsValid == null || IsValid(result)) Content = result;
            }
        }

        private string message;
        public string Message { get => message; set => SetProperty(ref message, value); }
        private ObservableCollection<EntryWithOptionViewModelEntry> options;
        public ObservableCollection<EntryWithOptionViewModelEntry> Options { get => options; set => SetProperty(ref options, value); }
        private Func<T, bool> isValid;
        public Func<T, bool> IsValid { get => isValid; set => SetProperty(ref isValid, value); }
        private T value;
        public T Value { get => value; set => SetProperty(ref this.value, value); }

        public EntryWithOptionViewModel(string Message, ObservableCollection<EntryWithOptionViewModelEntry> Options, T Initial, Func<T, bool> IsValid = null)
        {
            this.Message = Message;
            this.Options = Options;
            this.Value = Initial;
            this.IsValid = IsValid;
        }

        public class EntryWithOptionViewModelEntry : BaseViewModel
        {
            private T content;
            public T Content { get => content; set => SetProperty(ref content, value); }

            private string message;
            public string Message { get => message; set => SetProperty(ref message, value); }
            private bool isSpecialValue = false;
            public bool IsSpecialValue { get => isSpecialValue; set => SetProperty(ref isSpecialValue, value); }

            public EntryWithOptionViewModelEntry(string Message,T content)
            {
                this.Message = Message;
                this.Content = content;
            }
        }
    }
}
