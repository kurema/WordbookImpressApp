using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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
            StackLayout stack = null;
            //stack = AddTitleLabel("インプレスブックス", stack);
            stack= await AddSearchResult("インプレスブックスの本", "インプレス");
            stack = await AddBooksWithSpecialNew("最近追加された特典本", action: async () => await Navigation.PushAsync(new SpecialInformationPage(SpecialInformationPage.GetGroupsByGenre())));
            //stack = await AddBooksWithSpecialWordbook("単語帳が手に入る本", action: async () => await Navigation.PushAsync(new SpecialInformationPage(SpecialInformationPage.GetGroupByGenreSpecialWordbook())));
            stack = await AddBooksWithSpecialWordbook("単語帳が手に入る本", action: async () => await Navigation.PushAsync(new SpecialInformationPage(SpecialInformationPage.GetGroupsByWordbooks())));
            stack = await AddBooksWithSpecialEbook("本文PDFが手に入る本", action: async () => await Navigation.PushAsync(new SpecialInformationPage(SpecialInformationPage.GetGroupByGenreSpecialEbook())));

            var history = await WordbookImpressLibrary.Storage.PurchaseHistoryStorage.GetPurchaseHistory();
            if (history.ClickedASIN.Count() > 0)
            {
                var asin = history.ClickedASIN[(new Random().Next(history.ClickedASIN.Count()))];
                await AddRelated((w)=> {
                    var basic = "以前クリックした本に関連";
                    var item = w?.Items?.Item;
                    if (item == null || item.Length == 0) return basic;
                    return item[0]?.ItemAttributes?.Title + "に関連" ?? basic;
                    }, asin);
            }

            stack = await AddSearchResult("kuremaの本", "B077X71C4C");
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

        public async Task<StackLayout> AddBooksWithSpecialEbook(string title, StackLayout stack = null, Action action = null)
        {
            return await AddBooksWithSpecialRandomOrder(title, (b) => b?.special?.ebook?.Count() > 0 == true && b?.obsolete != true, stack, action);
        }

        public async Task<StackLayout> AddBooksWithSpecialWordbook(string title, StackLayout stack = null, Action action = null)
        {
            return await AddBooksWithSpecialRandomOrder(title, (b) => b?.special?.wordbook?.Count() > 0 == true && b?.obsolete != true, stack, action);
        }

        public async Task<StackLayout> AddBooksWithSpecialRandomOrder(string title, Func<WordbookImpressLibrary.Schemas.WordbookSuggestion.infoBooksBook,bool> func,StackLayout stack=null,Action action=null)
        {
            try
            {
                var rand = new Random();
                //読みづらいが、指定条件で検索したWordbookSuggestionの本の中をランダムで並び替え、できれば実本、ないならKindle本のASINを取得しています。
                var w = WordbookImpressLibrary.Storage.RemoteStorage.WordbookSuggestion?.books?.book
                    ?.Where(func)
                    ?.OrderBy((t) => rand.Next());
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
            await DisplayAlert("注意", "本アプリケーション内で表示されるコンテンツの一部は、アマゾンジャパン合同会社またはその関連会社により提供されたものです。これらのコンテンツは「現状有姿」で提供されており、随時変更または削除される場合があります。\n"+
                "価格および発送可能時期は表示された日付/時刻の時点のものであり、変更される場合があります。本商品の購入においては、購入の時点でAmazon.co.jpに表示されている価格および発送可能時期の情報が適用されます。\n"+
                "Amazon.co.jpアソシエイト。\n"+
                "特典情報などの一部は独自作成したデータベースを利用しています。これらは最新の情報でない可能性があります。ご自身でご確認の上ご利用ください。\n"
                , "OK");
        }

    }
}