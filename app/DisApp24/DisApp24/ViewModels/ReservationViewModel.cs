using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Helpers;
using DisApp24.Services;
using Firebase.Database;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DisApp24.ViewModels
{
    public class ReservationViewModel : BaseViewModel
    {

        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IFirebaseReservationService _firebaseReservationService;
        private readonly FirebaseClient _firebaseClient = new FirebaseClient("https://disapp24-reservation-module-default-rtdb.europe-west1.firebasedatabase.app");
        private AppUser currentUser;

        public ICommand ReserveCommand => new AsyncRelayCommand(ReserveAsync);
        public IAsyncRelayCommand InitializeCommand { get; }

        private ObservableCollection<Appointment> _appointments;
        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set => SetProperty(ref _appointments, value);
        }

        public string Resource { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string SelectedDate { get; set; }
        public string TimeSlot { get; set; }
        public string Comment { get; set; }

        private string _validationMessage;
        public string ValidationMessage
        {
            get { return _validationMessage; }
            set { SetProperty(ref _validationMessage, value); }
        }

        public ReservationViewModel()
        {
            _firebaseAuthService = ServiceHelper.GetService<IFirebaseAuthService>();
            _firebaseReservationService = ServiceHelper.GetService<IFirebaseReservationService>();
            WeakReferenceMessenger.Default.Register<SelectedDateMessage>(this, OnSelectedDateMessageReceived);

            // Initialize the appointments collection
            _appointments = new ObservableCollection<Appointment>();

        }

        private void OnSelectedDateMessageReceived(object recipient, SelectedDateMessage message)
        {
            SelectedDate = message.Date.ToString("dd.MM.yyyy");
            System.Diagnostics.Debug.WriteLine($"Selected date updated: {SelectedDate}");
        }

        private async Task ReserveAsync()
        {
            if (ValidateInput())
            {

                bool isUserAccountValid = await _firebaseAuthService.IsUserAccountValid(currentUser.Uid);
                if (!isUserAccountValid)
                {
                    // Zeige eine Fehlermeldung an und kehre zur Anmeldeseite zurück
                    await Shell.Current.DisplayAlert("Fehler", "Ihr Benutzerkonto ist nicht mehr gültig. Bitte melden Sie sich erneut an.", "OK");
                    await AppShell.Current.GoToAsync(nameof(LoginPage));
                    return;
                }

                var reservationData = new
                {
                    resource = Resource,
                    firstName = FirstName,
                    lastName = LastName,
                    email = Email,
                    phoneNumber = PhoneNumber,
                    date = SelectedDate,
                    timeSlot = TimeSlot,
                    comment = Comment,
                    status = "Pending",
                    userId = currentUser.Uid
                };

                var json = JsonConvert.SerializeObject(reservationData);

                try
                {
                    await _firebaseClient.Child("reservations").PostAsync(json);
                    await Shell.Current.DisplayAlert("Erfolg", "Ihre Reservierungsanfrage wurde erfolgreich gesendet. Sie erhalten eine Bestätigung per E-Mail, sobald Ihre Reservierung bestätigt wurde.", "OK");
                }
                catch (Exception ex)
                {
                    await Shell.Current.DisplayAlert("Fehler", "Es gab einen Fehler beim Senden Ihrer Reservierungsanfrage. Bitte versuchen Sie es später erneut.", "OK");
                }
            }
        }

        //Initialize

        public async Task InitializeDataAsync(AppUser user)
        {
            currentUser = user;

            // Fetch user's appointments from Firebase and add them to the appointments collection
            var userAppointments = await GetUserAppointmentsAsync(currentUser.Uid);

            Appointments.Clear();

            foreach (var appointment in userAppointments.OrderBy(a => DateTime.ParseExact(a.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture)))
            {
                Appointments.Add(appointment);
            }

            await PrintAvailableTimeSlotsPerMonth();
        }




        public ObservableCollection<string> Resources { get; } = new ObservableCollection<string>
{
    "Ressource 1",
    "Ressource 2",
    "Ressource 3"
};

        public ObservableCollection<string> TimeSlots { get; } = new ObservableCollection<string>
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



        //Validate Input

        public bool IsValidResource => Resource != null;
        public bool IsValidFirstName => !string.IsNullOrWhiteSpace(FirstName);
        public bool IsValidLastName => !string.IsNullOrWhiteSpace(LastName);
        public bool IsValidEmail => !string.IsNullOrWhiteSpace(Email) && InputValidationHelper.IsValidEmail(Email);
        public bool IsValidPhoneNumber => string.IsNullOrWhiteSpace(PhoneNumber) || InputValidationHelper.IsValidPhoneNumber(PhoneNumber);
        public bool IsValidDate => !string.IsNullOrWhiteSpace(SelectedDate) && SelectedDate != "Datum auswählen";
        public bool IsValidTime => TimeSlot != null;

        public bool IsValid => IsValidFirstName && IsValidLastName && IsValidEmail && IsValidPhoneNumber && IsValidResource && IsValidDate && IsValidTime;


        public bool IsResourceFrameInvalid { get; set; }
        public bool IsFirstNameFrameInvalid { get; set; }
        public bool IsLastNameFrameInvalid { get; set; }
        public bool IsEmailFrameInvalid { get; set; }
        public bool IsPhoneNumberFrameInvalid { get; set; }
        public bool IsDateInvalid { get; set; }
        public bool IsTimeFrameInvalid { get; set; }


        private bool ValidateInput()
        {

            IsResourceFrameInvalid = !IsValidResource;
            IsFirstNameFrameInvalid = !IsValidFirstName;
            IsLastNameFrameInvalid = !IsValidLastName;
            IsEmailFrameInvalid = !IsValidEmail;
            IsPhoneNumberFrameInvalid = !IsValidPhoneNumber;
            IsDateInvalid = !IsValidDate;
            IsTimeFrameInvalid = !IsValidTime;


            List<string> errorMessages = new List<string>();

            if (!IsValidResource)
            {
                errorMessages.Add("Bitte wählen Sie eine Ressource aus.");
            }
            if (!IsValidFirstName)
            {
                errorMessages.Add("Bitte geben Sie Ihren Vornamen ein.");
                
            }
            if (!IsValidLastName)
            {
                errorMessages.Add("Bitte geben Sie Ihren Nachnamen ein.");
            }
            if (!IsValidEmail)
            {
                errorMessages.Add("Bitte geben Sie eine gültige E-Mail-Adresse ein.");
            }
            if (!IsValidPhoneNumber)
            {
                errorMessages.Add("Bitte geben Sie eine gültige Telefonnummer ein.");
            }
            if (!IsValidDate)
            {
                errorMessages.Add("Bitte wählen Sie ein Datum aus.");
            }
            if (!IsValidTime)
            {
                errorMessages.Add("Bitte wählen Sie eine Uhrzeit aus.");
            }

            ValidationMessage = string.Join("\n", errorMessages);

            return IsValid;
        }



        public async Task<List<Appointment>> GetUserAppointmentsAsync(string userId)
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
            var reservations = await _firebaseReservationService.GetReservationsAsync();
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

    }
}
