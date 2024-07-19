using HR.LeaveManagement.Application.Contracts.Infrastructure;
using HR.LeaveManagement.Application.Models;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace HR.LeaveManagement.Infrastructure.Mail
{
    /// <summary>
    /// Handles email sending functionality using SendGrid.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private EmailSettings _emailSettings { get; }

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        /// <param name="emailSettings">Email settings configured in the application.</param>
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Sends an email
        /// </summary>
        /// <param name="email">Email details including recipient, subject, and body.</param>
        /// <returns>A task that represents the asynchronous operation that the email was sent successfully.</returns>
        public async Task<bool> SendEmail(Email email)
        {
            var client = new SendGridClient(_emailSettings.ApiKey);
            var to = new EmailAddress(email.To);
            var from = new EmailAddress
            {
                Email = _emailSettings.FromAddress,
                Name = _emailSettings.FromName
            };

            //Send mail
            var message = MailHelper.CreateSingleEmail(from, to, email.Subject, email.Body, email.Body);
            var response = await client.SendEmailAsync(message);

            return response.StatusCode == System.Net.HttpStatusCode.OK || response.StatusCode == System.Net.HttpStatusCode.Accepted;
        }
    }
}
