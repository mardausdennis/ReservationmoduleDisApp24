using Microsoft.Maui.Controls;
using System;
using DisApp24.Services;
using DisApp24.Helpers;


namespace DisApp24
{
    public partial class RegistrationPage : ContentPage
    {
        private readonly IFirebaseAuthService _firebaseAuthService;

        public RegistrationPage()
        {
            InitializeComponent();
            _firebaseAuthService = ServiceHelper.GetService<IFirebaseAuthService>();
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

        //Validate-Input
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
                LastNameFrame.BorderColor = Colors.DimGray;
            }

            if (string.IsNullOrWhiteSpace(EmailEntry.Text) || !InputValidationHelper.IsValidEmail(EmailEntry.Text))
            {
                EmailFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
                isValid = false;
            }
            else
            {
                EmailFrame.BorderColor = Colors.DimGray;
            }

            if (!string.IsNullOrWhiteSpace(PhoneNumberEntry.Text) && !InputValidationHelper.IsValidPhoneNumber(PhoneNumberEntry.Text))
            {
                PhoneNumberFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie eine gültige Telefonnummer ein.");
                isValid = false;
            }
            else
            {
                PhoneNumberFrame.BorderColor = Colors.DimGray;
            }

            if (string.IsNullOrWhiteSpace(PasswordEntry.Text))
            {
                PasswordFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie ein Passwort ein.");
                isValid = false;
            }
            else
            {
                PasswordFrame.BorderColor = Colors.DimGray;
            }

            if (string.IsNullOrWhiteSpace(ConfirmPasswordEntry.Text) || ConfirmPasswordEntry.Text != PasswordEntry.Text)
            {
                ConfirmPasswordFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Die eingegebenen Passwörter stimmen nicht überein.");
                isValid = false;
            }
            else
            {
                ConfirmPasswordFrame.BorderColor = Colors.DimGray;
            }

            ValidationLabel.Text = string.Join("\n", errorMessages);

            return isValid;
        }


        private void OnCancelButtonClicked(object sender, EventArgs e)
        {
            // Zurück zur Anmeldeseite
            Navigation.PopAsync();
        }
    }
}
