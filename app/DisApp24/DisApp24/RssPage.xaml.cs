using Microsoft.Maui.Controls;
using DisApp24.Services;
using DisApp24.ViewModels;
using DisApp24.Modules.Login;
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Models;

namespace DisApp24
{
    public partial class RssPage : ContentPage
    {

        public RssPage(RssViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;

            WeakReferenceMessenger.Default.Register<RssPage, LanguageChangedMessage>(this, (recipient, message) =>
            {
                recipient.RefreshViewModel();
            });
        }

        private async void RefreshViewModel()
        {
            await (BindingContext as RssViewModel).Initialize();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await (BindingContext as RssViewModel).Initialize();

        }

        // OnDisappearing Methode bleibt unverändert.
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}
