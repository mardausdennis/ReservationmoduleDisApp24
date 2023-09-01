using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using XCalendar.Core.Extensions;
using XCalendar.Core.Models;
using XCalendar.Core.Enums;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using DisApp24.Services;
using CommunityToolkit.Mvvm.Messaging;
using DisApp24.Models;
using MvvmHelpers.Commands;

namespace DisApp24.ViewModels
{
    public class CalendarPageViewModel : BaseViewModel
    {

        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IFirebaseReservationService _firebaseReservationService;
        private readonly INavigationService _navigation;


        public ObservableCollection<DateTime> SelectedDates { get; set; } = new ObservableCollection<DateTime>();
        public CalendarDay OutsideCalendarDay { get; set; } = new CalendarDay();
        public ICommand NavigateCalendarCommand { get; set; }

        public ICommand ChangeDateSelectionCommand { get; set; }

        public ICommand DateSelectedCommand { get; set; }
        public ICommand DateConfirmedCommand { get; set; }
        public ICommand DisplaySelectedDatesCommand { get; set; }



        public ObservableCollection<DateTime> FullyBookedDates { get; set; } = new ObservableCollection<DateTime>();
        public Calendar<CalendarDay> Calendar { get; set; } = new Calendar<CalendarDay>()
        {
            SelectionType = SelectionType.Single,
            SelectionAction = SelectionAction.Replace
        };
        

        #region Constructors
        public CalendarPageViewModel()
        {
            _firebaseAuthService = ServiceHelper.GetService<IFirebaseAuthService>();
            _firebaseReservationService = ServiceHelper.GetService<IFirebaseReservationService>();
            _navigation = ServiceHelper.GetService<INavigationService>();

            ChangeDateSelectionCommand = new Microsoft.Maui.Controls.Command<DateTime>(ChangeDateSelection);
            NavigateCalendarCommand = new Microsoft.Maui.Controls.Command<int>(NavigateCalendar);

            Calendar.DaysUpdated += Calendar_DaysUpdated;
            Calendar.UpdateDay(OutsideCalendarDay, Calendar.NavigatedDate);

            DateSelectedCommand = new Microsoft.Maui.Controls.Command<DateTime>(OnDateSelected);
            DateConfirmedCommand = new AsyncCommand(OnDateConfirmed);



            Task.Run(async () => await LoadAppointmentsAsync());
        }

        #endregion
        #region Methods

        private void UpdateInvalidDays()
        {
            foreach (var day in Calendar.Days)
            {
                day.IsInvalid = FullyBookedDates.Contains(day.DateTime);
            }
        }



        public bool AreAllTimeSlotsBooked(DateTime date, List<Appointment> appointments)
        {
            var availableTimeSlots = new List<string>
    {
        "08:00-09:00", "09:00-10:00", "10:00-11:00", "11:00-12:00",
        "12:00-13:00", "13:00-14:00", "14:00-15:00", "15:00-16:00"
    };

            var bookedTimeSlots = appointments.Where(a => DateTime.ParseExact(a.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture) == date).Select(a => a.TimeSlot);

            var areAllTimeSlotsBooked = bookedTimeSlots.Count() == availableTimeSlots.Count;
            System.Diagnostics.Debug.WriteLine($"Date: {date.ToString("dd.MM.yyyy")}, Fully Booked: {areAllTimeSlotsBooked}");

            return areAllTimeSlotsBooked;
        }


        public void UpdateFullyBookedDates(List<Appointment> appointments)
        {
            FullyBookedDates.Clear();

            // Extrahieren Sie alle eindeutigen Tage, an denen Termine vorhanden sind.
            var appointmentDates = appointments.Select(a => DateTime.ParseExact(a.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture))
                                               .Distinct();

            // Überprüfen Sie nur für diese Tage, ob alle Zeitschlitze ausgebucht sind.
            foreach (var date in appointmentDates)
            {
                if (AreAllTimeSlotsBooked(date, appointments))
                {
                    FullyBookedDates.Add(date);
                }
            }

            // Aktualisieren Sie die IsInvalid-Eigenschaft der Tage im Kalender.
            UpdateInvalidDays();
        }




        public async Task LoadAppointmentsAsync()
        {
            // Laden Sie hier die Termine (List<Appointment> appointments) aus Ihrer Datenquelle.
            var appointments = await _firebaseReservationService.GetReservationsAsync();


            System.Diagnostics.Debug.WriteLine($"Appointments: {string.Join(", ", appointments.Select(a => $"{a.Date} - {a.TimeSlot}"))}");
            // Aktualisieren Sie die Liste der ausgebuchten Tage, indem Sie die UpdateFullyBookedDates-Methode aufrufen
            UpdateFullyBookedDates(appointments);
        }



        public void NavigateCalendar(int amount)
        {
            if (Calendar.NavigatedDate.TryAddMonths(amount, out DateTime targetDate))
            {
                Calendar.Navigate(targetDate - Calendar.NavigatedDate);
            }
            else
            {
                Calendar.Navigate(amount > 0 ? TimeSpan.MaxValue : TimeSpan.MinValue);
            }
        }
        public void ChangeDateSelection(DateTime dateTime)
        {
            if (SelectedDates.Contains(dateTime))
            {
                SelectedDates.Remove(dateTime);
            }
            else
            {
                SelectedDates.Clear();
                SelectedDates.Add(dateTime);
            }
            Calendar?.ChangeDateSelection(dateTime);

            OnPropertyChanged(nameof(SelectedDatesString));
        }


        private void Calendar_DaysUpdated(object sender, EventArgs e)
        {
            UpdateInvalidDays();
            Calendar.UpdateDay(OutsideCalendarDay, Calendar.NavigatedDate);
        }


        public string SelectedDatesString
        {
            get
            {
                if (SelectedDates.Any())
                {
                    return SelectedDates.First().ToString("dd/MM/yyyy");
                }
                return "Kein Datum ausgewählt";
            }
        }


        #endregion

        private async Task OnDateConfirmed()
        {
            if (SelectedDates != null && SelectedDates.Any())
            {
                OnDateSelected(SelectedDates.First());
                await _navigation.GetNavigation().PopAsync();

            }
            else
            {
                // Send a message to the view to show the alert
                WeakReferenceMessenger.Default.Send(new ShowAlertMessage { Title = "Error", Message = "Please select a date before confirming.", ButtonText = "OK" });
            }
        }

        private void OnDateSelected(DateTime date)
        {
            WeakReferenceMessenger.Default.Send(new SelectedDateMessage { Date = date });
        }



    }

    public class SelectedDateMessage
    {
        public DateTime Date { get; set; }
    }
}
