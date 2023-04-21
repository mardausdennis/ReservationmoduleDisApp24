using SendGrid.Helpers.Mail;
using SendGrid;
using static Google.Apis.Requests.BatchRequest;

namespace web.Services
{
    public class EmailService
    {
        private readonly string _sendGridApiKey;

        public EmailService(string sendGridApiKey)
        {
            _sendGridApiKey = sendGridApiKey;
        }

        public async Task SendConfirmationEmailAsync(string to, string subject, string htmlContent)
        {
            var client = new SendGridClient(_sendGridApiKey);
            var from = new EmailAddress("dennis.mardaus@gmail.com", "DisApp24");
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, null, htmlContent);

            var response = await client.SendEmailAsync(msg);

            // Log the status code and response body for debugging purposes
            System.Diagnostics.Debug.WriteLine($"SendGrid response status code: {response.StatusCode}");
            System.Diagnostics.Debug.WriteLine($"SendGrid response body: {await response.Body.ReadAsStringAsync()}");
        }
    }
}
