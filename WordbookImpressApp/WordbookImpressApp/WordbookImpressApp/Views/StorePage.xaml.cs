using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressApp.Resx;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class StorePage : ContentPage
	{
		public StorePage ()
		{
			InitializeComponent ();

            PrepareStore();
        }

        public StackLayout AddFrame()
        {
            var frame = new Frame();
            frame.Style = Application.Current.Resources["CardStyle"] as Style;
            frame.Padding = 0;
            StoreStack.Children.Add(frame);
            StackLayout stack = new StackLayout();
            stack.Spacing = 0;
            frame.Content = stack;
            return stack;
        }

        public async void PrepareStore()
        {
            List<Task> tasks = new List<Task>();
            tasks.Add(AddSearchResult(AppResources.StoreItemsImpressTitle, AppResources.StoreItemsImpressSearchWord));
            tasks.Add(AddBooksWithSpecialNew(AppResources.StoreItemsSpecialLatest, action: async () => await Navigation.PushAsync(new SpecialInformationPage(SpecialInformationPage.GetGroupsByGenre()))));
            tasks.Add(AddBooksWithSpecialWordbook(AppResources.StoreItemsSpecialWordbook, action: async () => await Navigation.PushAsync(new SpecialInformationPage(SpecialInformationPage.GetGroupsByWordbooks())),showSpecialObtained:false));
            tasks.Add(AddBooksWithSpecialEbook(AppResources.StoreItemsSpecialEbook, action: async () => await Navigation.PushAsync(new SpecialInformationPage(SpecialInformationPage.GetGroupByGenreSpecialEbook())),showSpecialObtained:false ));

            var history = await WordbookImpressLibrary.Storage.PurchaseHistoryStorage.GetPurchaseHistory();
            if (history?.ClickedASIN?.Count() > 0)
            {
                var asin = history.ClickedASIN[(new Random().Next(history.ClickedASIN.Count()))];
                tasks.Add(AddRelated((w) =>
                {
                    var basic = AppResources.StoreItemsRelatedClicked;
                    var item = w?.Items?.Item;
                    if (item == null || item.Length == 0) return basic;
                    return item[0]?.ItemAttributes?.Title != null ? String.Format(AppResources.StoreItemsRelatedClickedTitle, item[0]?.ItemAttributes?.Title) : basic;
                }, asin));
            }

            tasks.Add(AddSearchResult(String.Format(AppResources.StoreItemsBookOfDeveloper,AppResources.ProfileDeveloperName), AppResources.ProfileDeveloperAccountsAmazonKindle));

            foreach(var task in tasks)
            {
                await task;
            }
        }

        public StackLayout AddTitleLabel(string title, StackLayout stack = null)
        {
            stack = stack ?? AddFrame();
            stack.Children.Add(new Label() { Text = title, TextColor = Color.Accent });
            return stack;
        }

        public async Task<StackLayout> AddBooksWithSpecialNew(string title, StackLayout stack = null, Action action = null)
        {
            try
            {
                return await AddBooksWithSpecial(title, WordbookImpressLibrary.Storage.RemoteStorage.WordbookSuggestion?.books?.book?.OrderByDescending(t => t.date_pushSpecified ? t?.date_push.Ticks : 0), stack, action);
            }
            catch { return null; }
        }

        public async Task<StackLayout> AddBooksWithSpecialEbook(string title, StackLayout stack = null, Action action = null, bool showWordbookObtained = true, bool showSpecialObtained = true)
        {
            return await AddBooksWithSpecialRandomOrder(title, (b) => b?.special?.ebook?.Count() > 0 == true && b?.obsolete != true, stack, action,showWordbookObtained,showSpecialObtained);
        }

        public async Task<StackLayout> AddBooksWithSpecialWordbook(string title, StackLayout stack = null, Action action = null, bool showWordbookObtained = true, bool showSpecialObtained = true)
        {
            return await AddBooksWithSpecialRandomOrder(title, (b) => b?.special?.wordbook?.Count() > 0 == true && b?.obsolete != true, stack, action, showWordbookObtained, showSpecialObtained);
        }

        public async Task<StackLayout> AddBooksWithSpecialRandomOrder(string title, Func<WordbookImpressLibrary.Schemas.WordbookSuggestion.infoBooksBook,bool> func,StackLayout stack=null,Action action=null
            ,bool showWordbookObtained= true, bool showSpecialObtained= true)
        {
            try
            {
                var rand = new Random();
                //読みづらいが、指定条件で検索したWordbookSuggestionの本の中をランダムで並び替え、できれば実本、ないならKindle本のASINを取得しています。
                var w2 = WordbookImpressLibrary.Storage.RemoteStorage.WordbookSuggestion?.books?.book?.Where(func);
                if (!showWordbookObtained) w2 = SpecialInformationPage.Group.ObtainedWordbookFilter(w2, false);
                if (!showSpecialObtained) w2 = SpecialInformationPage.Group.ObtainedSpecialFilter(w2,
                    await WordbookImpressLibrary.Storage.PurchaseHistoryStorage.GetPurchaseHistory()
                    , false);

                var w = w2?.OrderBy((t) => rand.Next());
                return await AddBooksWithSpecial(title, w, stack, action);
            }
            catch { return null; }
        }

        public async Task<StackLayout> AddBooksWithSpecial(string title, IOrderedEnumerable<WordbookImpressLibrary.Schemas.WordbookSuggestion.infoBooksBook> books, StackLayout stack = null, Action action = null)
        {
            try
            {
                var PreferPrintedBook = WordbookImpressLibrary.Storage.ConfigStorage.Content?.StorePreferPrintedBook ?? true;
                var w = books?.Select((b) => (b?.ids?.FirstOrDefault((id) => id.type == "ASIN" && id.binding == (PreferPrintedBook ? "printed_book" : "ebook")) ?? b?.ids?.FirstOrDefault((id) => id.type == "ASIN" && id.binding == (PreferPrintedBook ? "ebook" : "printed_book")))?.Value)?.Where((s) => s != null)?.ToArray();
                if (w?.Count() > 0)
                {
                    StoreItemsCardContentView storeItems;
                    stack = stack ?? AddFrame();
                    stack.Children.Add(storeItems = new StoreItemsCardContentView(title));

                    var res = await storeItems.StoreItems.AddASIN(w.ToArray());
                    if (action != null)
                    {
                        storeItems.ActionMore = () => { try { action(); } catch { } };
                    }
                    else if (res?.Items?.MoreSearchResultsUrl != null)
                    {
                        storeItems.ActionMore = () => { try { Device.OpenUri(new Uri(res?.Items?.MoreSearchResultsUrl)); } catch { } };
                    }
                    return stack;
                }
                else
                {
                    return null;
                }
            }
            catch { return null; }
        }

        public async Task<StackLayout> AddSearchResult(string title, string word, StackLayout stack = null)
        {
            try
            {
                StoreItemsCardContentView storeItems;
                stack = stack ?? AddFrame();
                stack.Children.Add(storeItems = new StoreItemsCardContentView(title));

                var res = await storeItems.StoreItems.AddSearchResult(word, Nager.AmazonProductAdvertising.Model.AmazonSearchIndex.Books);
                if (res?.Items?.MoreSearchResultsUrl != null)
                {
                    storeItems.ActionMore = () => { try { Device.OpenUri(new Uri(res?.Items?.MoreSearchResultsUrl)); } catch { } };
                }
                return stack;
            }
            catch { return null; }
        }

        public async Task<StackLayout> AddRelated(Func<Nager.AmazonProductAdvertising.Model.AmazonItemResponse, string> titleFunc,string ASIN, StackLayout stack = null)
        {
            try
            {
                StoreItemsCardContentView storeItems;
                stack = stack ?? AddFrame();
                stack.Children.Add(storeItems = new StoreItemsCardContentView());

                var res = await storeItems.StoreItems.AddRelaed(ASIN);
                storeItems.ItemsTitle = titleFunc?.Invoke(res.Item1) ?? "";
                if (res.Item2?.Items?.MoreSearchResultsUrl != null)
                {
                    storeItems.ActionMore = () => { try { Device.OpenUri(new Uri(res.Item2?.Items?.MoreSearchResultsUrl)); } catch { } };
                }
                return stack;
            }
            catch { return null; }
        }

        private async void ToolbarItem_Clicked_Attention(object sender, EventArgs e)
        {
            await DisplayAlert(AppResources.WordCaution, AppResources.StoreNoticeMessage, AppResources.AlertConfirmed);
        }

    }
}