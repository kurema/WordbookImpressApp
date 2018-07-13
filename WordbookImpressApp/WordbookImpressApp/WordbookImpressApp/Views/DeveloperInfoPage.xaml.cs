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

namespace WordbookImpressApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class DeveloperInfoPage : ContentPage
	{
        public DeveloperInfoViewModel Model => this.BindingContext as DeveloperInfoViewModel;

        public DeveloperInfoPage ()
		{
			InitializeComponent ();

            this.BindingContext = new DeveloperInfoViewModel();

            {
                GithubGrid.ColumnDefinitions = new ColumnDefinitionCollection();
                GithubGrid.Children.Clear();
                AddGithubInfo("GithubUser.Repos", "Projects");
                AddGithubInfo("GithubUser.Followers", "Followers");
                AddGithubInfo("GithubUser.Following", "Following");
            }

            UpdateGithubInfo();
        }

        public async void UpdateGithubInfo()
        {

            var github = new Octokit.GitHubClient(new Octokit.ProductHeaderValue(nameof(WordbookImpressApp)));
            var user = await github.User.Get("kurema");
            Model.GithubUser = new GithubUserViewModel(user);
        }

        public void AddGithubInfo(string source,string Header)
        {
            var cnt = GithubGrid.ColumnDefinitions?.Count() ?? 0;

            GithubGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            var sl = new StackLayout();
            var fc = new FontSizeConverter();
            {
                var label = new Label() {HorizontalTextAlignment = TextAlignment.Center, FontSize = (double)fc.ConvertFromInvariantString("Large"), FontAttributes = FontAttributes.Bold };
                label.SetBinding(Label.TextProperty, source);
                sl.Children.Add(label);
            }
            sl.Children.Add(new Label() { Text = Header,  HorizontalTextAlignment = TextAlignment.Center });
            Grid.SetColumn(sl, cnt);
            GithubGrid.Children.Add(sl);
        }

        public class DeveloperInfoViewModel:WordbookImpressLibrary.ViewModels.BaseViewModel
        {
            private GithubUserViewModel _GithubUser;
            public GithubUserViewModel GithubUser { get => _GithubUser; set => SetProperty(ref _GithubUser, value); }

            private AuthorInformationViewModel authorInformation;
            public AuthorInformationViewModel AuthorInformation { get => authorInformation ?? new AuthorInformationViewModel(RemoteStorage.AuthorInformation); }

            private WordbookImpressLibrary.Helper.DelegateCommand openUriCommand;
            public WordbookImpressLibrary.Helper.DelegateCommand OpenUriCommand => openUriCommand = openUriCommand ?? new WordbookImpressLibrary.Helper.DelegateCommand((o) => true,
                (o) =>
                {
                    Device.OpenUri(new Uri(o.ToString()));
                });

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

            public GithubUserViewModel(Octokit.User user) { this.user = user; }
        }

        public class AuthorInformationViewModel
        {
            private author author;

            public string Name => author.name;

            public AuthorInformationViewModel(author author) { this.author = author; }

            private ObservableCollection<Link> links;
            public ObservableCollection<Link> Links { get => links = links ?? GetLinks(); }

            public ObservableCollection<Link> GetLinks()
            {
                if (author?.links == null) return new ObservableCollection<Link>();
                var result=new ObservableCollection<Link>();
                foreach (var item in author.links)
                {
                    result.Add(new Link(WordbookImpressLibrary.Storage.RemoteStorage.GetStringByMultilingal(item?.title?.multilingal) ?? item.title1, item.src));
                }
                return result;
            }

            private ObservableCollection<Group<Link>> donations;
            public ObservableCollection<AuthorInformationViewModel.Group<Link>> Donations { get => donations ?? GetDonations(); }
            public ObservableCollection<AuthorInformationViewModel.Group<Link>> GetDonations()
            {
                if (author?.donations == null) return new ObservableCollection<Group<Link>>();
                var result= new ObservableCollection<Group<Link>>();
                foreach (var gp in author.donations)
                {
                    var group = new Group<Link>();
                    group.Title = WordbookImpressLibrary.Storage.RemoteStorage.GetStringByMultilingal(gp.title.multilingal) ?? gp.title1;
                    foreach(var item in gp.donation)
                    {
                        group.Add(new Link(RemoteStorage.GetStringByMultilingal(item.title.multilingal) ?? item.title1, item.src));
                    }
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
                        Device.OpenUri(new Uri(Src));
                    });
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

        private async void Button_Clicked_Donate(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ConfigPage(new System.Collections.ObjectModel.ObservableCollection<ConfigPage.SettingItems>()
            {
                new ConfigPage.SettingItems("仮想通貨")
                {
                    new ConfigPage.SettingItem("ビットコイン",""){Action=async (s)=>{Device.OpenUri(new Uri("monacoin:aaa")); } }
                }
            }
                )
            { Title="寄付"});
        }
    }
}