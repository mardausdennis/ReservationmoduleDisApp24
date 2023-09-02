using System;
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Models;
using Microsoft.Maui.Controls;

namespace DisApp24
{
    public partial class LanguageSwitchButton : ContentView
    {
        private LocalizationService _localizationService = new LocalizationService();

        public LanguageSwitchButton()
        {
            InitializeComponent();
            UpdateButtonImage();
        }

        private void UpdateButtonImage()
        {
            var currentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";

            if (currentCulture.Equals("en-US", StringComparison.OrdinalIgnoreCase))
            {      
                SwitchLanguageButton.Source = "austria_flag.png";
            }
            else
            {
                SwitchLanguageButton.Source = "uk_flag.png";
            }
        }



        private void OnSwitchLanguageClicked(object sender, EventArgs e)
        {
            var currentCulture = System.Globalization.CultureInfo.DefaultThreadCurrentUICulture?.Name ?? "en-US";

            if (currentCulture.Equals("en-US", StringComparison.OrdinalIgnoreCase))
            {
                _localizationService.SetLanguage("de-DE");
            }
            else
            {
                _localizationService.SetLanguage("en-US");
            }

            UpdateButtonImage();

            WeakReferenceMessenger.Default.Send(new LanguageChangedMessage());
        }

    }

    public class LocalizationService
    {
        public void SetLanguage(string cultureName)
        {
            var culture = new System.Globalization.CultureInfo(cultureName);
            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;
            // Optionally, add logic here to refresh the current view or restart the app to see changes.
        }
    }
}
