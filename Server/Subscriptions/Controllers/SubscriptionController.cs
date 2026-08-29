using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Subscriptions.Data;
using Subscriptions.Dto;
using Subscriptions.Entities;
using Subscriptions.Enum;
using System.Security.Claims;

namespace Subscriptions.Controllers
{
    [ApiController]
    [Route("subscription")]
    [Authorize]
    public class SubscriptionController : ControllerBase
    {        
        private readonly SubscriptionDbContext context;
        public SubscriptionController(SubscriptionDbContext _context)
        {
            context = _context;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateSubscription(CreateSubscriptionDto createSubscriptionDto)
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdGuid, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var IsSubscriptionExists = await context.Subscription
                                            .AnyAsync(x =>
                                                x.UserId == userId &&
                                                x.Name == createSubscriptionDto.Name &&
                                                x.Category == createSubscriptionDto.Category);

            if (IsSubscriptionExists)
            {
                return NotFound("Subscription already exist's.");
            }

            var subscription = new Subscription
            {
                UserId = userId,
                Name = createSubscriptionDto.Name,
                Amount = createSubscriptionDto.Amount,
                Category = createSubscriptionDto.Category,
                BillingCycle = createSubscriptionDto.BillingCycle,
                NextBillingDate = createSubscriptionDto.NextBillingDate
            };

            context.Subscription.Add(subscription);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetSubscription), new { subscriptionid = subscription.Id }, subscription);
        }
            
        [HttpPatch("update/{subscriptionid:guid}")]
        public async Task<IActionResult> UpdateSubscription(Guid subscriptionid ,UpdateSubscriptionDto updateSubscriptionDto)
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdGuid, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            // Ensure the subscription belongs to the authenticated user.
            var subscription = await context.Subscription.FirstOrDefaultAsync(x => x.Id == subscriptionid && x.UserId == userId);            

            if (subscription is null)
            {
                return NotFound("Subscription not found!");
            }
          
            subscription.Name = !string.IsNullOrWhiteSpace(updateSubscriptionDto.Name) ? updateSubscriptionDto.Name: subscription.Name;
            subscription.Amount = updateSubscriptionDto.Amount ?? subscription.Amount;
            subscription.Category = updateSubscriptionDto.Category ?? subscription.Category;
            subscription.BillingCycle = updateSubscriptionDto.BillingCycle ?? subscription.BillingCycle;
            subscription.NextBillingDate = updateSubscriptionDto.NextBillingDate ?? subscription.NextBillingDate;

            context.Subscription.Update(subscription);
            await context.SaveChangesAsync();

            return Ok(subscription);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var allSubscriptions = await context.Subscription.AsNoTracking().ToListAsync();

            return Ok(allSubscriptions);
        }

        [HttpGet("{subscriptionid:guid}")]
        public async Task<IActionResult> GetSubscription(Guid subscriptionid)
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdGuid, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            // Ensure the subscription belongs to the authenticated user.
            var subscription = await context.Subscription
                                .AsNoTracking() // used to increase speed
                                .FirstOrDefaultAsync(x => x.Id == subscriptionid && x.UserId == userId);            

            if (subscription is null)
            {
                return NotFound("Subscription not found!");
            }
            return Ok(subscription);
        }

        [HttpGet("user-subscription")]
        public async Task<IActionResult> GetUserSubscriptions()
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (!Guid.TryParse(userIdGuid,out var userId)) {

                return Unauthorized("Invalid user identifier.");
            }

            var subscriptions = await context.Subscription
                                    .AsNoTracking() // used to increase speed
                                    .Where(x => x.UserId == userId && x.Status != BillingStatus.Cancelled)
                                    .OrderBy(x => x.NextBillingDate)
                                    .ToListAsync();

            return Ok(subscriptions);
        }

        [HttpPut("status/{subscriptionid:guid}/{status}")]
        public async Task<IActionResult> UpdateSubscriptionStatus(Guid subscriptionid, BillingStatus status)
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdGuid, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            // Ensure the subscription belongs to the authenticated user.
            var subscription = await context.Subscription.FirstOrDefaultAsync(x => x.Id == subscriptionid && x.UserId == userId);

            if (subscription is null)
            {
                return NotFound("Subscription not found!");
            }

            if (subscription.Status == status) 
            {
                return Ok($"Status is already {status}");
            }

            // validating enum values
            if (!System.Enum.IsDefined(typeof(BillingStatus), status))
            {
                return BadRequest("Invalid status.");
            }


            subscription.Status = status;
            
            await context.SaveChangesAsync();

            return Ok($"Subscription status updated to {status}.");
        }

        [HttpDelete("{subscriptionid:guid}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> DeleteSubscription(Guid subscriptionid)
        {
            var subscription = await context.Subscription.FindAsync(subscriptionid);

            if (subscription is null)
            {
                return NotFound("Subscription not found!");
            }

            context.Subscription.Remove(subscription);
            await context.SaveChangesAsync();

            return Ok("Subscription deleted.");
        }
        
        [HttpPatch("renew/{subscriptionid:guid}")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> RenewSubscription(Guid subscriptionid)
        {
            var subscription = await context.Subscription.FindAsync(subscriptionid);

            if (subscription is null)
            {
                return NotFound("Subscription not found!");
            }

            if (subscription.Status == BillingStatus.Cancelled)
            {
                return BadRequest("Cancelled subscriptions cannot be renewed.");
            }

            if (subscription.Status != BillingStatus.Active)
            {
                return BadRequest("Only active subscriptions can be renewed.");
            }

            switch (subscription.BillingCycle)
            {
                case BillingCycle.Monthly:
                    subscription.NextBillingDate = subscription.NextBillingDate.AddMonths(1);
                    break;

                case BillingCycle.Yearly:
                    subscription.NextBillingDate = subscription.NextBillingDate.AddYears(1);
                    break;

                default:
                    return BadRequest("Invalid billing cycle.");
            }

            await context.SaveChangesAsync();

            return Ok(new
            {
                message = "Subscription renewed.",
                nextBillingDate = subscription.NextBillingDate
            });
        }
        
        [HttpGet("due")]
        [Authorize(Roles = "Worker")]
        public async Task<IActionResult> GetUserDueSubscriptions()
        {            
            var today = DateTime.UtcNow.Date;

            var subscriptions = await context.Subscription
                                .AsNoTracking()
                                .Where(x =>
                                    x.Status == BillingStatus.Active &&
                                    x.NextBillingDate.Date == today)
                                .OrderBy(x => x.NextBillingDate)
                                .ToListAsync();

            return Ok(subscriptions);
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories() {

            var categories = await context.Subscription
                                .AsNoTracking()
                                .Select(x => x.Category)
                                .Distinct()
                                .OrderByDescending(x => x)
                                .ToListAsync();

            return Ok(categories);
        }

        private string? GetCurrentUserId()
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdGuid, out var userId))
            {
                return null;
            }

            return userId.ToString();
        }

    }
}
    