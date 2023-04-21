using Firebase.Database;
using Firebase.Database.Query;
using Newtonsoft.Json;
using System.Globalization;
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
                    Resource = item.Object.Resource,
                    Status = item.Object.Status
                });
            }

            return reservations;
        }





        public async Task UpdateReservationStatusAsync(string reservationId, string status)
        {
            await _client.Child("reservations").Child(reservationId).Child("status").PutAsync($"\"{status}\"");
        }



        public async Task DeleteReservationAsync(string reservationId)
        {
            await _client.Child("reservations").Child(reservationId).DeleteAsync();
        }


    }
}
