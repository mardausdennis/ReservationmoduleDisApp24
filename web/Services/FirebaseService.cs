using Firebase.Database;
using web.Models;

namespace web.Services
{
    public class FirebaseService
    {
        private readonly FirebaseClient _client;

        public FirebaseService(string apiKey, string authDomain, string databaseUrl)
        {
            _client = new FirebaseClient(databaseUrl, new FirebaseOptions
            {
                AuthTokenAsyncFactory = () => Task.FromResult(apiKey)
            });
        }

        public async Task<List<Reservation>> GetReservationsAsync()
        {
            var reservations = new List<Reservation>();
            var firebase = _client.Child("reservations");
            var snapshot = await firebase.OnceAsync<Reservation>();

            foreach (var item in snapshot)
            {
                reservations.Add(new Reservation
                {
                    Id = item.Key,
                    FirstName = item.Object.FirstName,
                    LastName = item.Object.LastName,
                    Email = item.Object.Email,
                    PhoneNumber = item.Object.PhoneNumber,
                    Date = item.Object.Date,
                    TimeSlot = item.Object.TimeSlot,
                    Resource = item.Object.Resource
                });
            }

            return reservations;
        }
        public async Task DeleteReservationAsync(string reservationId)
        {
            // Your existing code to remove a reservation from Firebase
        }

    }
}
