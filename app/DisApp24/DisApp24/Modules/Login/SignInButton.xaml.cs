using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Models;
using DisApp24.Resources;
using DisApp24.Services;
using Microsoft.Maui.Controls;

namespace DisApp24.Modules.Login
{
    public partial class SignInButton : ContentView
    {
        private readonly IFirebaseAuthService _firebaseAuthService;
        public event EventHandler SignInStatusChanged;

        public ICommand AuthCommand { get; }

        public SignInButton()
        {
            InitializeComponent();

            // Optional: Abhängigkeitsinjektion im Konstruktor verwenden (wenn in Ihrer App eingerichtet).
            _firebaseAuthService = ServiceHelper.GetService<IFirebaseAuthService>();

            AuthCommand = new Command(ExecuteAuthCommand);
            BindingContext = this; // Dies stellt sicher, dass die View die Commands dieses ViewModels verwenden kann.

            UpdateButton();

            // Abonnieren der Nachricht
            WeakReferenceMessenger.Default.Register<SignInButton, UserSignedInMessage>(this, (recipient, message) =>
            {
                recipient.UpdateButton();
                recipient.SignInStatusChanged?.Invoke(this, EventArgs.Empty);
            });

            WeakReferenceMessenger.Default.Register<SignInButton, LanguageChangedMessage>(this, (recipient, message) =>
            {
                recipient.UpdateButton();
            });
        }

        public void UpdateButton()
        {
            if (_firebaseAuthService.IsSignedIn())
            {
                AuthButton.Text = AppResources.SignInButtonText_SignedIn;
            }
            else
            {
                AuthButton.Text = AppResources.SignInButtonText_SignedOut;
            }

            SignInStatusChanged?.Invoke(this, EventArgs.Empty);
        }

        private async void ExecuteAuthCommand()
        {
            if (_firebaseAuthService.IsSignedIn())
            {
                await _firebaseAuthService.SignOutAsync();
            }
            else
            {
                await AppShell.Current.GoToAsync(nameof(LoginPage));
            }

            UpdateButton();
            SignInStatusChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
