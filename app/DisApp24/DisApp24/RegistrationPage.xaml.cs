using Microsoft.Maui.Controls;
using System;
using DisApp24.Services;
using DisApp24.Helpers;


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
            var firstName = FirstNameEntry.Text;
            var lastName = LastNameEntry.Text;
            var phoneNumber = PhoneNumberEntry.Text;

            if (!ValidateInput())
            {
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Fehler", "Die eingegebenen Passwörter stimmen nicht überein.", "OK");
                return;
            }

            try
            {
                var result = await _firebaseAuthService.SignUpWithEmailPasswordAsync(email, password, firstName, lastName, phoneNumber);
                // Erfolgreiche Registrierung, Navigation zur Hauptseite oder einer anderen Seite
                await Navigation.PopModalAsync(); // Hier schließen wir die RegistrationPage
            }
            catch (Exception ex)
            {
                // Fehler bei der Registrierung, eine Fehlermeldung anzeigen
                await DisplayAlert("Fehler", ex.Message, "OK");
            }
        }

        private bool ValidateInput()
        {
            bool isValid = true;
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text))
            {
                FirstNameFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie Ihren Vornamen ein.");
                isValid = false;
            }
            else
            {
                FirstNameFrame.BorderColor = Colors.DimGray;
            }

            if (string.IsNullOrWhiteSpace(LastNameEntry.Text))
            {
                LastNameFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie Ihren Nachnamen ein.");
                isValid = false;
            }
            else
            {
                LastNameEntry.BorderColor = Colors.DimGray;
            }
            if (InputValidationHelper.IsNullOrWhiteSpace(EmailEntry.Text) || !InputValidationHelper.IsValidEmail(EmailEntry.Text))
            {
                EmailEntry.BackgroundColor = Colors.Red.WithAlpha((float)0.5);
                errorMessages.Add("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
                isValid = false;
            }
            else
            {
                EmailEntry.BackgroundColor = Colors.Transparent;
            }

            if (InputValidationHelper.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                PasswordEntry.BackgroundColor = Colors.Red.WithAlpha((float)0.5);
                errorMessages.Add("Bitte geben Sie ein Passwort ein.");
                isValid = false;
            }
            else
            {
                PasswordEntry.BackgroundColor = Colors.Transparent;
            }

            if (InputValidationHelper.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text) || ConfirmPasswordEntry.Text != PasswordEntry.Text)
            {
                ConfirmPasswordEntry.BackgroundColor = Colors.Red.WithAlpha((float)0.5);
                errorMessages.Add("Die eingegebenen Passwörter stimmen nicht überein.");
                isValid = false;
            }
            else
            {
                ConfirmPasswordEntry.BackgroundColor = Colors.Transparent;
            }

            ValidationLabel.Text = string.Join("\n", errorMessages);

            return isValid;
        }

        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            // Zurück zur Anmeldeseite
            Navigation.PopModalAsync();
        }
    }
}
