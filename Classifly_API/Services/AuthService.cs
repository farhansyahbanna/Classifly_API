using Classifly_API.Data;
using Classifly_API.DTOs.Requests;
using Classifly_API.DTOs.Responses;
using Classifly_API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Classifly_API.Services
{
    public class AuthService
    {
        private readonly ClassiflyDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(ClassiflyDbContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<User> Register(User user, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.CreatedAt = DateTime.UtcNow;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task<UserLoginResponse> Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || !VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            {
                return null;
            }

            // Generate token (simplified for example)
            var token = GenerateJwtToken(user);

            return new UserLoginResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role,
                FullName = user.FullName,
                Token = token
            };
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:Token"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha512Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<IEnumerable<UserResponseDto>> GetAllUsers()
        {
            var users = await _context.Users
                .OrderBy(u => u.FullName) 
                .Select(u => new UserResponseDto 
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FullName = u.FullName,
                    Role = u.Role,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return users;
        }

        public async Task<int> GetTotalUsersCount()
        {
  
            return await _context.Users.CountAsync();
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }

        public async Task<User> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task ChangePassword(int userId, string newPassword)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.SaveChangesAsync();
        }

        public async Task<string?> InitiatePasswordReset(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return null;
            }

            var token = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));

            user.PasswordResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(15);

            await _context.SaveChangesAsync();
            return token;
        }

        public async Task<bool> ResetPassword(string token, string newPassword)
        {
            // Cari user berdasarkan token yang valid dan belum kedaluwarsa
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == token &&
                u.ResetTokenExpires > DateTime.UtcNow);

            if (user == null)
            {
                // Token tidak valid atau sudah kedaluwarsa
                return false;
            }

            // Buat hash & salt untuk password baru
            CreatePasswordHash(newPassword, out byte[] passwordHash, out byte[] passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // PENTING: Hapus token setelah digunakan agar tidak bisa dipakai lagi
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return true;
        }



        public async Task<(bool Succeeded, string? ErrorMessage, UserResponseDto? User)> CreateUserByAdminAsync(AdminCreateUserDto userDto)
        {
            // 1. Cek apakah username atau email sudah ada
            if (await _context.Users.AnyAsync(u => u.Username == userDto.Username))
            {
                return (false, "Username sudah digunakan.", null);
            }

            if (await _context.Users.AnyAsync(u => u.Email == userDto.Email))
            {
                return (false, "Email sudah terdaftar.", null);
            }

            // 2. Buat objek User baru
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                FullName = userDto.FullName,
                Role = userDto.Role, 
                CreatedAt = DateTime.UtcNow
            };

       
            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;


            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var createdUserDto = new UserResponseDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role,
                CreatedAt = user.CreatedAt
            };

            return (true, null, createdUserDto);
        }

        // --- TAHAP 1: MEMINTA OTP ---
        public async Task<bool> RequestPasswordResetOtpAsync(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
  
                return true;
            }

    
            var otp = new Random().Next(100000, 999999).ToString();

            user.PasswordResetToken = otp;
            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(10); 

            await _context.SaveChangesAsync();

   
            var emailBody = $"<p>Gunakan kode berikut untuk mereset password Anda. Kode ini berlaku selama 10 menit.</p><h2>{otp}</h2>";
            await _emailService.SendEmailAsync(user.Email, "Kode OTP Reset Password Anda", emailBody);

            return true;
        }

         // --- TAHAP 2: VERIFIKASI OTP & BUAT TIKET RESET ---
        public async Task<string?> VerifyOtpAndGenerateResetTokenAsync(VerifyOtpRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        
            if (user == null || user.PasswordResetToken != request.Otp || user.ResetTokenExpires < DateTime.UtcNow)
            {
                return null; // Verifikasi gagal
            }

        
            var finalResetToken = Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        
            user.PasswordResetToken = finalResetToken; 
            user.ResetTokenExpires = DateTime.UtcNow.AddMinutes(15); 

            await _context.SaveChangesAsync();

            return finalResetToken;
        }
        // --- TAHAP 3: RESET PASSWORD DENGAN TIKET ---
        public async Task<bool> ResetPasswordWithTokenAsync(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.PasswordResetToken == request.Token &&
                u.ResetTokenExpires > DateTime.UtcNow);

            if (user == null)
            {
                return false; 
            }

            CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();
            return true;
        }
    }

}
