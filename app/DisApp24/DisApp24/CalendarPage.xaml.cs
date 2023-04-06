﻿using System;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging;

namespace DisApp24
{
    public partial class CalendarPage : ContentPage
    {
        public CalendarPage()
        {
            InitializeComponent();
            BindingContext = new CalendarPageViewModel();
        }
        private void OnDateSelected(DateTime date)
        {
            // Send the selected date to ReservationPage
            WeakReferenceMessenger.Default.Send(new SelectedDateMessage { Date = date });
        }
        private async void OnDateConfirmed(object sender, EventArgs e)
        {
            var selectedDates = (BindingContext as CalendarPageViewModel).SelectedDates;
            if (selectedDates.Any())
            {
                OnDateSelected(selectedDates.First());
                await Navigation.PopModalAsync();
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
