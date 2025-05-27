using Classifly_API.Models;
using Classifly_API.DTOs.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Classifly_API.Services;
using System.Security.Claims;

namespace Classifly_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class PeminjamanController : ControllerBase
    {
        private readonly BorrowService _borrowService;
        private readonly NotificationService _notificationService;

        public PeminjamanController(BorrowService borrowService, NotificationService notificationService)
        {
            _borrowService = borrowService;
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBorrowRequest([FromBody] BorrowRequestCreateRequest request)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var createdRequest = await _borrowService.CreateBorrowRequest(request, userId);

                return Ok(new
                {
                    Message = "Borrow request created successfully",
                    RequestId = createdRequest.Id,
                    Items = createdRequest.BorrowItems.Select(bi => new {
                        bi.ItemId,
                        bi.Quantity
                    })
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "Failed to create borrow request",
                    Error = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateBorrowStatus(int id, [FromBody] BorrowStatusUpdateRequest request)
        {
            try
            {
                var updatedRequest = await _borrowService.UpdateBorrowStatus(id, request.Status, request.AdminMessage);

                return Ok(new
                {
                    Message = $"Borrow request {request.Status.ToLower()} successfully",
                    RequestId = updatedRequest.Id,
                    Status = updatedRequest.Status
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = "Failed to update borrow status",
                    Error = ex.Message
                });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserBorrowRequests()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
            {
                return BadRequest(new
                {
                    Message = "Invalid user ID in claims"
                });
            }

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
