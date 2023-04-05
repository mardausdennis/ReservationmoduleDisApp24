using System;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging;

namespace DisApp24
{
    public partial class ReservationPage : ContentPage
    {
        public ReservationPage()
        {
            InitializeComponent();
            InitializePickers();
            WeakReferenceMessenger.Default.Register<SelectedDateMessage>(this, (recipient, message) =>
            {
                SelectedDateLabel.Text = $"Ausgewähltes Datum: {message.Date.ToString("dd.MM.yyyy")}";
            });
        }
        private void InitializePickers()
        {
            // Populate the ResourcePicker with sample data
            ResourcePicker.ItemsSource = new string[]
            {
                "Ressource 1",
                "Ressource 2",
                "Ressource 3"
            };
        }
        private async void OnSelectDateButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushModalAsync(new CalendarPage());
        }
        private void OnReserveButtonClicked(object sender, EventArgs e)
        {
            // Code zur Implementierung der Reservierungsfunktionalität
        }
    }
}
