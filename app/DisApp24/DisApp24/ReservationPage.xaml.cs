
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Services;
using DisApp24.Models;
using Firebase.Database;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Firebase.Database.Query;
using Firebase.Auth;
using System.Diagnostics;
using System.Globalization;
using DisApp24.ViewModels;

namespace DisApp24
{
    public partial class ReservationPage : ContentPage
    {

        
        private readonly IFirebaseAuthService _firebaseAuthService;
        private bool isFirstTimeAppearing = true;
        private AppUser currentUser;
        private ObservableCollection<Appointment> _appointments;

        private readonly ReservationViewModel _viewModel;


        public ReservationPage()
        {
            InitializeComponent();
            InitializePickers();

            _viewModel = ServiceHelper.GetService<ReservationViewModel>();
            BindingContext = _viewModel;

            _firebaseAuthService = ServiceHelper.GetService<IFirebaseAuthService>();

            // Initialize the appointments collection
            _appointments = new ObservableCollection<Appointment>();

            WeakReferenceMessenger.Default.Register<SelectedDateMessage>(this, (recipient, message) =>
            {
                SelectedDateLabel.Text = $"Ausgewähltes Datum: {message.Date.ToString("dd.MM.yyyy")}";
            });

            WeakReferenceMessenger.Default.Register<UserChangedMessage>(this, OnUserChanged);

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

        //Event-Handler
        private void OnUserChanged(object recipient, UserChangedMessage message)
        {
            currentUser = message.Value;
            isFirstTimeAppearing = true;
        }

        private async void OnResourcePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            if (ResourcePicker.SelectedIndex != -1)
            {
                // Führe die Ausblendanimation aus, bevor die Auswahl ändern
                await FormLayout.FadeTo(0, 250, Easing.SinInOut);

                // Warte einen Moment, bevor die Einblendanimation starten
                await Task.Delay(50);

                FormLayout.IsVisible = true;
                await FormLayout.FadeTo(1, 250, Easing.SinInOut);
            }
            else
            {
                await FormLayout.FadeTo(0, 300, Easing.SinInOut);
                FormLayout.IsVisible = false;
            }
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is Entry entry)
            {
                if (entry.Parent is Frame frame)
                {
                    frame.BorderColor = Colors.DimGray;
                }
            }
        }

        private void OnTimePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            TimeFrame.BorderColor = Colors.DimGray;
        }

        private async void OnSelectDateButtonClicked(object sender, EventArgs e)
        {
            await AppShell.Current.GoToAsync(nameof(CalendarPage));
            SelectedDateLabel.TextColor = Colors.DimGray;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_firebaseAuthService.IsSignedIn())
            {
                await AppShell.Current.GoToAsync(nameof(LoginPage));
            }

            if (currentUser == null && _firebaseAuthService.IsSignedIn())
            {
                currentUser = await _firebaseAuthService.GetCurrentUserAsync();
                isFirstTimeAppearing = true;
            }


            if (currentUser != null)
            {

                if (currentUser != null && isFirstTimeAppearing)
                {
                    // Eingabefelder mit Benutzerdaten ausfüllen
                    FirstNameEntry.Text = currentUser.FirstName;
                    LastNameEntry.Text = currentUser.LastName;
                    EmailEntry.Text = currentUser.Email;
                    PhoneNumberEntry.Text = currentUser.PhoneNumber;
                }

                // Fetch user's appointments from Firebase and add them to the appointments collection
                var userAppointments = await _viewModel.GetUserAppointmentsAsync(currentUser.Uid);

                _appointments.Clear();

                foreach (var appointment in userAppointments.OrderBy(a => DateTime.ParseExact(a.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
                {
                    _appointments.Add(appointment);
                }

                AppointmentsCollection.ItemsSource = _appointments;

                await _viewModel.PrintAvailableTimeSlotsPerMonth();

                isFirstTimeAppearing = false;

            }

        }






    }
}