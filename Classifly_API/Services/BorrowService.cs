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
                throw new Exception("Barang Tidak Ditemukan");

            // Buat objek BorrowRequest
            var borrowRequest = new BorrowRequest
            {
                BorrowDate = request.BorrowDate,
                ReturnDate = DateTime.UtcNow.AddDays(7), // Set default return date 7 days from now
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
                    throw new Exception($"Tidak Cukup Stock untuk ID Barang {item.ItemId}");

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
                throw new Exception("Peminjaman Barang Tidak Ditemukan");

            borrowRequest.Status = status;
            borrowRequest.AdminMessage = adminMessage;

            // Update item quantity jika approved/rejected
            if (status == "Approved" || status == "Rejected")
            {
                foreach (var borrowItem in borrowRequest.BorrowItems)
                {
                    var item = await _context.Items.FindAsync(borrowItem.ItemId);
                    if (item == null) 
                        throw new Exception($"Barang dengan ID {borrowItem.ItemId} Tidak Ditemukan");

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
                Status = br.Status,
                AdminMessage = br.AdminMessage,
                Location = br.Location,
                Latitude = br.Latitude,
                Longitude = br.Longitude,
                UserId = br.User.Id,
                UserName = br.User.Username, 
                BorrowItems = br.BorrowItems.Select(bi => new BorrowItemDTO
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
                    Status = br.Status,
                    AdminMessage = br.AdminMessage,
                    Location = br.Location,
                    Latitude = br.Latitude,
                    Longitude = br.Longitude,
                    UserId = br.User.Id,
                    UserName = br.User.Username, // Example
                    BorrowItems = br.BorrowItems.Select(bi => new BorrowItemDTO
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
