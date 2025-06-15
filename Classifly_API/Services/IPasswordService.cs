namespace Classifly_API.Services
{
    public interface IPasswordService
    {
        /// <summary>
        /// Memulai proses lupa password.
        /// Menghasilkan token, menyimpannya di database, dan mengirim email ke pengguna.
        /// </summary>
        /// <param name="email">Email pengguna yang meminta reset.</param>
        /// <returns>Task</returns>
        Task InitiatePasswordResetAsync(string email);

        /// <summary>
        /// Menyelesaikan proses reset password menggunakan token.
        /// </summary>
        /// <param name="token">Token reset yang diterima dari email.</param>
        /// <param name="newPassword">Password baru yang akan di-hash dan disimpan.</param>
        /// <returns>True jika berhasil, false jika token tidak valid atau kadaluwarsa.</returns>
        Task<bool> FinalizePasswordResetAsync(string token, string newPassword);
    }
}
