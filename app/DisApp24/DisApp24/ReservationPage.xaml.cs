
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Services;
using DisApp24.Helpers;
using Firebase.Database;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Firebase.Database.Query;
using Firebase.Auth;
using System.Diagnostics;
using System.Globalization;

namespace DisApp24
{
    public partial class ReservationPage : ContentPage
    {

        
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly FirebaseClient _firebaseClient = new FirebaseClient("https://disapp24-reservation-module-default-rtdb.europe-west1.firebasedatabase.app");
        private bool isFirstTimeAppearing = true;
        private AppUser currentUser;


        private ObservableCollection<Appointment> _appointments;


        public ReservationPage(IFirebaseAuthService firebaseAuthService)
        {
            InitializeComponent();
            InitializePickers();

            _firebaseAuthService = firebaseAuthService;

            // Initialize the appointments collection
            _appointments = new ObservableCollection<Appointment>();
            

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

        //Wenn Ressource ausgewählt dann Form sichtbar
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

        //Input-Validation 
        private bool ValidateInput()
        {
            bool isValid = true;
            List<string> errorMessages = new List<string>();

            if (ResourcePicker.SelectedIndex == -1)
            {
                errorMessages.Add("Bitte wählen Sie eine Ressource aus.");
                isValid = false;
            }
            if (string.IsNullOrWhiteSpace(FirstNameEntry.Text))
            {
                FirstNameFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie Ihren Vornamen ein.");
                isValid = false;
            }
            else
            {
                FirstNameFrame.BorderColor = Colors.DimGray;
            }

            if (string.IsNullOrWhiteSpace(LastNameEntry.Text))
            {
                LastNameFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie Ihren Nachnamen ein.");
                isValid = false;
            }
            else
            {
                LastNameFrame.BorderColor = Colors.DimGray;
            }

            if (string.IsNullOrWhiteSpace(EmailEntry.Text) || !InputValidationHelper.IsValidEmail(EmailEntry.Text))
            {
                EmailFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
                isValid = false;
            }
            else
            {
                EmailFrame.BorderColor = Colors.DimGray;
            }

            if (!string.IsNullOrWhiteSpace(PhoneNumberEntry.Text) && !InputValidationHelper.IsValidPhoneNumber(PhoneNumberEntry.Text))
            {
                PhoneNumberFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte geben Sie eine gültige Telefonnummer ein.");
                isValid = false;
            }
            else
            {
                PhoneNumberFrame.BorderColor = Colors.DimGray;
            }
            if (string.IsNullOrWhiteSpace(SelectedDateLabel.Text) || SelectedDateLabel.Text == "Datum auswählen")
            {
                SelectedDateLabel.TextColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte wählen Sie ein Datum aus.");
                isValid = false;
            }
            else
            {
                SelectedDateLabel.TextColor = Colors.Black;
            }

            if (TimePicker.SelectedIndex == -1)
            {
                TimeFrame.BorderColor = Color.FromRgba(255, 0, 0, 0.5);
                errorMessages.Add("Bitte wählen Sie eine Uhrzeit aus.");
                isValid = false;
            }
            else
            {
                TimeFrame.BorderColor = Colors.DimGray;
            }

            ValidationLabel.Text = string.Join("\n", errorMessages);

            return isValid;
        }




        //Event-Handler
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
            await Navigation.PushModalAsync(new CalendarPage(_firebaseAuthService));
            SelectedDateLabel.TextColor = Colors.DimGray;
        }

