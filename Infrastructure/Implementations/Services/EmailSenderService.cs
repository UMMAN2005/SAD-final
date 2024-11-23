using System.Net;
using System.Net.Mail;
using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Implementations.Services;

public class EmailSenderService(IConfiguration configuration) : IEmailSenderService
{
    public async Task SendEmail(string email, string subject, string message)
    {

        var smtpClient = new SmtpClient(configuration["Smtp:Host"])
        {
            Port = int.Parse(configuration["Smtp:Port"] ?? string.Empty),
            Credentials = new NetworkCredential(configuration["Smtp:Username"], configuration["Smtp:Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(configuration["Smtp:SenderEmail"] ?? string.Empty, configuration["Smtp:SenderName"]),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };

        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }
}

