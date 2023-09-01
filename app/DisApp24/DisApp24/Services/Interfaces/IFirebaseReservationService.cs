using System.Collections.Generic;
using DisApp24.Models;

namespace DisApp24.Services
{
    public interface IFirebaseReservationService
    {
        Task<List<Appointment>> GetReservationsAsync();
    }
}
