using Microsoft.Maui.Controls;
using System;
using DisApp24.Services;

namespace DisApp24
{
    public partial class RegistrationPage : ContentPage
    {
        private readonly IFirebaseAuthService _firebaseAuthService;

        public RegistrationPage(IFirebaseAuthService firebaseAuthService)
        {
            InitializeComponent();
            _firebaseAuthService = firebaseAuthService;
        }

        private async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            var email = EmailEntry.Text;
            var password = PasswordEntry.Text;
            var confirmPassword = ConfirmPasswordEntry.Text;

            if (password != confirmPassword)
            {
                await DisplayAlert("Fehler", "Die eingegebenen Passwörter stimmen nicht überein.", "OK");
                return;
            }

            try
            {
                var result = await _firebaseAuthService.SignUpWithEmailPasswordAsync(email, password);
                // Erfolgreiche Registrierung, Navigation zur Hauptseite oder einer anderen Seite
                await Navigation.PopModalAsync(); // Hier schließen wir die RegistrationPage
            }
            catch (Exception ex)
            {
                // Fehler bei der Registrierung, eine Fehlermeldung anzeigen
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            // Zurück zur Anmeldeseite
            Navigation.PopModalAsync();
        }
    }
}
