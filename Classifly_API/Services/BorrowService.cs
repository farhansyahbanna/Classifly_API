using Classifly_API.Data;
using Classifly_API.Models;
using Microsoft.EntityFrameworkCore;
using Classifly_API.DTOs.Responses;

namespace Classifly_API.Services
{
    public class BorrowService
    {
        private readonly ClassiflyDbContext _context;

        public BorrowService(ClassiflyDbContext context)
        {
            _context = context;
        }

        public async Task<BorrowRequest> CreateBorrowRequest(BorrowRequest borrowRequest)
        {
            // Check item availability
            var item = await _context.Items.FindAsync(borrowRequest.ItemId);
            if (item == null)
                throw new Exception("Item not found");

            if (item.Quantity < borrowRequest.Quantity)
                throw new Exception("Not enough items available");

            _context.BorrowRequests.Add(borrowRequest);
            await _context.SaveChangesAsync();
            return borrowRequest;
        }

        public async Task<BorrowRequest> UpdateBorrowStatus(int id, string status, string adminMessage)
        {
            var borrowRequest = await _context.BorrowRequests
                .Include(br => br.Item)
                .Include(br => br.User)
                .FirstOrDefaultAsync(br => br.Id == id);

            if (borrowRequest == null)
                throw new Exception("Borrow request not found");

            borrowRequest.Status = status;
            borrowRequest.AdminMessage = adminMessage;

            // Update item quantity if approved
            if (status == "Approved")
            {
                var item = await _context.Items.FindAsync(borrowRequest.ItemId);
                item.Quantity -= borrowRequest.Quantity;
            }

            await _context.SaveChangesAsync();
            return borrowRequest;
        }

        public async Task<IEnumerable<BorrowRequest>> GetBorrowRequestsByUser(int userId)
        {
            return await _context.BorrowRequests
                .Include(br => br.Item)
                .Where(br => br.UserId == userId)
                .OrderByDescending(br => br.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRequest>> GetAllBorrowRequests()
        {
            return await _context.BorrowRequests
                .Include(br => br.Item)
                .Include(br => br.User)
                .OrderByDescending(br => br.CreatedAt)
                .ToListAsync();
        }

        public async Task<BorrowRequest> GetBorrowRequestById(int id)
        {
            return await _context.BorrowRequests
                .Include(br => br.Item)
                .Include(br => br.User)
                .FirstOrDefaultAsync(br => br.Id == id);
        }
    }
}
