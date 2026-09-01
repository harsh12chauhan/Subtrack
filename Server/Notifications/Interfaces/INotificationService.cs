using Microsoft.AspNetCore.Mvc;
using Notifications.Dto;

namespace Notifications.Interfaces
{
    public interface INotificationService
    {
        Task<NotificationResponseDto> CreateNotification(CreateNotificationDto createNotificationDto);

        Task<List<NotificationResponseDto>> GetUserNotifications();

        Task<string> ReadAllNotifications();

        Task<string> ReadNotification(Guid notificationId);

        Task<int> GetCountOfUnreadNotifications();

        Task<string> DeleteNotifications(Guid notificationId);

        Task<NotificationResponseDto> GetAllNotifications();
    }
}
