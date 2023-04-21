using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using web.Models;
using web.Services;
using Microsoft.Extensions.Logging;
using System.Globalization;

public class PendingReservationsModel : PageModel
{
    public List<Reservation> Reservations { get; set; }
    public List<Reservation> PendingReservations { get; set; }


    private readonly FirebaseService _firebaseService;
    private readonly EmailService _emailService;
    private readonly GoogleCalendarService _googleCalendarService;

    public PendingReservationsModel(FirebaseService firebaseService, EmailService emailService, IConfiguration configuration)
    {
        _firebaseService = firebaseService;
        Reservations = new List<Reservation>();

        _googleCalendarService = new GoogleCalendarService(configuration["Google:ServiceAccountKeyPath"]);
        _emailService = new EmailService(configuration["SendGrid:ApiKey"]);

    }

    public async Task<IActionResult> OnGet()
    {
        Reservations = await _firebaseService.GetReservationsAsync();
        DateTime currentTime = DateTime.Now;

        PendingReservations = Reservations.Where(r => r.Status == "Pending" &&
        DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture) >= currentTime.Date)
            .OrderBy(r => DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture))
            .ThenBy(r => TimeSpan.ParseExact(r.TimeSlot.Split('-')[0], @"hh\:mm", CultureInfo.InvariantCulture))
            .ToList();

        return Page();
    }


    public async Task<IActionResult> OnPostRejectAsync(string id)
    {
        System.Diagnostics.Debug.WriteLine($"OnPostRejectAsync called with ID: {id}");

        if (string.IsNullOrEmpty(id))
        {
            return Page();
        }

        var reservations = await _firebaseService.GetReservationsAsync();
        var reservation = reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
        {
            System.Diagnostics.Debug.WriteLine("Reservation not found");
            return NotFound();
        }

        await _firebaseService.UpdateReservationStatusAsync(id, "Rejected");

        string subject = "Reservation Rejected";
        string content = $@"
                     <h1>Reservation Rejected</h1>
                     <p>Dear {reservation.Name},</p>
                     <p>Unfortunately, your reservation for {reservation.Date} at {reservation.TimeSlot} has been rejected.</p>
                     <p>We apologize for any inconvenience this may cause.</p>";

        await _emailService.SendConfirmationEmailAsync(reservation.Email, subject, content);

        Reservations.Remove(reservation);

        return RedirectToPage();
    }


    public async Task<IActionResult> OnPostConfirmAsync(string id)
    {
        System.Diagnostics.Debug.WriteLine($"OnPostConfirmAsync called with ID: {id}");

        if (string.IsNullOrEmpty(id))
        {
            return Page();
        }

        //Fetch reservations directly from the Firebase service
        var reservations = await _firebaseService.GetReservationsAsync();
        var reservation = reservations.FirstOrDefault(r => r.Id == id);

        if (reservation == null)
        {
            System.Diagnostics.Debug.WriteLine("Reservation not found");
            return NotFound();
        }

        await _googleCalendarService.CreateEventAsync(reservation);

        string subject = "Reservation Confirmation";
        string content = $@"
                     <h1>Reservation Confirmed</h1>
                     <p>Dear {reservation.Name},</p>
                     <p>Your reservation for {reservation.Date} at {reservation.TimeSlot} has been confirmed.</p>
                     <p>Thank you for choosing our service!</p>";

        await _emailService.SendConfirmationEmailAsync(reservation.Email, subject, content);

        await _firebaseService.UpdateReservationStatusAsync(id, "Confirmed");

        Reservations.Remove(reservation);

        return RedirectToPage();
    }

}
