using Classifly_API.Data;
using Classifly_API.Models;
using Microsoft.EntityFrameworkCore;
using Classifly_API.DTOs.Responses;
using Classifly_API.DTOs.Requests;

namespace Classifly_API.Services
{
    public class BorrowService
    {
        private readonly ClassiflyDbContext _context;

        public BorrowService(ClassiflyDbContext context)
        {
            _context = context;
        }

        public async Task<BorrowRequest> CreateBorrowRequest(BorrowRequestCreateRequest request, int userId)
        {
            // Validasi semua item
            var itemIds = request.Items.Select(i => i.ItemId).ToList();
            var itemsFromDb = await _context.Items
                .Where(i => itemIds.Contains(i.Id))
                .ToListAsync();

            if (itemsFromDb.Count != itemIds.Count)
                throw new Exception("One or more items not found.");

            // Buat objek BorrowRequest
            var borrowRequest = new BorrowRequest
            {
                BorrowDate = request.BorrowDate,
                ReturnDate = request.ReturnDate,
                Location = request.Location,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                UserId = userId,
                Status = "Pending",
                CreatedAt = DateTime.UtcNow
            };

            // Tambahkan BorrowItem ke BorrowRequest
            foreach (var item in request.Items)
            {
                var itemFromDb = itemsFromDb.First(i => i.Id == item.ItemId);

                // Validasi stok
                if (item.Quantity > itemFromDb.Quantity)
                    throw new Exception($"Not enough stock for item ID {item.ItemId}");

                // Kurangi stok
                //itemFromDb.Quantity -= item.Quantity;

                borrowRequest.BorrowItems.Add(new BorrowItem
                {
                    ItemId = item.ItemId,
                    Quantity = item.Quantity
                });
            }

            _context.BorrowRequests.Add(borrowRequest);
            await _context.SaveChangesAsync();

            return borrowRequest;
        }

        public async Task<BorrowRequest> UpdateBorrowStatus(int id, string status, string adminMessage)
        {
            var borrowRequest = await _context.BorrowRequests
                .Include(br => br.BorrowItems)
                    .ThenInclude(bi => bi.Item)
                .Include(br => br.User)
                .FirstOrDefaultAsync(br => br.Id == id);

            if (borrowRequest == null)
                throw new Exception("Borrow request not found");

            borrowRequest.Status = status;
            borrowRequest.AdminMessage = adminMessage;

            // Update item quantity jika approved/rejected
            if (status == "Approved" || status == "Rejected")
            {
                foreach (var borrowItem in borrowRequest.BorrowItems)
                {
                    var item = await _context.Items.FindAsync(borrowItem.ItemId);
                    if (item == null) // Add null check to avoid dereferencing a null reference
                        throw new Exception($"Item with ID {borrowItem.ItemId} not found");

                    if (status == "Approved")
                    {
                        item.Quantity -= borrowItem.Quantity;
                    }
                    else if (status == "Rejected")
                    {
                        // Kembalikan stok jika ditolak
                        item.Quantity += borrowItem.Quantity;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return borrowRequest;
        }

        public async Task<IEnumerable<BorrowRequestDto>> GetBorrowRequestsByUser(int userId)
        {
            return await _context.BorrowRequests
            .Include(br => br.BorrowItems)
                .ThenInclude(bi => bi.Item)
            .Include(br => br.User)
            .Where(br => br.UserId == userId)
            .OrderByDescending(br => br.CreatedAt)
            .Select(br => new BorrowRequestDto
            {
                Id = br.Id,
                BorrowDate = br.BorrowDate,
                ReturnDate = br.ReturnDate,
                Status = br.Status,
                Location = br.Location,
                Latitude = br.Latitude,
                Longitude = br.Longitude,
                UserId = br.User.Id,
                UserName = br.User.Username, // Example
                BorrowItems = br.BorrowItems.Select(bi => new BorrowItemDto
                {
                    ItemId = bi.Item.Id,
                    ItemName = bi.Item.Name,
                    Quantity = bi.Quantity
                }).ToList()
            })
            .ToListAsync();
        }

        public async Task<IEnumerable<BorrowRequestDto>> GetAllBorrowRequests()
        {
            return await _context.BorrowRequests
                .Include(br => br.BorrowItems)
                    .ThenInclude(bi => bi.Item)
                .Include(br => br.User)
                .OrderByDescending(br => br.CreatedAt)
                .Select(br => new BorrowRequestDto
                {
                    Id = br.Id,
                    BorrowDate = br.BorrowDate,
                    ReturnDate = br.ReturnDate,
                    Status = br.Status,
                    Location = br.Location,
                    Latitude = br.Latitude,
                    Longitude = br.Longitude,
                    UserId = br.User.Id,
                    UserName = br.User.Username, // Example
                    BorrowItems = br.BorrowItems.Select(bi => new BorrowItemDto
                    {
                        ItemId = bi.Item.Id,
                        ItemName = bi.Item.Name,
                        Quantity = bi.Quantity
                    }).ToList()
                })
                .ToListAsync();
        }

        public async Task<BorrowRequest> GetBorrowRequestById(int id)
        {
            return await _context.BorrowRequests
                .Include(br => br.BorrowItems)
                    .ThenInclude(bi => bi.Item)
                .Include(br => br.User)
                .FirstOrDefaultAsync(br => br.Id == id);
        }
    }
}
