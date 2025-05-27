using Classifly_API.Data;
using Classifly_API.DTOs.Responses;
using Classifly_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Classifly_API.Services
{
    public class ItemService
    {
        private readonly ClassiflyDbContext _context;

        public ItemService(ClassiflyDbContext context)
        {
            _context = context;
        }

        public async Task<Item> CreateItem(Item item)
        {
            var categoryExists = await _context.Categories
                .AnyAsync(c => c.Id == item.CategoryId);

            if (!categoryExists)
                throw new Exception("Category does not exist");

            item.CreatedAt = DateTime.UtcNow;
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<Item>> GetItemsByCategory(int categoryId)
        {
            return await _context.Items
                .Where(i => i.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ItemResponse>> GetAllItems()
        {
            return await _context.Items
             .Include(i => i.Category) // Include Category untuk eager loading
             .Select(i => new ItemResponse
             {
                 Id = i.Id,
                 Name = i.Name,
                 Description = i.Description,
                 Quantity = i.Quantity,
                 CategoryName = i.Category.Name, // Akses nama kategori
                 ImageUrl = i.ImageUrl
             })
             .ToListAsync();
        }

        public async Task<ItemResponse> GetItemById(int id)
        {
            return await _context.Items
              .Include(i => i.Category)
              .Where(i => i.Id == id) // Fixed: Replaced 'Contains' with '=='
              .Select(i => new ItemResponse
              {
                  Id = i.Id,
                  Name = i.Name,
                  Description = i.Description,
                  Quantity = i.Quantity,
                  CategoryName = i.Category.Name, // Access category name
                  ImageUrl = i.ImageUrl
              })
              .FirstOrDefaultAsync(); // Fixed: Changed ToListAsync to FirstOrDefaultAsync for single item retrieval
        }

        public async Task<Item> UpdateItem(Item item)
        {
            var existingItem = await _context.Items.FindAsync(item.Id);
            if (existingItem == null)
                throw new Exception("Item not found");

            existingItem.Name = item.Name;
            existingItem.Description = item.Description;
            existingItem.Quantity = item.Quantity;
            existingItem.Category = item.Category;
            existingItem.ImageUrl = item.ImageUrl;
            existingItem.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existingItem;
        }

        public async Task DeleteItem(int id)
        {
            var item = await _context.Items.FindAsync(id);
            if (item == null)
                throw new Exception("Item not found");

            _context.Items.Remove(item);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ItemResponse>> SearchItems(string searchTerm)
        {
            return await _context.Items
             .Include(i => i.Category) // Include Category untuk eager loading
             .Where(i => i.Name.Contains(searchTerm) || i.Description.Contains(searchTerm))
             .Select(i => new ItemResponse
             {
                 Id = i.Id,
                 Name = i.Name,
                 Description = i.Description,
                 Quantity = i.Quantity,
                 CategoryName = i.Category.Name, // Akses nama kategori
                 ImageUrl = i.ImageUrl
             })
             .ToListAsync();
        }
    }
}
