using SendGrid.Helpers.Mail;
using SendGrid;

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
            var from = new EmailAddress("DisApp24-noreply@love4quality.com", "DisApp24");
            var toEmail = new EmailAddress(to);
            var msg = MailHelper.CreateSingleEmail(from, toEmail, subject, null, htmlContent);

            await client.SendEmailAsync(msg);
        }
    }
}
