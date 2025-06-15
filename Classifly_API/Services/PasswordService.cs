using Classifly_API.Data;
using System.Security.Cryptography;

namespace Classifly_API.Services
{
    public class PasswordService : IPasswordService
    {
        // Asumsikan Anda menggunakan DbContext dari Entity Framework Core
        private readonly ClassiflyDbContext _dbContext;
        private readonly IEmailService _emailService;

        // Contoh constructor dengan dependency injection
        public PasswordService(ClassiflyDbContext dbContext, IEmailService emailService)
        {
            _dbContext = dbContext;
            _emailService = emailService;
        }

        public async Task InitiatePasswordResetAsync(string email)
        {
            // 1. Cari user berdasarkan email
            // Ganti 'User' dengan model entitas pengguna Anda
            // var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            // PENTING: Jangan beri tahu client jika email tidak ditemukan.
            // Ini untuk mencegah penyerang menebak email yang terdaftar.
            // if (user == null)
            // {
            //     return; // Keluar secara diam-diam
            // }

            // --- Simulasi jika user ditemukan ---
            var user = new { Email = email }; // Hapus baris ini saat menggunakan database nyata
            if (user == null) return;
            // ------------------------------------

            // 2. Buat token reset yang aman dan unik
            var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            // 3. Set waktu kadaluwarsa token (misalnya: 15 menit dari sekarang)
            var tokenExpiry = DateTime.UtcNow.AddMinutes(15);

            // 4. Simpan token dan waktu kadaluwarsanya ke database untuk user tersebut
            // user.PasswordResetToken = token;
            // user.ResetTokenExpires = tokenExpiry;
            // await _dbContext.SaveChangesAsync();

            // 5. Kirim email ke pengguna
            var resetLink = $"https://your-frontend-app.com/reset-password?token={Uri.EscapeDataString(token)}";
            var emailBody = $"<p>Klik link berikut untuk mereset password Anda: <a href='{resetLink}'>Reset Password</a></p>";

            // await _emailService.SendEmailAsync(user.Email, "Reset Password", emailBody);

            Console.WriteLine($"[SIMULASI] Email dikirim ke {email} dengan link: {resetLink}");
            await Task.CompletedTask;
        }

        public async Task<bool> FinalizePasswordResetAsync(string token, string newPassword)
        {
            // 1. Cari user berdasarkan token DAN pastikan token belum kadaluwarsa
            // var user = await _dbContext.Users.FirstOrDefaultAsync(u => 
            //     u.PasswordResetToken == token && 
            //     u.ResetTokenExpires > DateTime.UtcNow);

            // if (user == null)
            // {
            //     // Token tidak valid atau sudah kadaluwarsa
            //     return false;
            // }

            // --- Simulasi jika token valid ---
            var user = new { Token = token }; // Hapus baris ini saat menggunakan database nyata
            if (user == null) return false;
            // ------------------------------------

            // 2. Hash password baru sebelum disimpan
            // var hashedPassword = PasswordHasher.Hash(newPassword); // Ganti dengan library hashing Anda

            // 3. Update password user di database
            // user.PasswordHash = hashedPassword;

            // 4. HAPUS token agar tidak bisa digunakan lagi
            // user.PasswordResetToken = null;
            // user.ResetTokenExpires = null;

            // await _dbContext.SaveChangesAsync();

            Console.WriteLine($"[SIMULASI] Password untuk user dengan token {token} berhasil diubah.");
            await Task.CompletedTask;
            return true;
        }
    }
}
