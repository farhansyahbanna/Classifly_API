using Classifly_API.Data;
using Classifly_API.Models;
using Microsoft.EntityFrameworkCore;
using Classifly_API.DTOs.Responses;
using Classifly_API.DTOs.Requests;

namespace Classifly_API.Services
{
    public class DamageReportService
    {
        private readonly ClassiflyDbContext _context;

        public DamageReportService(ClassiflyDbContext context)
        {
            _context = context;
        }

        public async Task<DamageReportResponse> CreateDamageReport(DamageReport damageReport)
        {
            // Verify Borrow Request exists
            var borrowRequest = await _context.BorrowRequests.FindAsync(damageReport.BorrowRequestId);
            if (borrowRequest == null)
                throw new Exception("Peminjaman Barang Tidak Ditemukan");

            // Verify User exists
            var user = await _context.Users.FindAsync(damageReport.UserId);
            if (user == null)
                throw new Exception("User Tidak ditemukan");

            // // Verify Borrow Request is approved
 
            if (borrowRequest.Status.ToLower() != "approved")
            {
                throw new Exception("Peminjaman Harus Sudah Di Approved");
            }

            // Validate image URL
            if (string.IsNullOrEmpty(damageReport.ImageUrl))
            {
                damageReport.ImageUrl = null;
            }
            else if (damageReport.ImageUrl.Length > 255)
            {
                throw new Exception("Image URL is too long");
            }

            // Default values
            if (string.IsNullOrEmpty(damageReport.Location))
            {
                damageReport.Location = "Unknown";
            }

            damageReport.AdminMessage = null;
            damageReport.Status = "Pending";
            damageReport.CreatedAt = DateTime.UtcNow;

            _context.DamageReports.Add(damageReport);
            await _context.SaveChangesAsync();
            return new DamageReportResponse
            {
                Id = damageReport.Id,
                Description = damageReport.Description,
                ImageUrl = damageReport.ImageUrl,
                Location = damageReport.Location,
                Latitude = damageReport.Latitude,
                Longitude = damageReport.Longitude,
                Status = damageReport.Status,
                AdminMessage = damageReport.AdminMessage,
                CreatedAt = damageReport.CreatedAt,
                UserId = damageReport.UserId,
                UserName = user.FullName, // pastikan `FullName` ada di model User
                BorrowRequestId = damageReport.BorrowRequestId,
                BorrowDate = borrowRequest.BorrowDate.ToUniversalTime(),
                ReturnDate = borrowRequest.ReturnDate.ToUniversalTime()
            };
        }

        public async Task<DamageReportResponse> UpdateDamageReportStatus(int id, string status, string adminMessage)
        {
            var damageReport = await _context.DamageReports
                .Include(dr => dr.BorrowRequest)
                .Include(dr => dr.User)
                .FirstOrDefaultAsync(dr => dr.Id == id);

            if (damageReport == null)
                throw new Exception("Damage report not found");

            damageReport.Status = status;
            damageReport.AdminMessage = adminMessage;

            var validStatuses = new[] { "Pending", "Resolved", "In Progress" };
            if (!validStatuses.Contains(status))
                throw new Exception("Invalid status value.");

            await _context.SaveChangesAsync();
            return new DamageReportResponse
            {
                
                Id = damageReport.Id,
                UserId = damageReport.UserId,
                Status = damageReport.Status,
                AdminMessage = damageReport.AdminMessage,
                

            };
        }

        public async Task<IEnumerable<DamageReportResponse>> GetDamageReportsByUser(int userId)
        {
            return await _context.DamageReports
                .Include(dr => dr.BorrowRequest)
                .Where(dr => dr.UserId == userId)
                .OrderByDescending(dr => dr.CreatedAt)
                .Select(dr => new DamageReportResponse
                {
                    Id = dr.Id,
                    Description = dr.Description,
                    ImageUrl = dr.ImageUrl,
                    Location = dr.Location,
                    Latitude = dr.Latitude,
                    Longitude = dr.Longitude,
                    Status = dr.Status,
                    AdminMessage = dr.AdminMessage,
                    CreatedAt = dr.CreatedAt,
                    UserId = dr.UserId,
                    UserName = dr.User.FullName, // pastikan `FullName` ada di model User
                    BorrowRequestId = dr.BorrowRequestId,
                    BorrowDate = dr.BorrowRequest.BorrowDate.ToUniversalTime(),
                    ReturnDate = dr.BorrowRequest.ReturnDate.ToUniversalTime()
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<DamageReportResponse>> GetAllDamageReports()
        {
            return await _context.DamageReports
                .Include(dr => dr.BorrowRequest)
                .Include(dr => dr.User)
                .OrderByDescending(dr => dr.CreatedAt)
                .Select(dr => new DamageReportResponse
                {
                    Id = dr.Id,
                    Description = dr.Description,
                    ImageUrl = dr.ImageUrl,
                    Location = dr.Location,
                    Latitude = dr.Latitude,
                    Longitude = dr.Longitude,
                    Status = dr.Status,
                    AdminMessage = dr.AdminMessage,
                    CreatedAt = dr.CreatedAt,
                    UserId = dr.UserId,
                    UserName = dr.User.FullName, // pastikan `FullName` ada di model User
                    BorrowRequestId = dr.BorrowRequestId,
                    BorrowDate = dr.BorrowRequest.BorrowDate.ToUniversalTime(),
                })
                .ToListAsync();
        }

        public async Task<DamageReport> GetDamageReportById(int id)
        {
            return await _context.DamageReports
                .Include(dr => dr.BorrowRequest)
                .Include(dr => dr.User)
                .FirstOrDefaultAsync(dr => dr.Id == id);
        }
    }
}
