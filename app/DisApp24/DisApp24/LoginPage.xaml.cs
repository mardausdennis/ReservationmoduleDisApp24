using Microsoft.Maui.Controls;
using System;
using DisApp24.Services;
using Microsoft.Extensions.DependencyInjection;


namespace DisApp24
{
    public partial class LoginPage : ContentPage
    {

        private readonly IFirebaseAuthService _firebaseAuthService;

        public LoginPage(IFirebaseAuthService firebaseAuthService)
        {
            InitializeComponent();
            _firebaseAuthService = firebaseAuthService;
            
        }

        private async void OnLoginButtonClicked(object sender, EventArgs e)
        {
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;

            try
            {
                var result = await _firebaseAuthService.SignInWithEmailPasswordAsync(email, password);
                // Erfolgreiche Anmeldung, Navigation zur Hauptseite oder einer anderen Seite
                await Navigation.PopModalAsync(); // Hier schließen wir die LoginPage
            }
            catch (Exception ex)
            {
                // Fehler bei der Anmeldung, eine Fehlermeldung anzeigen
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
        }

        private async void OnGoogleLoginButtonClicked(object sender, EventArgs e)
        {
            try
            {
               
                await DisplayAlert("Info", "Die Google-Anmeldung ist derzeit nicht verfügbar.", "OK");

            }
            catch (Exception ex)
            {
                // Fehler bei der Anmeldung, eine Fehlermeldung anzeigen
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
        }


        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            // Navigation zur RegistrationPage
            await Navigation.PushModalAsync(new RegistrationPage(_firebaseAuthService));
        }


        private void OnGuestContinueButtonClicked(object sender, EventArgs e)
        {
            // Als Gast fortfahren
            Navigation.PopModalAsync();
        }
    }
}
