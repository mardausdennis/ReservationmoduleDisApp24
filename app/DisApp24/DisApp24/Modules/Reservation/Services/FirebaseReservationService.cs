using System.Collections.Generic;
using System.Threading.Tasks;
using DisApp24.Models;
using Firebase.Database;

namespace DisApp24.Services
{
    public class FirebaseReservationService : IFirebaseReservationService
    {
        private readonly FirebaseClient _firebaseClient;
        private readonly AppConfig _config;

        public FirebaseReservationService()
        {
            _config = ServiceHelper.GetService<AppConfig>();
            _firebaseClient = new FirebaseClient(_config.FirebaseUrl);
        }

        public async Task<List<Appointment>> GetReservationsAsync()
        {
            var firebaseAppointments = await _firebaseClient
                .Child("reservations")
                .OnceAsync<Appointment>();

            var appointments = new List<Appointment>();
            foreach (var item in firebaseAppointments)
            {
                var appointment = item.Object;
                appointment.Key = item.Key;
                appointments.Add(appointment);
            }

            return appointments;
        }
    }
}
