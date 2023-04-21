using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Globalization;
using web.Models;

namespace web.Services
{
    public class GoogleCalendarService
    {
        private readonly CalendarService _calendarService;

        private readonly string _calendarId = @"a65fa70b8a066ff7aff016026e3e4c41a5896c6f95c2ca469ee8621720b5109a@group.calendar.google.com"; // Use "primary" for the user's primary calendar

        public GoogleCalendarService(string serviceAccountKeyPath)
        {
            GoogleCredential credential;
            using (var stream = new FileStream(serviceAccountKeyPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleCredential.FromStream(stream).CreateScoped(CalendarService.Scope.Calendar);
            }

            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "web",
            });
        }


        public async Task CreateEventAsync(Reservation reservation)
        {
            Console.WriteLine("Creating event...");
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
                    TimeZone = "Europe/Zurich",
                },
                End = new EventDateTime
                {
                    DateTime = startDate.Add(endTime.TimeOfDay),
                    TimeZone = "Europe/Zurich",
                },
            };

            // Insert the event into the user's calendar
            var request = _calendarService.Events.Insert(calendarEvent, _calendarId).Execute();
            //var createdEvent = await request.ExecuteAsync();
            System.Diagnostics.Debug.WriteLine($"Event created: {request.HtmlLink}"); 
        }


    }
}
