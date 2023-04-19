using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using web.Models;
using web.Services;

namespace web.Pages;

public class IndexModel : PageModel
{

    public List<Reservation> Reservations { get; set; }

    private readonly GoogleCalendarService _googleCalendarService;
    private readonly FirebaseService _firebaseService;
    private readonly EmailService _emailService;

    public IndexModel(GoogleCalendarService googleCalendarService, FirebaseService firebaseService, EmailService emailService)
    {
        _googleCalendarService = googleCalendarService;
        _firebaseService = firebaseService;
        _emailService = emailService;
        Reservations = new List<Reservation>();
    }

    public async Task<IActionResult> OnGet()
    {
        Reservations = await _firebaseService.GetReservationsAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostConfirmAsync(string id)
    {
        //Debug
        Debug.WriteLine($"Confirm button clicked for reservation ID: {id}");

        var reservation = Reservations.FirstOrDefault(r => r.Id == id);
        if (reservation != null)
        {
            //Debug
            Debug.WriteLine($"Reservation found: {reservation.FirstName} {reservation.LastName}");


            await _googleCalendarService.CreateEventAsync(reservation);

            string subject = "Reservation Confirmation";
            string content = $@"
                             <h1>Reservation Confirmed</h1>
                             <p>Dear {reservation.Name},</p>
                             <p>Your reservation for {reservation.Date} at {reservation.TimeSlot} has been confirmed.</p>
                             <p>Thank you for choosing our service!</p>";

            await _emailService.SendConfirmationEmailAsync(reservation.Email, subject, content);

            await _firebaseService.DeleteReservationAsync(id);

            Reservations.Remove(reservation);
        }
        else
        {
            //Debug
            Debug.WriteLine($"Reservation not found for ID: {id}");
        }

        return RedirectToPage();
    }


}
