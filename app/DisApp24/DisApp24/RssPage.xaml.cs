using Microsoft.Maui.Controls;
using DisApp24.Services;
using DisApp24.ViewModels;
using DisApp24.Models;

namespace DisApp24
{
    public partial class RssPage : ContentPage
    {

        private readonly IFirebaseAuthService _firebaseAuthService;
        
        
        private ToolbarItem _signInButton;
        private ToolbarItem _signOutButton;

        public RssPage(RssViewModel vm)
        {
            InitializeComponent();
            var viewModel = vm;
            BindingContext = viewModel;

            _firebaseAuthService = ServiceHelper.GetService<IFirebaseAuthService>();

            _signInButton = new ToolbarItem { Text = "Anmelden", Command = viewModel.SignInCommand };
            _signOutButton = new ToolbarItem { Text = "Abmelden", Command = viewModel.SignOutCommand };

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

            if (BindingContext is RssViewModel viewModel)
            {
                viewModel.SignInStateChanged += UpdateSignInOutButtons;
            }
            UpdateSignInOutButtons();
        }


        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (BindingContext is RssViewModel viewModel)
            {
                viewModel.SignInStateChanged -= UpdateSignInOutButtons;
            }
        }

    }
}
