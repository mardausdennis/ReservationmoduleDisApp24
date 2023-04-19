using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using System.Globalization;
using web.Models;

namespace web.Services
{
    public class GoogleCalendarService
    {
        private readonly CalendarService _calendarService;
        private readonly string _calendarId = "primary"; // Use "primary" for the user's primary calendar

        public GoogleCalendarService(string apiKey)
        {
            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                ApiKey = apiKey,
                ApplicationName = "web",
            });
        }

        public async Task CreateEventAsync(Reservation reservation)
        {
            // Parse date and time
            DateTime startDate = DateTime.ParseExact(reservation.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            string[] timeRange = reservation.TimeSlot.Split('-');
            DateTime startTime = DateTime.ParseExact(timeRange[0], "HH:mm", CultureInfo.InvariantCulture);
            DateTime endTime = DateTime.ParseExact(timeRange[1], "HH:mm", CultureInfo.InvariantCulture);

            // Create the event
            Event calendarEvent = new Event
            {
                Summary = $"Reservation for {reservation.Resource}",
                Location = "Austria",
                Description = $"Reservation made by {reservation.FirstName} {reservation.LastName}",
                Start = new EventDateTime
                {
                    DateTime = startDate.Add(startTime.TimeOfDay),
                    TimeZone = "UTC",
                },
                End = new EventDateTime
                {
                    DateTime = startDate.Add(endTime.TimeOfDay),
                    TimeZone = "UTC",
                },
            };

            // Insert the event into the user's calendar
            var request = _calendarService.Events.Insert(calendarEvent, _calendarId);
            await request.ExecuteAsync();
        }

    }
}
