using MailKit.Net.Smtp;
using MimeKit;
namespace Task4.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
}
public class EmailService: IEmailService
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        try
        {
            var emailMessage = new MimeMessage();

            var smtpEmail = Environment.GetEnvironmentVariable("SMTP_EMAIL") ?? "";
            var smtpPass = Environment.GetEnvironmentVariable("SMTP_PASS") ?? "";
            var smtpServer = Environment.GetEnvironmentVariable("SMTP_SERVER") ?? "";
            var smtpPort = Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587";

            emailMessage.From.Add(new MailboxAddress("Confirm", smtpEmail));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("html") { Text = message };

            using var client = new SmtpClient();

            await client.ConnectAsync(smtpServer, int.Parse(smtpPort), MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(smtpEmail, smtpPass);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);
        }
        catch (SmtpCommandException ex)
        {
            // логируем, но НЕ падаем
            Console.WriteLine($"SMTP error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"General email error: {ex.Message}");
        }
    }
    
        
}