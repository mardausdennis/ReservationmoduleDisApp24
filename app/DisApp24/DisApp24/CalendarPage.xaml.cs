using System;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Services;
using DisApp24.ViewModels;

namespace DisApp24
{
    public partial class CalendarPage : ContentPage
    {
        public CalendarPage()
        {
            InitializeComponent();
            BindingContext = ServiceHelper.GetService<CalendarPageViewModel>();
        }

        private void OnDateSelected(DateTime date)
        {
            WeakReferenceMessenger.Default.Send(new SelectedDateMessage { Date = date });
        }

        private async void OnDateConfirmed(object sender, EventArgs e)
        {
            var selectedDates = (BindingContext as CalendarPageViewModel).SelectedDates;

            if (selectedDates != null && selectedDates.Any())
            {
                OnDateSelected(selectedDates.First());
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Please select a date before confirming.", "OK");
            }
        }

        private void DisplaySelectedDates(IEnumerable<DateTime> dates)
        {
            var selectedDatesLabel = new Label
            {
                Text = string.Join(", ", dates.Select(date => date.ToString("dd/MM/yyyy"))),
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            MainStackLayout.Children.Add(selectedDatesLabel);
        }
    }
    public class SelectedDateMessage
    {
        public DateTime Date { get; set; }
    }
}
