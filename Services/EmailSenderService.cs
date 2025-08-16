using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace CampeonatinhoApp.Services
{
    public class EmailSenderService : IEmailSender
    {
        private readonly ILogger<EmailSenderService> _logger;
        public EmailSenderService(ILogger<EmailSenderService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string htmlMessage)
        {
            _logger.LogInformation($"Sending email to {to}.");

            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(Environment.GetEnvironmentVariable("EmailSettings__SenderName"), Environment.GetEnvironmentVariable("EmailSettings__FromEmail")));
            email.To.Add(new MailboxAddress(to, to));

            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlMessage
            };

            using (var smtp = new SmtpClient())
            {
                smtp.Connect(Environment.GetEnvironmentVariable("EmailSettings__MailServer"), Convert.ToInt16(Environment.GetEnvironmentVariable("EmailSettings__MailPort")), false);
                smtp.Authenticate(Environment.GetEnvironmentVariable("EmailSettings__FromEmail"), Environment.GetEnvironmentVariable("EmailSettings__Password"));

                smtp.Send(email);
                smtp.Disconnect(true);
            }

            _logger.LogInformation($"Email sent without errors.");
            return Task.CompletedTask;
        }
    }
}
