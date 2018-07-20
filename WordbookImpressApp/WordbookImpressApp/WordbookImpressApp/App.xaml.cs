using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation (XamlCompilationOptions.Compile)]
namespace WordbookImpressApp
{
	public partial class App : Application
	{
        public App()
        {
            InitializeComponent();

            Load();

            {
                var page = new MasterDetailPage()
                {
                    Master = new Views.MasterPage(),
                    Detail = new NavigationPage(new Views.WordbooksPage()) { Title = "’PŒê’ " }
                };
                if (WordbookImpressLibrary.Storage.TutorialStorage.TutorialCompleted)
                {
                    MainPage = page;
                }
                else
                {
                    var model = Views.TutorialsPage.GetTutorial();
                    model.OnFinishAction = () =>
                    {
                        WordbookImpressLibrary.Storage.TutorialStorage.SetTutorialCompleted(true);
                        MainPage = page;
                    };
                    MainPage = new Views.TutorialsPage(model);
                }
            }
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}

        private async void Load()
        {
            WordbookImpressLibrary.Storage.ImageCacheStorage.PlaceholderWordbook = "tango.jpg";
            await WordbookImpressLibrary.Storage.WordbooksImpressInfoStorage.LoadLocalData();
            await WordbookImpressLibrary.Storage.RecordStorage.LoadLocalData();
            await WordbookImpressLibrary.Storage.WordbooksImpressStorage.LoadLocalData();
            await WordbookImpressLibrary.Storage.ConfigStorage.LoadLocalData();
            await WordbookImpressLibrary.Storage.RemoteStorage.LoadRemoteDatas();
            await WordbookImpressApp.Storage.LicenseStorage.LoadNugetDatasLicenseText();
        }
    }
}
