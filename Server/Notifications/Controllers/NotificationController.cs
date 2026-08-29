using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Notifications.Data;
using Notifications.Dto;
using Notifications.Entity;
using System.Security.Claims;

namespace Notifications.Controllers
{
    [ApiController]
    [Route("notification")]
    public class NotificationController: ControllerBase
    {
        private readonly NotificationDbContext context;

        public NotificationController(NotificationDbContext _context) { 
                context = _context;
        }
       
        [HttpPost("create")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> CreateNotification(CreateNotificationDto createNotificationDto) {
            
            Notification notification = new Notification { 
                
                UserId = createNotificationDto.UserId,
                Title = createNotificationDto.Title,
                Message = createNotificationDto.Message,
                Type = createNotificationDto.Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            context.Notification.Add(notification);
            await context.SaveChangesAsync();

            return Ok(notification);
            
        }

        [HttpGet("my")]        
        public async Task<IActionResult> GetUserNotifications() {

            var UserIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(UserIdGuid, out var userId)) {
                return Unauthorized("Invalid user identifier.");
            }

            var notifications = await context.Notification
                                .AsNoTracking()
                                .Where(x => x.UserId == userId)
                                .OrderByDescending(x => x.CreatedAt)
                                .ToListAsync();

            return Ok(notifications);
        }

        [HttpPatch("readall")]        
        public async Task<IActionResult> ReadAllNotifications() {

            var UserIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(UserIdGuid, out var userId)) {
                return Unauthorized("Invalid user identifier.");
            }

            var notifications = await context.Notification
                                    .Where(x => x.UserId == userId)
                                    .ToListAsync();

            if (!notifications.Any())
            {
                return NotFound("All Notifications are already seen");
            }
          
            foreach (var item in notifications)
            {
                item.IsRead = true;
            }            
            
            await context.SaveChangesAsync();
            return Ok("Marked Seen");
        }

        [HttpPatch("read/{notificationId:guid}")]        
        public async Task<IActionResult> GetNotifications(Guid notificationId) {

            var UserIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(UserIdGuid, out var userId)) {
                return Unauthorized("Invalid user identifier.");
            }

            var notification = await context.Notification
                                .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId);

            if (notification is null) {
                return NotFound("No notifications found.");
            }

            if (notification.IsRead) {
                return Ok("Notification already seen");
            }

            notification.IsRead = true;
            await context.SaveChangesAsync();

            return Ok("Marked Seen");
        }

        [HttpGet("unreadcount")]
        public async Task<IActionResult> GetCountOfUnreadNotifications()
        {

            var UserIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(UserIdGuid, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var notificationsCount = await context.Notification
                                    .CountAsync(x => !x.IsRead && x.UserId == userId);
                        
            return Ok(notificationsCount);
        }

        [HttpDelete("delete/{notificationId:guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteNotifications(Guid notificationId) {

            var notification = await context.Notification
                                .FirstOrDefaultAsync(x => x.Id == notificationId);

            if (notification is null) {
                return NotFound("Notification Not Exists");
            }

            context.Notification.Remove(notification);
            await context.SaveChangesAsync();

            return Ok("Notification Deleted");
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllNotifications()
        {            
            var notifications = await context.Notification
                                    .AsNoTracking()
                                    .ToListAsync();

            return Ok(notifications);
        }
        
    }
}
