using Microsoft.Maui.Controls;
using DisApp24.Services;
using DisApp24.ViewModels;
using DisApp24.Models;

namespace DisApp24
{
    public partial class RssPage : ContentPage
    {

        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IRssService _rssService;
        
        private ToolbarItem _signInButton;
        private ToolbarItem _signOutButton;

        public RssPage(IFirebaseAuthService firebaseAuthService, IRssService rssService, AppConfig config)
        {
            InitializeComponent();
            var viewModel = new RssViewModel(firebaseAuthService, rssService, Navigation, config);
            BindingContext = viewModel;

            _rssService = rssService;
            _firebaseAuthService = firebaseAuthService;

            _signInButton = new ToolbarItem { Text = "Anmelden", Command = viewModel.SignInCommand };
            _signOutButton = new ToolbarItem { Text = "Abmelden", Command = viewModel.SignOutCommand };

            viewModel.SignInStateChanged += UpdateSignInOutButtons;
            UpdateSignInOutButtons();
        }




        private void UpdateSignInOutButtons()
        {
            ToolbarItems.Clear();

            if (_firebaseAuthService.IsSignedIn())
            {
                ToolbarItems.Add(_signOutButton);
            }
            else
            {
                ToolbarItems.Add(_signInButton);
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as RssViewModel).Initialize();
            UpdateSignInOutButtons();
        }


    }
}
