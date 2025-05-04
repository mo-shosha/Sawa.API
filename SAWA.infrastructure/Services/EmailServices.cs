using Microsoft.Extensions.Configuration;
using SAWA.core.IServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SAWA.infrastructure.Services
{
    public class EmailServices : IEmailServices
    {
        private readonly IConfiguration _configuration;
        public EmailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(toEmail) || !new EmailAddressAttribute().IsValid(toEmail))
            {
                throw new ArgumentException("Invalid email address.", nameof(toEmail));
            }
            try
            {
                // Access Gmail settings from configuration
                var email = _configuration["GmailSettings:Email"];
                var password = _configuration["GmailSettings:Password"];
                var host = _configuration["GmailSettings:Host"];
                var port = int.Parse(_configuration["GmailSettings:Port"]);

                // Configure SMTP client
                var smtpClient = new SmtpClient(host)
                {
                    Port = port,
                    Credentials = new NetworkCredential(email, password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(email),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                // Send the email
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                // Log the exception (you could use a logging library here)
                throw new Exception("Failed to send email. See inner exception for details.", ex);
            }
        }
    }
}
