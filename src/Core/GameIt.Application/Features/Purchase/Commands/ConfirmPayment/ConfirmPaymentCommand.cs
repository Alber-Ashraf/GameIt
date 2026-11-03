using GameIt.Application.Models.Stripe;
using MediatR;

namespace GameIt.Application.Features.Purchase.Commands.ConfirmPayment;

public class ConfirmPaymentCommand : IRequest<ConfirmPaymentResult>
{
    public string SessionId { get; set; } = string.Empty;

    public ConfirmPaymentCommand()
    {
    }

    public ConfirmPaymentCommand(string sessionId)
    {
        SessionId = sessionId;
    }
}
