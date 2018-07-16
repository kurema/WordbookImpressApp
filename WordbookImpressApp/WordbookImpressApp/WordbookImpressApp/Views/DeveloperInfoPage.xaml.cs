using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using WordbookImpressLibrary.Schemas.AuthorInformation;
using System.Collections.ObjectModel;

using System.ComponentModel;
using System.Runtime.CompilerServices;

using WordbookImpressLibrary.Storage;
using WordbookImpressLibrary.Helper;

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeveloperInfoPage : ContentPage
	{
        public DeveloperInfoViewModel Model => this.BindingContext as DeveloperInfoViewModel;

        public DeveloperInfoPage ()
		{
			InitializeComponent ();

            this.BindingContextChanged += (s, e) => LinksUpdate();

            DeveloperInfoViewModel model;
            this.BindingContext = model = new DeveloperInfoViewModel(Navigation);
            this.Model.PropertyChanged += (s, e) => LinksUpdate();
            RemoteStorage.Updated += (s, e) => { LinksUpdate();  };

            {
                GithubGrid.ColumnDefinitions = new ColumnDefinitionCollection();
                GithubGrid.Children.Clear();
                AddGithubInfo("GithubUser.Repos", "Projects", "OpenUriCommand", "GithubUser.HtmlUrl");
                AddGithubInfo("GithubUser.Followers", "Followers", "OpenUriCommand", "GithubUser.HtmlUrl");
                //AddGithubInfo("GithubUser.Following", "Following", "OpenUriCommand", "GithubUser.HtmlUrl");
                AddGithubInfo("TwitterUser.FollowersCount", "Twitter\nFollowers", "OpenUriCommand", "TwitterUser.Url");
                //AddGithubInfo("TwitterUser.FollowingsCount", "Following", "OpenUriCommand", "GithubUser.HtmlUrl");
            }

            UpdateGithubInfo();
            UpdateTwitterInfo();
            UpdateAmazonInfo();
        }

        public async void UpdateAmazonInfo()
        {
            storeItems.Clear();
            await storeItems.AddSearchResult("B077X71C4C", Nager.AmazonProductAdvertising.Model.AmazonSearchIndex.Books);
        }

        public async void UpdateGithubInfo()
        {
            try
            {
                var github = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(nameof(WordbookImpressApp)));
                var user = await github.User.Get("kurema");
                Model.GithubUser = new GithubUserViewModel(user);
            }
            catch { }
        }

        public async void UpdateTwitterInfo()
        {
            try
            {
                var key = await Storage.TwitterStorage.GetAppOnlyToken();
                var tws = await key.Statuses.UserTimelineAsync(count => 100, screen_name => "kurema_makoto", include_rts => false);
                Model.TwitterTimeline = new ObservableCollection<TweetViewModel>(tws.Select(s => new TweetViewModel(s)));
                Model.TwitterUser = new TwitterUserInfoViewModel(tws.First()?.User);
            }
            catch(Exception e) {
            }
        }

        public void AddGithubInfo(string source, string Header, string command=null,string commandParameter=null)
        {
            var cnt = GithubGrid.ColumnDefinitions?.Count() ?? 0;

            GithubGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            var sl = new StackLayout();
            if (command != null)
            {
                {
                    var gr = new TapGestureRecognizer();
                    gr.SetBinding(TapGestureRecognizer.CommandProperty, command);
                    gr.SetBinding(TapGestureRecognizer.CommandParameterProperty, commandParameter);
                    sl.GestureRecognizers.Add(gr);
                }
                {
                    var gr = new ClickGestureRecognizer();
                    gr.SetBinding(ClickGestureRecognizer.CommandProperty, command);
                    gr.SetBinding(ClickGestureRecognizer.CommandParameterProperty, commandParameter);
                    sl.GestureRecognizers.Add(gr);
                }
            }
            var fc = new FontSizeConverter();
            {
                var label = new Label() {HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions=LayoutOptions.CenterAndExpand, FontSize = (double)fc.ConvertFromInvariantString("Large"), FontAttributes = FontAttributes.Bold };
                label.SetBinding(Label.TextProperty, source);
                sl.Children.Add(label);
            }
            sl.Children.Add(new Label() { Text = Header, HorizontalTextAlignment = TextAlignment.Center, HorizontalOptions = LayoutOptions.CenterAndExpand, TextColor = new Color(0.0, 0.0, 0.0, 0.5) });
            Grid.SetColumn(sl, cnt);
            GithubGrid.Children.Add(sl);
        }

        private void LinksUpdate()
        {
            Links.Children.Clear();

            if (!String.IsNullOrWhiteSpace(Model?.GithubUser?.Email))
            {
                var command = new DelegateCommand((o) => true, (o) => { try { Device.OpenUri(new Uri(Model.GithubUser.Email)); } catch { } });
                LinkUpdateAddLink("Email", Model.GithubUser.Email, command);
            }

            if (Model?.AuthorInformation?.Links != null)
            {
                foreach (var item in Model.AuthorInformation.Links)
                {
                    LinkUpdateAddLink(item.Title, item.Src, item.OpenUriCommand);
                }
            }
        }

        private void LinkUpdateAddLink(string title,string detail, System.Windows.Input.ICommand command)
        {
            var stack = new StackLayout();
            {
                stack.Padding = 10;
                stack.GestureRecognizers.Add(new TapGestureRecognizer() { Command = command });
                stack.GestureRecognizers.Add(new ClickGestureRecognizer() { Command = command });
            }
            {
                var label = new Label();
                label.Text = title;
                stack.Children.Add(label);
            }
            {
                var label = new Label();
                label.Text = detail;
                //label.VerticalTextAlignment = TextAlignment.End;
                //label.VerticalOptions = LayoutOptions.EndAndExpand;
                label.TextColor = new Color(0, 0, 0, 0.5);
                label.LineBreakMode = LineBreakMode.TailTruncation;
                stack.Children.Add(label);
            }
            Links.Children.Add(stack);
        }

        public class DeveloperInfoViewModel:WordbookImpressLibrary.ViewModels.BaseViewModel
        {
            private GithubUserViewModel _GithubUser;
            public GithubUserViewModel GithubUser { get => _GithubUser; set => SetProperty(ref _GithubUser, value); }

            private AuthorInformationViewModel authorInformation;
            public AuthorInformationViewModel AuthorInformation { get => authorInformation = authorInformation ?? (RemoteStorage.AuthorInformation != null ? new AuthorInformationViewModel(RemoteStorage.AuthorInformation, navigation) : null); }

            public void AuthorInformationUpdated() => OnPropertyChanged(nameof(AuthorInformation));

            private ObservableCollection<TweetViewModel> twitterTimeline = new ObservableCollection<TweetViewModel>();
            public ObservableCollection<TweetViewModel> TwitterTimeline { get => twitterTimeline; set => SetProperty(ref twitterTimeline, value); }

            private TwitterUserInfoViewModel twitterUser;
            public TwitterUserInfoViewModel TwitterUser { get => twitterUser; set => SetProperty(ref twitterUser, value); }

            private DelegateCommand openUriCommand;
            public DelegateCommand OpenUriCommand => openUriCommand = openUriCommand ?? new WordbookImpressLibrary.Helper.DelegateCommand((o) => true,
                (o) =>
                {
                    try
                    {
                        Device.OpenUri(new Uri(o.ToString()));
                    }
                    catch { }
                });


            private INavigation navigation;

            public DeveloperInfoViewModel(INavigation navigation)
            {
                this.navigation = navigation;

                RemoteStorage.Updated += (s, e) => OnPropertyChanged(nameof(AuthorInformation));
            }
        }

        public class TwitterUserInfoViewModel
        {
            private CoreTweet.User user;
            public int FollowersCount => user.FollowersCount;
            public int FollowingsCount => user.FriendsCount;
            public string ProfileBackgroundImageUrl => user.ProfileBackgroundImageUrlHttps;
            public string ProfileBannerUrl => user.ProfileBannerUrl;
            public string ProfileUrl => user.Url;
            //Why CoreTweet do not have this!?
            public string Url => "https://twitter.com/" + user?.ScreenName;

            public TwitterUserInfoViewModel(CoreTweet.User user) {
                this.user = user;
            }
        }

        public class GithubUserViewModel
        {
            private Octokit.User user;

            public int Repos => user.PublicRepos;
            public int Gists => user.PublicGists;
            public int Followers => user.Followers;
            public int Following => user.Following;
            public string AvatarUri => user.AvatarUrl;
            public string Name => user.Name;
            public string Bio => user.Bio;
            public bool? Hireable => user.Hireable;
            public string Email => user.Email;

            public string HtmlUrl => user.HtmlUrl;

            public GithubUserViewModel(Octokit.User user) { this.user = user; }
        }

        public class TweetViewModel
        {
            //public string FullText => status?.FullText;
            public string Text => status?.Text;
            public DateTime DateTime => status?.CreatedAt.ToLocalTime().DateTime ?? new DateTime();
            public string DateTimeString => DateTime.ToString(System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat);
            //Why CoreTweet do not have this!?
            public string Url => "https://twitter.com/" + status.User.Id + "/status/" + status.Id;

            private CoreTweet.Status status;
            public TweetViewModel(CoreTweet.Status status)
            {
                this.status = status;
            }
        }

        public class AuthorInformationViewModel
        {
            private author author;

            public string Name => author.name;

            private INavigation navigation;

            public AuthorInformationViewModel(author author, INavigation navigation) { this.author = author; this.navigation = navigation; }

            private ObservableCollection<Link> links;
            public ObservableCollection<Link> Links { get => links = links ?? GetLinks(); }

            public ObservableCollection<Link> GetLinks()
            {
                if (author?.links == null) return new ObservableCollection<Link>();
                var result=new ObservableCollection<Link>();
                foreach (var item in author.links)
                {
                    result.Add(new Link(RemoteStorage.GetStringByMultilingal(item?.title?.multilingal) ?? item.title1, item.src));
                }
                return result;
            }

            public DelegateCommand OpenDonationCommand { get => new DelegateCommand(
                (p) => true,(p)=>
                {
                    new ConfigPage();
                    var result = new ObservableCollection<ConfigPage.SettingItems>();
                    foreach (var item in Donations)
                    {
                        var items = new ConfigPage.SettingItems(item.Title);
                        foreach (var item2 in item)

                        {
                            items.Add(new ConfigPage.SettingItem(item2.Title, string.IsNullOrWhiteSpace(item2.Address)?item2.Src: item2.Address)
                            {

                                Action = async (w) =>
                                {
                                    if (item2.Src != null)
                                    {
                                        try { Device.OpenUri(new Uri(item2.Src)); }
                                        catch
                                        {
                                            try
                                            {
                                                await navigation.PushModalAsync(new QRCodePage(item2.Src));
                                            }
                                            catch (Exception e) { }
                                        }
                                    }
                                    else if (item2.Address != null)
                                    {
                                        try
                                        {
                                            await navigation.PushModalAsync(new QRCodePage(item2.Address));
                                        }
                                        catch (Exception e) { }
                                    }
                                }
                            });
                        }
                        result.Add(items);
                    }
                    navigation.PushAsync(new ConfigPage(result) { Title = "寄付" });
                }); }


            private ObservableCollection<Group<LinkDonation>> donations;
            public ObservableCollection<AuthorInformationViewModel.Group<LinkDonation>> Donations { get => donations=donations ?? GetDonations(); }

            public ObservableCollection<AuthorInformationViewModel.Group<LinkDonation>> GetDonations()
            {
                if (author?.donations == null) return new ObservableCollection<Group<LinkDonation>>();
                var result= new ObservableCollection<Group<LinkDonation>>();
                foreach (var gp in author.donations)
                {
                    var group = new Group<LinkDonation>();
                    group.Title = WordbookImpressLibrary.Storage.RemoteStorage.GetStringByMultilingal(gp?.title?.multilingal) ?? gp.title1;
                    foreach(var item in gp.donation)
                    {
                        group.Add(new LinkDonation(RemoteStorage.GetStringByMultilingal(item?.title?.multilingal) ?? item.title1, item.src,item.address));
                    }
                    result.Add(group);
                }
                return result;
            }

            public class Link
            {
                public string Src { get; set; }
                public string Title { get; set; }

                public Link(string title,string src)
                {
                    this.Src = src;
                    this.Title = title;
                }

                private WordbookImpressLibrary.Helper.DelegateCommand openUriCommand;
                public WordbookImpressLibrary.Helper.DelegateCommand OpenUriCommand => openUriCommand = openUriCommand ?? new WordbookImpressLibrary.Helper.DelegateCommand((o) => true,
                    (o) =>
                    {
                        try
                        {
                            Device.OpenUri(new Uri(Src));
                        }
                        catch { }
                    });
            }

            public class LinkDonation : Link
            {
                public string Address { get; set; }

                public LinkDonation(string title, string src) : base(title,src) { }
                public LinkDonation(string title, string src,string Address) : base(title, src) { this.Address = Address; }

            }

            public class Group<T> : ObservableCollection<T>,INotifyPropertyChanged
            {
                private string title;
                public string Title { get => title; set { title = value; OnPropertyChanged(); } }

                #region INotifyPropertyChanged
                protected override event PropertyChangedEventHandler PropertyChanged;
                protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }
                #endregion
            }
        }

        private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            ((ListView)sender).SelectedItem = null;
            //((TweetViewModel)e.SelectedItem)
            try
            {
                Device.OpenUri(new Uri(((TweetViewModel)e.SelectedItem).Url));
            }
            catch { }
        }
    }
}