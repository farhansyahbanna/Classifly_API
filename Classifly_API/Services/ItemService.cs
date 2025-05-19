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
            item.CreatedAt = DateTime.UtcNow;
            _context.Items.Add(item);
            await _context.SaveChangesAsync();
            return item;
        }

        public async Task<IEnumerable<Item>> GetAllItems()
        {
            return await _context.Items.ToListAsync();
        }

        public async Task<Item> GetItemById(int id)
        {
            return await _context.Items.FindAsync(id);
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
                .Where(i => i.Name.Contains(searchTerm) || i.Description.Contains(searchTerm))
                .Select(i => new ItemResponse
                {
                    Id = i.Id,
                    Name = i.Name,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    Category = i.Category,
                    ImageUrl = i.ImageUrl
                })
                .ToListAsync();
        }
    }
}
