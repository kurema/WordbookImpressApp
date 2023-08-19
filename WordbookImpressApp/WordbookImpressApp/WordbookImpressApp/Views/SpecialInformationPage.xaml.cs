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

            WordbookImpressLibrary.Storage.PurchaseHistoryStorage.Updated += (s, e) => 
            Model?.ReloadStorage();

        }

        public SpecialInformationPage(Groups model):this()
        {
            this.BindingContext = model;
            //Model[0].Content[0].special
        }

        public static Groups GetGroupsByWordbooks()
        {
            return new Groups((WordbookImpressLibrary.Storage.RemoteStorage.WordbookSuggestion?.books?.book?.Where(b => b?.special?.wordbook?.Count() > 0)?.GroupBy(b => b.special.wordbook[0].@ref))?.Select(a => new Group() { Content = new ObservableCollection<infoBooksBook>(a), Head = WordbookImpressLibrary.Storage.RemoteStorage.WordbookSuggestion?.wordbooks?.FirstOrDefault(w => w?.id == a.Key)?.title?.FirstOrDefault() ?? "" }));
        }

        public static Groups GetGroupsByGenre(Func<infoBooksBook,bool> filter=null)
        {
            filter = filter ?? (a => true);
            return new Groups((WordbookImpressLibrary.Storage.RemoteStorage.WordbookSuggestion?.books?.book?.Where(filter)?.GroupBy(b => b.genre))?.Select(a => new Group() { Content = new ObservableCollection<infoBooksBook>(a), Head = a.Key })) { ShowObtainedWordbook = true };
        }

        public static Groups GetGroupByGenreSpecialEbook()
        {
            var result= GetGroupsByGenre((b) => b?.special?.ebook?.Count() > 0);
            result.ShowObtainedWordbook = true;
            return result;
        }

        public static Groups GetGroupByGenreSpecialWordbook()
        {
            return GetGroupsByGenre((b) => b?.special?.wordbook?.Count() > 0);
        }

        public class Groups: IEnumerable<Group>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            private ObservableCollection<Group> Content;
            private bool showObsolete = false;
            private bool showObtainedWordbook = false;
            private bool showObtainedSpecial = false;

            public Groups()
            {
            }

            public Groups(IEnumerable<Group> collection) 
            {
                Content = new ObservableCollection<Group>(collection);
            }

            public Groups(List<Group> list)
            {
                Content = new ObservableCollection<Group>(list);
            }

            private event NotifyCollectionChangedEventHandler CollectionChangedLocal;
            public event NotifyCollectionChangedEventHandler CollectionChanged
            {
                add
                {
                    ((INotifyCollectionChanged)Content).CollectionChanged += value;
                    CollectionChangedLocal += value;
                }

                remove
                {
                    ((INotifyCollectionChanged)Content).CollectionChanged -= value;
                    CollectionChangedLocal -= value;
                }
            }

            private void OnCollectionChangedLocal(NotifyCollectionChangedAction action,IList list=null)
            {
                CollectionChangedLocal?.Invoke(this, new NotifyCollectionChangedEventArgs(action, list));
            }

            public bool ShowObsolete
            {
                get => showObsolete;
                set
                {
                    if (ShowObsolete == value) return;
                    foreach (var item in Content)
                        item.ShowObsolete = value;
                    if (value == false) OnCollectionChangedLocal(NotifyCollectionChangedAction.Remove, Content.Where(a => a.Count() == 0).ToList());
                    if (value == true) OnCollectionChangedLocal(NotifyCollectionChangedAction.Add, Content.Where(a => a.Count() == 0).ToList());
                    SetProperty(ref showObsolete, value);
                }
            }

            public bool ShowObtainedWordbook
            {
                get => showObtainedWordbook; set
                {
                    if (showObtainedWordbook == value) return;
                    foreach (var item in Content)
                    {
                        item.ShowObtainedWordbook = value;
                        item.UpdateLazyObtainedWordbook();
                    }
                    if (value == false) OnCollectionChangedLocal(NotifyCollectionChangedAction.Remove, Content.Where(a => a.Count() == 0).ToList());
                    if (value == true) OnCollectionChangedLocal(NotifyCollectionChangedAction.Add, Content.Where(a => a.Count() == 0).ToList());
                    SetProperty(ref showObtainedWordbook, value);
                }
            }
            public bool ShowObtainedSpecial
            {
                get => showObtainedSpecial; set
                {
                    if (showObtainedSpecial == value) return;
                    foreach (var item in Content)
                    {
                        item.ShowObtainedSpecial = value;
                        var t = item.UpdateObtainedSpecialCache();
                    }
                    if (value == false) OnCollectionChangedLocal(NotifyCollectionChangedAction.Remove, Content.Where(a => a.Count() == 0).ToList());
                    if (value == true) OnCollectionChangedLocal(NotifyCollectionChangedAction.Add, Content.Where(a => a.Count() == 0).ToList());
                    SetProperty(ref showObtainedSpecial, value);
                }
            }

            public void ReloadStorage()
            {
                var a = this.ShowObsolete;
                var b = this.ShowObtainedWordbook;
                var c = this.ShowObtainedSpecial;
                this.ShowObsolete = true;
                this.ShowObtainedWordbook = true;
                this.ShowObtainedSpecial = true;
                this.ShowObsolete = a;
                this.ShowObtainedWordbook = b;
                this.ShowObtainedSpecial = c;
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
                PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }

            public IEnumerator<Group> GetEnumerator()
            {
                return Content.Where(a => a.Count() > 0).GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
            #endregion

        }
        
        public class Group : IEnumerable<infoBooksBook>, INotifyCollectionChanged, INotifyPropertyChanged
        {
            private ObservableCollection<infoBooksBook> _Content;
            public ObservableCollection<infoBooksBook> Content
            {
                get => _Content;
                set
                {
                    SetProperty(ref _Content, value);
                    var t=UpdateObtainedSpecialCache();
                    UpdateLazyObtainedWordbook();
                }
            }
            private bool showObsolete = false;
            private bool showObtainedWordbook = false;
            private bool showObtainedSpecial = false;
            public bool ShowObsolete { get => showObsolete; set {
                    if (ShowObsolete == value) return;
                    SetProperty(ref showObsolete, value);
                    if (value == false) OnCollectionChanged(NotifyCollectionChangedAction.Remove, Content.Where((c) => c.obsolete == true).ToList());
                    if (value == true) OnCollectionChanged(NotifyCollectionChangedAction.Add, Content.Where((c) => c.obsolete == true).ToList());
                } }
            public bool ShowObtainedWordbook { get => showObtainedWordbook; set {
                    if (ShowObtainedWordbook == value) return;
                    SetProperty(ref showObtainedWordbook, value);
                    if (value == false) OnCollectionChanged(NotifyCollectionChangedAction.Remove, LazyObtainedWordbook.Value.ToList());
                    if (value == true) OnCollectionChanged(NotifyCollectionChangedAction.Add, LazyObtainedWordbook.Value.ToList());
                }
            }

            //public async void ClearSpecialCache()
            //{
            //    UpdateLazyObtainedWordbook();
            //    await UpdateObtainedSpecialCache();
            //}

            public Lazy<infoBooksBook[]> LazyObtainedWordbook;

            public void UpdateLazyObtainedWordbook()
            {
                LazyObtainedWordbook = new Lazy<infoBooksBook[]>(() => ObtainedWordbookFilter(Content, true)?.ToArray());
            }

            public bool ShowObtainedSpecial { get => showObtainedSpecial; set {
                    if (ShowObtainedSpecial == value) return;
                    SetProperty(ref showObtainedSpecial, value);
                    if (value == false) OnCollectionChanged(NotifyCollectionChangedAction.Remove, ObtainedSpecialCache.ToList());
                    if (value == true) OnCollectionChanged(NotifyCollectionChangedAction.Remove, ObtainedSpecialCache.ToList());
                }
            }

            public static IEnumerable<infoBooksBook> ObtainedWordbookFilter(IEnumerable<infoBooksBook> content, bool q) =>
                content?.Where(c => q == c.special?.wordbook
                          ?.Any(d => true == WordbookImpressLibrary.Storage.RemoteStorage.GetWordbookByRef(d?.@ref)
                          ?.Any(e => true == WordbookImpressLibrary.Storage.WordbooksImpressInfoStorage.Content
                          ?.Any(g => g.Url == e?.access?.url))));

            public static IEnumerable<infoBooksBook> ObtainedSpecialFilter (IEnumerable<infoBooksBook> content, WordbookImpressLibrary.Models.PurchaseHistory history, bool q) => 
                content
                    ?.Where(a => q == a.links?.Where(b => b.type == "impress")
                    ?.Any(d => true == history.SpecialObtainedUrl
                    ?.Any(e => (d.@ref?.ToLower() == e.ToLower()))));


            private infoBooksBook[] _ObtainedSpecialCache;
            public infoBooksBook[] ObtainedSpecialCache
            {
                get => _ObtainedSpecialCache; set
                {
                    var oldCache = _ObtainedSpecialCache;
                    SetProperty(ref _ObtainedSpecialCache, value);
                    if (ShowObtainedSpecial) return;
                    OnCollectionChanged(NotifyCollectionChangedAction.Add, oldCache?.ToList());
                    OnCollectionChanged(NotifyCollectionChangedAction.Remove, value?.ToList());
                }
            }
            public async Task UpdateObtainedSpecialCache()
            {
                var history = await WordbookImpressLibrary.Storage.PurchaseHistoryStorage.GetPurchaseHistory();
                ObtainedSpecialCache = ObtainedSpecialFilter(Content, history, true)?.ToArray();
            }

            public Group()
            {
                UpdateLazyObtainedWordbook();
            }

            private string head;
            public string Head { get => head; set => SetProperty(ref head, value); }

            public IEnumerator<infoBooksBook> GetEnumerator()
            {
                return Content.Where((s) => (ShowObsolete || !s.obsolete)
                && (ShowObtainedWordbook || (LazyObtainedWordbook?.Value?.Contains(s) == false))
                && (ShowObtainedSpecial || (ObtainedSpecialCache?.Contains(s) == false))).GetEnumerator();
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

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
            var item = e.SelectedItem as infoBooksBook;
            if (item == null) return;
            //await Navigation.PushAsync(new SpecialInformationItemPage(new WordbookImpressLibrary.ViewModels.BookInformationViewModel(item)));
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            if (Model?.ShowObsolete != null) Model.ShowObsolete = !Model.ShowObsolete;
        }

        private void ToolbarItem_Clicked_1(object sender, EventArgs e)
        {
            if (Model?.ShowObsolete != null) Model.ShowObtainedWordbook = !Model.ShowObtainedWordbook;
        }

        private void ToolbarItem_Clicked_2(object sender, EventArgs e)
        {
            if (Model?.ShowObsolete != null) Model.ShowObtainedSpecial = !Model.ShowObtainedSpecial;
        }
    }
}