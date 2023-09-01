using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using web.Models;
using web.Services;
using Microsoft.Extensions.Logging;
using System.Globalization;

public class PendingReservationsModel : ReservationBasePageModel
{
    public List<Reservation> PendingReservations { get; set; }
    public List<Reservation> UpcomingPendingReservations { get; set; }
    public List<Reservation> PastUnconfirmedReservations { get; set; }

    public PendingReservationsModel(FirebaseService firebaseService, EmailService emailService, IConfiguration configuration)
        : base(firebaseService, emailService, configuration)
    { }

    public async Task<IActionResult> OnGet()
    {
        await FetchReservationsAsync();
        DateTime currentTime = DateTime.Now;

        PendingReservations = GetPendingReservations(currentTime);
        UpcomingPendingReservations = GetUpcomingReservations(currentTime, "Pending");
        PastUnconfirmedReservations = GetUnconfirmedPastReservations(currentTime);

        return Page();
    }

    public async Task<IActionResult> OnPostRejectAsync(string id)
    {
        return await HandleRejectAsync(id);
    }

    public async Task<IActionResult> OnPostConfirmAsync(string id)
    {
        return await HandleConfirmAsync(id);
    }
}
