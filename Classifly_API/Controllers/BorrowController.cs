using Classifly_API.Models;
using Classifly_API.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Classifly_API.Services;

namespace Classifly_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BorrowController : ControllerBase
    {
        private readonly BorrowService _borrowService;
        private readonly NotificationService _notificationService;

        public BorrowController(BorrowService borrowService, NotificationService notificationService)
        {
            _borrowService = borrowService;
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBorrowRequest([FromBody] BorrowRequestCreateRequest request)
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            var borrowRequest = new BorrowRequest
            {
                UserId = userId,
                ItemId = request.ItemId,
                Quantity = request.Quantity,
                BorrowDate = request.BorrowDate,
                ReturnDate = request.ReturnDate,
                Location = request.Location
            };

            try
            {
                var createdRequest = await _borrowService.CreateBorrowRequest(borrowRequest);
                return Ok(createdRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBorrowStatus(int id, [FromBody] BorrowStatusUpdateRequest request)
        {
            try
            {
                var updatedRequest = await _borrowService.UpdateBorrowStatus(id, request.Status, request.AdminMessage);

                // Send notification to user
                await _notificationService.CreateNotification(new Notification
                {
                    UserId = updatedRequest.UserId,
                    Title = "Update Status Peminjaman",
                    Message = $"Peminjaman Anda untuk {updatedRequest.Item.Name} telah {request.Status}. Pesan admin: {request.AdminMessage}"
                });

                return Ok(updatedRequest);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserBorrowRequests()
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            var requests = await _borrowService.GetBorrowRequestsByUser(userId);
            return Ok(requests);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllBorrowRequests()
        {
            var requests = await _borrowService.GetAllBorrowRequests();
            return Ok(requests);
        }
    }
}
