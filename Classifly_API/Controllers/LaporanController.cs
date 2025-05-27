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
    public class LaporanController : ControllerBase
    {
        private readonly DamageReportService _damageReportService;
        private readonly NotificationService _notificationService;

        public LaporanController(DamageReportService damageReportService, NotificationService notificationService)
        {
            _damageReportService = damageReportService;
            _notificationService = notificationService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateDamageReport([FromForm] DamageReportCreateRequest request)
        {
            var userId = int.Parse(User.FindFirst("id").Value);

            // Handle file upload
            string imageUrl = null;
            if (request.ImageFile != null && request.ImageFile.Length > 0)
            {
                imageUrl = await SaveFile(request.ImageFile);
            }

            var damageReport = new DamageReport
            {
                UserId = userId,
                ItemId = request.ItemId,
                Description = request.Description,
                ImageUrl = imageUrl,
                Location = request.Location
            };

            try
            {
                var createdReport = await _damageReportService.CreateDamageReport(damageReport);
                return Ok(createdReport);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateDamageReportStatus(int id, [FromBody] DamageReportStatusUpdateRequest request)
        {
            try
            {
                var updatedReport = await _damageReportService.UpdateDamageReportStatus(id, request.Status, request.AdminMessage);

                // Send notification to user
                await _notificationService.CreateNotification(new Notification
                {
                    UserId = updatedReport.UserId,
                    Title = "Update Status Laporan Kerusakan",
                    Message = $"Laporan kerusakan Anda untuk {updatedReport.Item.Name} telah {request.Status}. Pesan admin: {request.AdminMessage}"
                });

                return Ok(updatedReport);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUserDamageReports()
        {
            var userId = int.Parse(User.FindFirst("id").Value);
            var reports = await _damageReportService.GetDamageReportsByUser(userId);
            return Ok(reports);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllDamageReports()
        {
            var reports = await _damageReportService.GetAllDamageReports();
            return Ok(reports);
        }

        private async Task<string> SaveFile(IFormFile file)
        {
            // Implement file saving logic
            return $"damage-reports/{Guid.NewGuid()}_{file.FileName}";
        }
    }
}
