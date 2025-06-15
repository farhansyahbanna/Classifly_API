using Classifly_API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Classifly_API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotifikasiController : ControllerBase
    {
        private readonly NotificationService _notificationService;

        public NotifikasiController(NotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetUserNotifications()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var notifications = await _notificationService.GetUserNotifications(userId);
            return Ok(notifications);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            await _notificationService.MarkNotificationAsRead(id, userId);
            return Ok(new { Message = "Notifikasi Telah Dibaca" });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
   
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }
            var userId = int.Parse(userIdString);

          
            var success = await _notificationService.DeleteNotificationAsync(id, userId);

            if (!success)
            {
                
                return NotFound(new { Message = "Notifikasi tidak ditemukan." });
            }   
            return Ok(new { Message = "Notifikasi berhasil dihapus." });
        }
    }
}
