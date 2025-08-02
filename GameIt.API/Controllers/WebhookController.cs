using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using GameIt.Application.Interfaces.Stripe;
using GameIt.Domain;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace GameIt.API.Controllers
{
    [ApiController]
    [Route("api/webhook")]
    public class WebhookController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IStripeService _stripeService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            ILogger<WebhookController> logger,
            IStripeService stripeService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _logger = logger;
            _stripeService = stripeService;
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
        }

        [HttpPost]
        public async Task<IActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            try
            {
                var stripeSignature = Request.Headers["Stripe-Signature"];

                if (string.IsNullOrEmpty(stripeSignature))
                {
                    _logger.LogWarning("Stripe-Signature header is missing");
                    return BadRequest("Stripe-Signature header is missing");
                }

                var stripeEvent = await _stripeService.ConstructWebhookEventAsync(json, stripeSignature);

                _logger.LogInformation($"Received Stripe event: {stripeEvent.Type}");

                switch (stripeEvent.Type)
                {
                    case "checkout.session.completed":
                        var session = stripeEvent.Data.Object as Session;
                        await HandleCheckoutSessionCompleted(session);
                        break;

                    case "checkout.session.async_payment_succeeded":
                        var successSession = stripeEvent.Data.Object as Session;
                        await HandlePaymentSuccess(successSession);
                        break;

                    case "checkout.session.async_payment_failed":
                        var failedSession = stripeEvent.Data.Object as Session;
                        await HandlePaymentFailed(failedSession);
                        break;

                    default:
                        _logger.LogInformation($"Unhandled event type: {stripeEvent.Type}");
                        break;
                }

                return Ok();
            }
            catch (ApplicationException ex)
            {
                _logger.LogError(ex, "Stripe webhook validation failed");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing webhook");
                return StatusCode(500);
            }
        }


        private async Task HandleCheckoutSessionCompleted(Session session)
        {
            // Only process if payment is actually complete
            if (session.PaymentStatus != "paid")
            {
                _logger.LogInformation($"Session {session.Id} completed but not paid yet (status: {session.PaymentStatus})");
                return;
            }

            await ProcessSuccessfulPayment(session);
        }

        private async Task HandlePaymentSuccess(Session session)
        {
            _logger.LogInformation($"Async payment succeeded for session {session.Id}");
            await ProcessSuccessfulPayment(session);
        }

        private async Task ProcessSuccessfulPayment(Session session)
        {
            if (!Guid.TryParse(session.Metadata["purchase_id"], out var purchaseId))
            {
                _logger.LogError("Invalid purchase_id in session metadata");
                throw new BadRequestException("Invalid purchase_id");
            }

            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var purchase = await _unitOfWork.Purchases.GetByIdAsync(purchaseId);
                    if (purchase == null)
                    {
                        _logger.LogError($"Purchase not found: {purchaseId}");
                        throw new NotFoundException(nameof(Purchase), purchaseId);
                    }

                    // Idempotency check
                    if (purchase.Status == PaymentStatus.Succeeded)
                    {
                        _logger.LogInformation($"Purchase {purchaseId} already processed");
                        return;
                    }

                    // Update purchase
                    purchase.Status = PaymentStatus.Succeeded;
                    purchase.PurchaseDate = DateTime.UtcNow;
                    purchase.StripePaymentIntentId = session.PaymentIntentId;
                    _unitOfWork.Purchases.Update(purchase);

                    // Add to library
                    var libraryEntry = new Library
                    {
                        UserId = purchase.UserId,
                        GameId = purchase.GameId,
                        PurchasedAt = DateTime.UtcNow
                    };

                    await _unitOfWork.Libraries.CreateAsync(libraryEntry);
                    await _unitOfWork.SaveChangesAsync();
                    await transaction.CommitAsync();

                    _logger.LogInformation($"Successfully processed purchase {purchaseId}");
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        private async Task HandlePaymentFailed(Session session)
        {
            if (!Guid.TryParse(session.Metadata["purchase_id"], out var purchaseId))
            {
                _logger.LogError("Invalid purchase_id in failed session metadata");
                throw new BadRequestException("Invalid purchase_id");
            }

            var purchase = await _unitOfWork.Purchases.GetByIdAsync(purchaseId);
            if (purchase == null)
            {
                _logger.LogError($"Purchase not found: {purchaseId}");
                throw new NotFoundException(nameof(Purchase), purchaseId);
            }

            purchase.Status = PaymentStatus.Failed;
            _unitOfWork.Purchases.Update(purchase);
            await _unitOfWork.SaveChangesAsync();

            _logger.LogInformation($"Marked purchase {purchaseId} as failed");
        }
    }
}