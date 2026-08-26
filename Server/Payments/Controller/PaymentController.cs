using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Payments.Data;
using Payments.Dto;
using Payments.Entity;
using Payments.Enum;
using System.Security.Claims;

namespace Payments.Controller
{
    [ApiController]
    [Route("payment")]
    [Authorize]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentDbContext context;

        public PaymentController(PaymentDbContext _context)
        {
            context = _context;
        }

        [HttpPost("process")]
        public async Task<IActionResult> ProcessPayment(ProcessPaymentDto processPaymentDto)
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdGuid, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            //Ensure that subscription exist's
            //var IsSubscriptionExists = context.

            //var subscription = await context.Subscription
            //                        .FirstOrDefaultAsync(x =>
            //                            x.Id == processPaymentDto.SubscriptionId &&
            //                            x.UserId == userId);

            //if (subscription == null)
            //{
            //    return NotFound("Subscription not found.");
            //}

            var payment = new Payment
            {
                SubscriptionId = processPaymentDto.SubscriptionId,
                UserId = userId,
                Amount = processPaymentDto.SubscriptionAmount,
                Status = (PaymentStatus)Random.Shared.Next(0, 4), // PaymentStatus.Completed,
                TransactionReference = $"TXN-{Guid.NewGuid():N}"
            };

            context.Payment.Add(payment);
            await context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPayment), new { paymentid = payment.Id }, payment);
        }

        [HttpGet("{paymentid:guid}")]
        public async Task<IActionResult> GetPayment(Guid paymentid)
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdGuid, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            // Ensure the payment belongs to the authenticated user.
            var payment = await context.Payment.AsNoTracking().FirstOrDefaultAsync(x => x.Id == paymentid && x.UserId == userId);

            if (payment is null)
            {
                return NotFound("Payment not found.");
            }

            return Ok(payment);
        }

        [HttpGet("subscription/{subscriptionId:guid}")]
        public async Task<IActionResult> GetPaymentBySubscriptionId(Guid subscriptionId)
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdGuid, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var payments = await context.Payment
                .AsNoTracking()
                .Where(x => x.SubscriptionId == subscriptionId && x.UserId == userId)
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();

            return Ok(payments);
        }

        [HttpGet("transactions")]
        public async Task<IActionResult> GetUserPaymentHistory()
        {
            var userIdGuid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdGuid, out var userId))
            {
                return Unauthorized("Invalid user identifier.");
            }

            var payments = await context.Payment
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();

            return Ok(payments);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllPaymentTransactions()
        {
            var payments = await context.Payment
                .AsNoTracking()
                .OrderByDescending(x => x.PaymentDate)
                .ToListAsync();

            return Ok(payments);
        }

    }
}
