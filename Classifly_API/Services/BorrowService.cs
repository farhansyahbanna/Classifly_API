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
                BorrowDate = request.BorrowDate.ToUniversalTime(),
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
            // Gunakan transaksi untuk memastikan semua operasi berhasil atau semua gagal
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
          
                    var borrowRequest = await _context.BorrowRequests
                        .Include(br => br.BorrowItems)
                            .ThenInclude(bi => bi.Item) 
                        .FirstOrDefaultAsync(br => br.Id == id);

                    if (borrowRequest == null)
                    {
                        throw new Exception("Peminjaman Barang Tidak Ditemukan");
                    }

            
                    var originalStatus = borrowRequest.Status;

         
                    if (originalStatus == status)
                    {
                        borrowRequest.AdminMessage = adminMessage; 
                        await _context.SaveChangesAsync();
                        await transaction.CommitAsync();
                        return borrowRequest;
                    }

                    // 3. Terapkan logika berdasarkan PERUBAHAN STATUS

                    // KASUS 1: Peminjaman disetujui (dari Pending/Rejected menjadi Approved)
                    if (status == "Approved")
                    {
                        foreach (var borrowItem in borrowRequest.BorrowItems)
                        {
                          
                            var item = borrowItem.Item;
                            if (item == null)
                                throw new Exception($"Data barang untuk ItemId {borrowItem.ItemId} tidak ditemukan.");


                            if (item.Quantity < borrowItem.Quantity)
                            {
                                throw new Exception($"Stok untuk '{item.Name}' tidak mencukupi.");
                            }

                
                            item.Quantity -= borrowItem.Quantity;
                        }
                    }
   
                    else if (originalStatus == "Approved" && (status == "Rejected" || status == "Cancelled"))
                    {
                        foreach (var borrowItem in borrowRequest.BorrowItems)
                        {
                            var item = borrowItem.Item;
                            if (item == null)
                                throw new Exception($"Data barang untuk ItemId {borrowItem.ItemId} tidak ditemukan.");

 
                            item.Quantity += borrowItem.Quantity;
                        }
                    }
                    // KASUS 3: Dari Pending ke Rejected, atau perubahan lain yang tidak memengaruhi stok,
                    // tidak perlu melakukan apa-apa terhadap kuantitas.


                    borrowRequest.Status = status;
                    borrowRequest.AdminMessage = adminMessage;


                    await _context.SaveChangesAsync();


                    await transaction.CommitAsync();

                    return borrowRequest;
                }
                catch (Exception ex)
                {

                    await transaction.RollbackAsync();

  
                    throw new Exception($"Gagal memperbarui status: {ex.Message}");
                }
            }
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
                BorrowDate = br.BorrowDate.ToUniversalTime(),
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
                    BorrowDate = br.BorrowDate.ToUniversalTime(),
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
