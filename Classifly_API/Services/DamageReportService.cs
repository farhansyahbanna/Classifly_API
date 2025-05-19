using Classifly_API.Data;
using Classifly_API.Models;
using Microsoft.EntityFrameworkCore;
using Classifly_API.DTOs.Responses;

namespace Classifly_API.Services
{
    public class DamageReportService
    {
        private readonly ClassiflyDbContext _context;

        public DamageReportService(ClassiflyDbContext context)
        {
            _context = context;
        }

        public async Task<DamageReport> CreateDamageReport(DamageReport damageReport)
        {
            // Verify item exists
            var item = await _context.Items.FindAsync(damageReport.ItemId);
            if (item == null)
                throw new Exception("Item not found");

            _context.DamageReports.Add(damageReport);
            await _context.SaveChangesAsync();
            return damageReport;
        }

        public async Task<DamageReport> UpdateDamageReportStatus(int id, string status, string adminMessage)
        {
            var damageReport = await _context.DamageReports
                .Include(dr => dr.Item)
                .Include(dr => dr.User)
                .FirstOrDefaultAsync(dr => dr.Id == id);

            if (damageReport == null)
                throw new Exception("Damage report not found");

            damageReport.Status = status;
            damageReport.AdminMessage = adminMessage;
            await _context.SaveChangesAsync();
            return damageReport;
        }

        public async Task<IEnumerable<DamageReport>> GetDamageReportsByUser(int userId)
        {
            return await _context.DamageReports
                .Include(dr => dr.Item)
                .Where(dr => dr.UserId == userId)
                .OrderByDescending(dr => dr.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<DamageReport>> GetAllDamageReports()
        {
            return await _context.DamageReports
                .Include(dr => dr.Item)
                .Include(dr => dr.User)
                .OrderByDescending(dr => dr.CreatedAt)
                .ToListAsync();
        }

        public async Task<DamageReport> GetDamageReportById(int id)
        {
            return await _context.DamageReports
                .Include(dr => dr.Item)
                .Include(dr => dr.User)
                .FirstOrDefaultAsync(dr => dr.Id == id);
        }
    }
}
