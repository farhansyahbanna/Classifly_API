namespace Classifly_API.Services
{
    /// <summary>
    /// Mendefinisikan kontrak untuk layanan pengiriman email.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Mengirim email secara asynchronous.
        /// </summary>
        /// <param name="toEmail">Alamat email penerima.</param>
        /// <param name="subject">Judul email.</param>
        /// <param name="htmlMessage">Isi email dalam format HTML.</param>
        /// <returns>Task yang menandakan operasi telah selesai.</returns>
        Task SendEmailAsync(string toEmail, string subject, string htmlMessage);
        //Task SendPasswordResetEmailAsync(string email, string resetLink);
    }
}
