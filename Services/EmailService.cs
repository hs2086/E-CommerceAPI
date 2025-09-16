using System.Net;
using System.Net.Mail;

namespace E_COMMERCEAPI.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(string receptor, string body);
    }
    public class EmailService : IEmailService
    {
        private readonly IConfiguration configuration;
        public EmailService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task SendEmailAsync(string receptor, string body)
        {
            var email = configuration.GetValue<string>("EMAIL_CONFIGURATION:EMAIL");
            var password = configuration.GetValue<string>("EMAIL_CONFIGURATION:PASSWORD");
            var host = configuration.GetValue<string>("EMAIL_CONFIGURATION:HOST");
            var port = configuration.GetValue<int>("EMAIL_CONFIGURATION:PORT");
            var smtpClient = new SmtpClient(host, port);
            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;

            smtpClient.Credentials = new NetworkCredential(email, password);

            string Subject = "Verify Account";
            string Body = body;
            var message = new MailMessage(email, receptor, Subject, Body);
            await smtpClient.SendMailAsync(message);
        }
    }
}
