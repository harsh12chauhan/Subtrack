using Notifications.Data;
using Notifications.Dto;
using Notifications.Entity;
using Notifications.Interfaces;

namespace Notifications.Services
{
    public class NotificationService(NotificationDbContext context) : INotificationService
    {
        public async Task<NotificationResponseDto> CreateNotification(CreateNotificationDto createNotificationDto)
        {
            Notification notification = new Notification
            {

                UserId = createNotificationDto.UserId,
                Title = createNotificationDto.Title,
                Message = createNotificationDto.Message,
                Type = createNotificationDto.Type,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            context.Notification.Add(notification);
            await context.SaveChangesAsync();

            NotificationResponseDto notificationResponseDto = new NotificationResponseDto { 
                Id = notification.Id,
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                Type = notification.Type,
                IsRead = notification.IsRead,
                CreatedAt= notification.CreatedAt
            };

            return notificationResponseDto;
        }

        public Task<int> GetCountOfUnreadNotifications()
        {
            throw new NotImplementedException();
        }

        public Task<List<NotificationResponseDto>> GetUserNotifications()
        {
            throw new NotImplementedException();
        }

        public Task<string> ReadAllNotifications()
        {
            throw new NotImplementedException();
        }

        public Task<string> ReadNotification(Guid notificationId)
        {
            throw new NotImplementedException();
        }

        public Task<string> DeleteNotifications(Guid notificationId)
        {
            throw new NotImplementedException();
        }

        public Task<NotificationResponseDto> GetAllNotifications()
        {
            throw new NotImplementedException();
        }
    }
}
