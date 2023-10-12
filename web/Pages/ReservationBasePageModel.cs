using Microsoft.AspNetCore.Mvc.RazorPages;
using web.Models;
using web.Services;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

public abstract class ReservationBasePageModel : PageModel
{
    public List<Reservation> Reservations { get; set; }

    protected readonly FirebaseService _firebaseService;
    protected readonly EmailService _emailService;
    protected readonly GoogleCalendarService _googleCalendarService;

    protected ReservationBasePageModel(FirebaseService firebaseService, EmailService emailService, IConfiguration configuration)
    {
        _firebaseService = firebaseService;
        Reservations = new List<Reservation>();

        _googleCalendarService = new GoogleCalendarService(configuration["Google:ServiceAccountKeyPath"]);
        _emailService = new EmailService(configuration["SendGrid:ApiKey"]);
    }

    protected async Task FetchReservationsAsync()
    {
        Reservations = await _firebaseService.GetReservationsAsync();
    }

    protected List<Reservation> GetUpcomingReservations(DateTime currentTime, string status = null)
    {
        var reservations = Reservations.Where(r =>
            (DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture).Date > currentTime.Date ||
            (DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture).Date == currentTime.Date &&
            TimeSpan.ParseExact(r.TimeSlot.Split('-')[0], @"hh\:mm", CultureInfo.InvariantCulture) > currentTime.TimeOfDay)));

        if (!string.IsNullOrEmpty(status))
        {
            reservations = reservations.Where(r => r.Status == status);
        }

        return reservations.OrderBy(r => DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture))
           .ThenBy(r => TimeSpan.ParseExact(r.TimeSlot.Split('-')[0], @"hh\:mm", CultureInfo.InvariantCulture))
           .ToList();
    }

    protected List<Reservation> GetPendingReservations(DateTime currentTime)
    {
        return Reservations.Where(r => r.Status == "Pending" &&
            DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture) >= currentTime.Date)
                .OrderBy(r => DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture))
                .ThenBy(r => TimeSpan.ParseExact(r.TimeSlot.Split('-')[0], @"hh\:mm", CultureInfo.InvariantCulture))
                .ToList();
    }


    protected List<Reservation> GetPastReservations(DateTime currentTime)
    {
        return Reservations.Where(r => DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture).Date < currentTime.Date || (DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture).Date == currentTime.Date && TimeSpan.ParseExact(r.TimeSlot.Split('-')[0], @"hh\:mm", CultureInfo.InvariantCulture) < currentTime.TimeOfDay))
            .OrderByDescending(r => DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture))
            .ThenByDescending(r => TimeSpan.ParseExact(r.TimeSlot.Split('-')[0], @"hh\:mm", CultureInfo.InvariantCulture))
            .ToList();
    }

    protected List<Reservation> GetUnconfirmedPastReservations(DateTime currentTime)
    {
        return Reservations.Where(r =>
            r.Status == "Pending" &&
            (DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture).Date < currentTime.Date ||
            (DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture).Date == currentTime.Date &&
            TimeSpan.ParseExact(r.TimeSlot.Split('-')[0], @"hh\:mm", CultureInfo.InvariantCulture) < currentTime.TimeOfDay)))
           .OrderByDescending(r => DateTime.ParseExact(r.Date, "dd.MM.yyyy", CultureInfo.InvariantCulture))
           .ThenByDescending(r => TimeSpan.ParseExact(r.TimeSlot.Split('-')[0], @"hh\:mm", CultureInfo.InvariantCulture))
           .ToList();
    }


    protected async Task<IActionResult> HandleRejectAsync(string id)
    {
        System.Diagnostics.Debug.WriteLine($"HandleRejectAsync called with ID: {id}");

        if (string.IsNullOrEmpty(id))
        {
            return Page();
        }

        await FetchReservationsAsync();
        var reservation = Reservations.FirstOrDefault(r => r.Id == id);

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

    protected async Task<IActionResult> HandleConfirmAsync(string id)
    {
        System.Diagnostics.Debug.WriteLine($"HandleConfirmAsync called with ID: {id}");

        if (string.IsNullOrEmpty(id))
        {
            return Page();
        }

        await FetchReservationsAsync();
        var reservation = Reservations.FirstOrDefault(r => r.Id == id);

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
