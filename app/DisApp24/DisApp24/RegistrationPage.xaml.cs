using Microsoft.Maui.Controls;
using DisApp24.Services;
using DisApp24.Helpers;
using DisApp24.ViewModels;
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Models;

namespace DisApp24
{
    public partial class RegistrationPage : ContentPage, IRecipient<RegistrationMessage>
    {

        private readonly RegistrationViewModel _viewModel;

        public RegistrationPage()
        {
            InitializeComponent();
            _viewModel = ServiceHelper.GetService<RegistrationViewModel>();
            BindingContext = _viewModel;

            // Subscribe to the messages
            WeakReferenceMessenger.Default.Register(this);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            // Unsubscribe from the messages when the page is not visible
            WeakReferenceMessenger.Default.Unregister<RegistrationMessage>(this);
        }

        public void Receive(RegistrationMessage message)
        {
            if (message.IsSuccessful)
            {
                // Handle successful registration
                Navigation.PopAsync(); 
                Navigation.PopAsync();
            }
        }
    }
}
