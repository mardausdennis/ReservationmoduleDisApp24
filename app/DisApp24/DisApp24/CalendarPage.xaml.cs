using System;
using Microsoft.Maui.Controls;
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Services;
using DisApp24.ViewModels;
using DisApp24.Models;

namespace DisApp24
{
    public partial class CalendarPage : ContentPage
    {
        public CalendarPage()
        {
            InitializeComponent();
            BindingContext = ServiceHelper.GetService<CalendarPageViewModel>();

            var navigationService = ServiceHelper.GetService<NavigationService>();
            navigationService.SetNavigation(Navigation);

            // Subscribe to the ShowAlertMessage
            WeakReferenceMessenger.Default.Register<ShowAlertMessage>(this, async (r, m) =>
            {
                await DisplayAlert(m.Title, m.Message, m.ButtonText);
            });

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            // Unsubscribe from the ShowAlertMessage when the page is disappearing
            WeakReferenceMessenger.Default.Unregister<ShowAlertMessage>(this);
        }


    }
    
}
