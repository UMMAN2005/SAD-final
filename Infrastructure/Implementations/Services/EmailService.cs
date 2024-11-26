using Core.Interfaces.Services;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Infrastructure.Implementations.Services;

public class EmailService(IConfiguration configuration)
  : IEmailService {
  // ReSharper disable once UnusedMember.Local
  public static string Me => "ummanmemmedov2005@gmail.com";

  public async Task SendAsync(string to, string subject, string body) {
    var emailSettings = configuration.GetSection("EmailSettings");

    var email = new MimeMessage();

    email.From.Add(MailboxAddress.Parse(emailSettings["From"]));
    email.To.Add(MailboxAddress.Parse(to));
    email.Subject = subject;
    email.Body = new TextPart(TextFormat.Html) { Text = body };

    using var smtp = new SmtpClient();
    await smtp.ConnectAsync(emailSettings["Provider"], Convert.ToInt32(emailSettings["Port"]), SecureSocketOptions.SslOnConnect);
    await smtp.AuthenticateAsync(emailSettings["UserName"], emailSettings["Password"]);
    await smtp.SendAsync(email);
    await smtp.DisconnectAsync(true);
  }
}