        private async void OnReserveButtonClicked(object sender, EventArgs e)
        {
            if (ValidateInput())
            {

                int dateStartIndex = SelectedDateLabel.Text.IndexOf(':') + 2;
                string selectedDate = SelectedDateLabel.Text.Substring(dateStartIndex);

                var reservationData = new
                {
                    resource = ResourcePicker.SelectedItem.ToString(),
                    firstName = FirstNameEntry.Text,
                    lastName = LastNameEntry.Text,
                    email = EmailEntry.Text,
                    phoneNumber = PhoneNumberEntry.Text,
                    date = selectedDate,
                    timeSlot = TimePicker.SelectedItem.ToString(),
                    comment = CommentEditor.Text,
                    status = "Pending",
                    userId = currentUser.Uid
                };

                var json = JsonConvert.SerializeObject(reservationData);

                try
                {
                    await _firebaseClient.Child("reservations").PostAsync(json);
                    await DisplayAlert("Erfolg", "Ihre Reservierungsanfrage wurde erfolgreich gesendet. Sie erhalten eine Bestätigung per E-Mail, sobald Ihre Reservierung bestätigt wurde.", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Fehler", "Es gab einen Fehler beim Senden Ihrer Reservierungsanfrage. Bitte versuchen Sie es später erneut.", "OK");
                }
            }
        }

        private async Task<List<Appointment>> GetUserAppointmentsAsync(string userId)
        {
            var appointments = new List<Appointment>();
            var firebaseAppointments = await _firebaseClient
                .Child("reservations")
                .OnceAsync<Appointment>();

            foreach (var item in firebaseAppointments)
            {
                var appointment = item.Object;
                if (appointment.UserId == userId) // Filtern der Termine anhand der userId.
                {
                    appointment.Key = item.Key;
                    appointments.Add(appointment);
                }
            }

            return appointments;
        }


        public bool AreAllTimeSlotsBooked(DateTime date, List<Appointment> appointments)
        {
            var availableTimeSlots = new List<string>
    {
        "08:00-09:00", "09:00-10:00", "10:00-11:00", "11:00-12:00",
        "12:00-13:00", "13:00-14:00", "14:00-15:00", "15:00-16:00"
    };

            var bookedTimeSlots = appointments.Where(a => DateTime.ParseExact(a.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture) == date).Select(a => a.TimeSlot);

            return bookedTimeSlots.Count() == availableTimeSlots.Count;
        }

        public List<string> GetAvailableTimeSlots(DateTime date, List<Appointment> appointments)
        {
            var allTimeSlots = new List<string>
    {
        "08:00-09:00", "09:00-10:00", "10:00-11:00", "11:00-12:00",
        "12:00-13:00", "13:00-14:00", "14:00-15:00", "15:00-16:00"
    };

            var bookedTimeSlots = appointments.Where(a => DateTime.ParseExact(a.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture) == date).Select(a => a.TimeSlot);

            var availableTimeSlots = allTimeSlots.Except(bookedTimeSlots).ToList();

            return availableTimeSlots;
        }


        public async Task PrintAvailableTimeSlotsPerMonth()
        {
            var reservations = await _firebaseAuthService.GetReservationsAsync();
            var distinctDates = reservations.Select(r => DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture)).Distinct().OrderBy(r => r);
            var groupedDatesByMonth = distinctDates.GroupBy(d => new { d.Year, d.Month });

            foreach (var monthGroup in groupedDatesByMonth)
            {
                System.Diagnostics.Debug.WriteLine($"{monthGroup.First().ToString("MMMM")}:");

                foreach (var date in monthGroup)
                {
                    if (!AreAllTimeSlotsBooked(date, reservations))
                    {
                        var availableTimeSlots = GetAvailableTimeSlots(date, reservations);
                        System.Diagnostics.Debug.WriteLine($"  {date.ToString("dd.MM.yyyy")}: TimeSlots frei: {string.Join(", ", availableTimeSlots)}");
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"  {date.ToString("dd.MM.yyyy")}: Keine TimeSlots frei!");
                    }
                }
            }
        }



        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if (!_firebaseAuthService.IsSignedIn())
            {
                await Navigation.PushModalAsync(new LoginPage(_firebaseAuthService));
            }
            else
            {
                // Benutzerdaten abrufen
                currentUser = await _firebaseAuthService.GetCurrentUserAsync();
                if (currentUser != null)
                {
                    var userProfile = await _firebaseAuthService.GetUserProfileAsync(currentUser.Uid);

                    if (userProfile != null && isFirstTimeAppearing)
                    {
                        // Eingabefelder mit Benutzerdaten ausfüllen
                        FirstNameEntry.Text = userProfile.ContainsKey("FirstName") ? userProfile["FirstName"].ToString() : "";
                        LastNameEntry.Text = userProfile.ContainsKey("LastName") ? userProfile["LastName"].ToString() : "";
                        EmailEntry.Text = currentUser.Email;
                        PhoneNumberEntry.Text = userProfile.ContainsKey("PhoneNumber") ? userProfile["PhoneNumber"].ToString() : "";
                    }
                }
            }


            // Fetch user's appointments from Firebase and add them to the appointments collection
            var userAppointments = await GetUserAppointmentsAsync(currentUser.Uid);
            _appointments.Clear();
            foreach (var appointment in userAppointments.OrderBy(a => DateTime.ParseExact(a.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
            {
                _appointments.Add(appointment);
            }



            AppointmentsCollection.ItemsSource = _appointments;

            await PrintAvailableTimeSlotsPerMonth();

            isFirstTimeAppearing = false;


        }





    }
}