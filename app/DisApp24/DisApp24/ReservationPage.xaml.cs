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

            // Populate the TimePicker with time slots
            TimePicker.ItemsSource = new string[]
            {
                "08:00-09:00",
                "09:00-10:00",
                "10:00-11:00",
                "11:00-12:00",
                "12:00-13:00",
                "13:00-14:00",
                "14:00-15:00",
                "15:00-16:00"
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
