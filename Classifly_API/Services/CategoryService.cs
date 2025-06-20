﻿using Classifly_API.Data;
using Classifly_API.DTOs.Requests;
using Classifly_API.DTOs.Responses;
using Classifly_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Classifly_API.Services
{
    public class CategoryService
    {
        private readonly ClassiflyDbContext _context;

        public CategoryService(ClassiflyDbContext context)
        {
            _context = context;
        }

        public async Task<CategoryResponse> CreateCategory(CategoryCreateRequest request)
        {
            var category = new Category
            {
                Name = request.Name,
                
                CreatedAt = DateTime.UtcNow
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return MapToDto(category);
        }



        public async Task<IEnumerable<CategoryResponse>> GetAllCategories()
        {
            return await _context.Categories
                .Where(c => !c.IsDeleted)
                .Select(c => MapToDto(c))
                .ToListAsync();
        }
        public async Task<CategoryResponse> GetCategoryById(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            return category != null ? MapToDto(category) : null;
        }

        public async Task DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null || category.IsDeleted)
                throw new Exception("Kategori Tidak Ditemukan");

            if (category.Items.Any())
                throw new Exception("Cannot delete category with existing items");

            category.IsDeleted = true;
            await _context.SaveChangesAsync();
        }

        public async Task<Category?> UpdateCategory(int id, CategoryUpdateRequest request)
        {
 
            var existingCategory = await _context.Categories.FindAsync(id);

            if (existingCategory == null)
            {
                return null; 
            }


            var isDuplicate = await _context.Categories
                .AnyAsync(c => c.Name == request.CategoryName && c.Id != id);

            if (isDuplicate)
            {

                throw new InvalidOperationException("Nama kategori tersebut sudah digunakan.");
            }


            existingCategory.Name = request.CategoryName;
  
            await _context.SaveChangesAsync();

            return existingCategory;
        }



        private static CategoryResponse MapToDto(Category category)
        {
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                
                CreatedAt = category.CreatedAt,

            };
        }
    }
}

