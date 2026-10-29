using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Discount.Commands.DeleteDiscount;

public class DeleteDiscountCommandHandler : IRequestHandler<DeleteDiscountCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    public DeleteDiscountCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Unit> Handle(DeleteDiscountCommand request, CancellationToken cancellationToken)
    {
        // Validate the request
        var validator = new DeleteDiscountCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Discount", validationResult);

        // Fetch existing Discount
        var existingDiscount = await _unitOfWork.Discounts.GetByIdAsync(request.Id);
        if (existingDiscount == null)
            throw new NotFoundException(nameof(Discount), request.Id);

        // Delete the Discount entity from the repository
        _unitOfWork.Discounts.Delete(existingDiscount);

        // Save changes to the database
        await _unitOfWork.SaveChangesAsync();

        // Return Unit.Value to indicate successful completion
        return Unit.Value;
    }
}