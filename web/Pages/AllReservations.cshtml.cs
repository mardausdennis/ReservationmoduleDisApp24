using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using web.Models;
using web.Services;
using Microsoft.Extensions.Logging;
using System.Globalization;

public class AllReservationsModel : ReservationBasePageModel
{
    public List<Reservation> PastReservations { get; set; }
    public List<Reservation> UpcomingReservations { get; set; }
    public List<Reservation> UnconfirmedPastReservations { get; set; }

    public AllReservationsModel(FirebaseService firebaseService, EmailService emailService, IConfiguration configuration)
        : base(firebaseService, emailService, configuration) 
    { }

    public async Task<IActionResult> OnGet()
    {
        await FetchReservationsAsync();

        DateTime currentTime = DateTime.Now;

        UpcomingReservations = GetUpcomingReservations(currentTime);
        PastReservations = GetPastReservations(currentTime);
        UnconfirmedPastReservations = GetUnconfirmedPastReservations(currentTime);

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
