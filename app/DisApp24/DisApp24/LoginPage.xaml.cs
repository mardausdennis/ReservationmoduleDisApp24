using Microsoft.Maui.Controls;
using System;

namespace DisApp24
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void OnLoginButtonClicked(object sender, EventArgs e)
        {
            // Logik für die Anmeldung hinzufügen
        }

        private void OnGoogleLoginButtonClicked(object sender, EventArgs e)
        {
            // Logik für die Anmeldung mit Google hinzufügen
        }

        private void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            // Navigieren zur Registrierungsseite
        }

        private void OnGuestContinueButtonClicked(object sender, EventArgs e)
        {
            // Als Gast fortfahren
            Navigation.PopModalAsync();
        }
    }
}
