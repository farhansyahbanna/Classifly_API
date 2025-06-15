using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Classifly_API.Services
{
    // Kelas ini untuk memodelkan konfigurasi dari appsettings.json
    public class SendGridSettings
    {
        public string ApiKey { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }

    public class SendGridEmailService : IEmailService
    {
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly SendGridSettings _sendGridSettings;

        // Menggunakan IOptions untuk mengambil konfigurasi dari appsettings.json
        public SendGridEmailService(IOptions<SendGridSettings> sendGridSettings, ILogger<SendGridEmailService> logger)
        {
            _sendGridSettings = sendGridSettings.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            if (string.IsNullOrEmpty(_sendGridSettings.ApiKey))
            {
                _logger.LogError("SendGrid ApiKey tidak dikonfigurasi.");
                return;
            }

            var client = new SendGridClient(_sendGridSettings.ApiKey);
            var from = new EmailAddress(_sendGridSettings.FromEmail, _sendGridSettings.FromName);
            var to = new EmailAddress(toEmail);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Email ke {toEmail} berhasil dikirim.");
            }
            else
            {
                _logger.LogError($"Gagal mengirim email ke {toEmail}. Status: {response.StatusCode}. Body: {await response.Body.ReadAsStringAsync()}");
            }
        }
    }
}
