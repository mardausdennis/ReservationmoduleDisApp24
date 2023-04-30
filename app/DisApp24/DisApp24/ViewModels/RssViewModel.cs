using DisApp24.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Windows.Input;
using DisApp24.Models;
using Microsoft.Maui.Controls;

namespace DisApp24.ViewModels
{
    public class RssViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<RssItem> RssItems { get; set; }

        private IFirebaseAuthService _firebaseAuthService;
        private IRssService _rssService;
        private readonly AppConfig _config;
        public ICommand SignInCommand { get; set; }
        public ICommand SignOutCommand { get; set; }
        public ICommand ItemSelectedCommand { get; set; }

        private INavigation _navigation;

        public event PropertyChangedEventHandler PropertyChanged;
        public Action SignInStateChanged { get; set; }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public RssViewModel(IFirebaseAuthService firebaseAuthService, IRssService rssService, INavigation navigation, AppConfig config) 
        {
            _firebaseAuthService = firebaseAuthService;
            _rssService = rssService;
            _navigation = navigation;
            _config = config;

            RssItems = new ObservableCollection<RssItem>();
            SignInCommand = new Command(async () => await ExecuteSignInCommand());
            SignOutCommand = new Command(ExecuteSignOutCommand);
            ItemSelectedCommand = new Command<RssItem>(async (item) => await ExecuteItemSelectedCommand(item));
        }



        private async Task ExecuteSignInCommand()
        {
            await _navigation.PushAsync(new LoginPage(_firebaseAuthService, isModal: false));
            SignInStateChanged?.Invoke();
        }

        private void ExecuteSignOutCommand()
        {
            _firebaseAuthService.SignOutAsync();
            SignInStateChanged?.Invoke();
        }

        private async Task ExecuteItemSelectedCommand(RssItem item)
        {
            if (item == null)
                return;

            var contentStripped = WebUtility.HtmlDecode(item.Summary);

            await _navigation.PushAsync(new RssItemDetailsPage(item)); 
        }

        private async Task LoadRssFeed()
        {
            string rssUrl = _config.RssUrl;
            var rssItems = await _rssService.GetRssFeedAsync(rssUrl);

            foreach (var item in rssItems)
            {
                RssItems.Add(new RssItem
                {
                    Title = item.Title,
                    Summary = item.Summary,
                    PublishDate = item.PublishDate
                });
            }
        }


        public async Task Initialize()
        {
            await LoadRssFeed();
        }



    }
}
