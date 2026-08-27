using Microsoft.AspNetCore.Mvc;

namespace Notifications.Controllers
{
    [ApiController]
    public class NotificationController: ControllerBase
    {
        [HttpPost("create")]
        public Task<IActionResult> async CreateNotification() {
            
            return CreatedAtAction(nameof(GetSubscription), new { subscriptionid = subscription.Id }, subscription);
        }
    }
}
