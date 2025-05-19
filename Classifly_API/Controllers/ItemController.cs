using Classifly_API.DTOs.Requests;
using Classifly_API.Models;
using Classifly_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Classifly_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ItemController : ControllerBase
    {
        private readonly ItemService _itemService;

        public ItemController(ItemService itemService)
        {
            _itemService = itemService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromForm] ItemCreateRequest request)
        {
            try
            {
                // Handle file upload
                string imageUrl = null;
                if (request.ImageFile != null && request.ImageFile.Length > 0)
                {
                    // Save file and get URL (implementation depends on your storage solution)
                    imageUrl = await SaveFile(request.ImageFile);
                }

                var item = new Item
                {
                    Name = request.Name,
                    Description = request.Description,
                    Quantity = request.Quantity,
                    Category = request.Category,
                    ImageUrl = imageUrl
                };

                var createdItem = await _itemService.CreateItem(item);
                return Ok(createdItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllItems()
        {
            var items = await _itemService.GetAllItems();
            return Ok(items);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetItemById(int id)
        {
            var item = await _itemService.GetItemById(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateItem(int id, [FromForm] ItemUpdateRequest request)
        {
            try
            {
                string imageUrl = null;
                if (request.ImageFile != null && request.ImageFile.Length > 0)
                {
                    imageUrl = await SaveFile(request.ImageFile);
                }

                var item = new Item
                {
                    Id = id,
                    Name = request.Name,
                    Description = request.Description,
                    Quantity = request.Quantity,
                    Category = request.Category,
                    ImageUrl = imageUrl ?? request.ExistingImageUrl
                };

                var updatedItem = await _itemService.UpdateItem(item);
                return Ok(updatedItem);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItem(int id)
        {
            try
            {
                await _itemService.DeleteItem(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            // Implement file saving logic (e.g., save to wwwroot or cloud storage)
            // Return the URL/path to the saved file
            return $"uploads/{Guid.NewGuid()}_{file.FileName}";
        }
    }
}
