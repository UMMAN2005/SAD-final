namespace Core.Interfaces.Services;
public interface IEmailSenderService {
  Task SendEmail(string email, string subject, string body);
}