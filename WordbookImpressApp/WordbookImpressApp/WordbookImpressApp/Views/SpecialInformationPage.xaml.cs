using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressLibrary.Schemas.WordbookSuggestion;
using System.Collections.ObjectModel;
using System.Collections;

using System.ComponentModel;
using System.Collections.Specialized;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SpecialInformationPage : ContentPage
	{
        private Groups Model => this.BindingContext as Groups;

        public SpecialInformationPage ()
		{
			InitializeComponent ();
		}

        public SpecialInformationPage(Groups model):this()
        {
            this.BindingContext = model;
            //Model[0].Content[0].special
        }

        public static Groups GetGroupsByGenre(Func<infoBooksBook,bool> filter=null)
        {
            filter = filter ?? (a => true);
            return new Groups((WordbookImpressLibrary.Storage.RemoteStorage.WordbookSuggestion?.books?.book?.Where(filter)?.GroupBy(b => b.genre))?.Select(a => new Group() { Content = a.ToList(), Head = a.Key }));
        }

        public static Groups GetGroupByGenreSpecialEbook()
        {
            return GetGroupsByGenre((b) => b?.special?.ebook?.Count() > 0);
        }

        public static Groups GetGroupByGenreSpecialWordbook()
        {
            return GetGroupsByGenre((b) => b?.special?.wordbook?.Count() > 0);
        }

        public class Groups: ObservableCollection<Group>
        {
            private bool showObsolete = false;

            public Groups()
            {
            }

            public Groups(IEnumerable<Group> collection) : base(collection)
            {
            }

            public Groups(List<Group> list) : base(list)
            {
            }

            public bool ShowObsolete { get => showObsolete;
                set
                {
                    foreach(var item in this)
                    {
                        item.ShowObsolete = value;
                    }
                    showObsolete = value;
                }
            }
        }
        
        public class Group : IEnumerable<infoBooksBook>,System.Collections.Specialized.INotifyCollectionChanged,INotifyPropertyChanged
        {
            public List<infoBooksBook> Content = new List<infoBooksBook>();
            private bool showObsolete = false;
            public bool ShowObsolete { get => showObsolete; set {
                    if (ShowObsolete == value) return;
                    if (ShowObsolete == true) OnCollectionChanged(NotifyCollectionChangedAction.Remove, Content.Where((c) => c.obsolete == true).ToList());
                    if (ShowObsolete == false) OnCollectionChanged(NotifyCollectionChangedAction.Add, Content.Where((c) => c.obsolete == true).ToList());
                    SetProperty(ref showObsolete, value);
                } }

            private string head;
            public string Head { get => head; set => SetProperty(ref head, value); }

            public IEnumerator<infoBooksBook> GetEnumerator()
            {
                return Content.Where((s) => ShowObsolete || !s.obsolete).GetEnumerator();
            }


            #region INotifyPropertyChanged
            protected bool SetProperty<T>(ref T backingStore, T value,
                [System.Runtime.CompilerServices.CallerMemberName]string propertyName = "",
                System.Action onChanged = null)
            {
                if (System.Collections.Generic.EqualityComparer<T>.Default.Equals(backingStore, value))
                    return false;

                backingStore = value;
                onChanged?.Invoke();
                OnPropertyChanged(propertyName);
                return true;
            }
            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion

            public event NotifyCollectionChangedEventHandler CollectionChanged;
            protected void OnCollectionChanged(NotifyCollectionChangedAction action,IList list=null)
            {
                if (action == NotifyCollectionChangedAction.Replace)
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
                }
                else
                {
                    CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action,list));
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
            var item = e.SelectedItem as infoBooksBook;
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (Model?.ShowObsolete != null) Model.ShowObsolete = !Model.ShowObsolete;
        }
    }
}