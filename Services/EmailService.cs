using System.Net;
using System.Net.Mail;
using DotNetEnv;
using JobAlert.Logger;

namespace JobAlert.Services
{
    public class EmailService
    {
        private readonly FileConsoleLogger _logger = new();
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _test_end_user;
        

        public EmailService()
        {
            Env.Load();

            _smtpUsername = Environment.GetEnvironmentVariable("SMTP_USERNAME")
                            ?? throw new InvalidOperationException("SMTP_USERNAME is not set in the environment variables.");
            _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD")
                            ?? throw new InvalidOperationException("SMTP_PASSWORD is not set in the environment variables.");
            _smtpHost = Environment.GetEnvironmentVariable("SMTP_HOST")
                        ?? throw new InvalidOperationException("SMTP_HOST is not set in the environment variables.");
            var smtpPort = Environment.GetEnvironmentVariable("SMTP_PORT");

            if (smtpPort == null || !int.TryParse(smtpPort, out _smtpPort))
            {
                throw new InvalidOperationException("SMTP_PORT is not set correctly in the environment variables.");
            }
            _test_end_user = Environment.GetEnvironmentVariable("TEST_END_USER")
                            ?? throw new InvalidOperationException("TEST_END_USER is not set in the environment variables.");

        }

        public void SendEmail(string subject, string body)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = _smtpPort,
                    Credentials = new NetworkCredential(_smtpUsername, _smtpPassword),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_smtpUsername),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true,
                };
                mailMessage.To.Add(_test_end_user);

                smtpClient.Send(mailMessage);

                _logger.Info("Email sent successfully to " + _test_end_user);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error sending email: {ex.Message}", ex);
            }
        }
    }
}
