using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Discount.Commands.UpdateDiscount;

public class UpdateDiscountCommandhandler : IRequestHandler<UpdateDiscountCommand, Unit>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public UpdateDiscountCommandhandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Unit> Handle(UpdateDiscountCommand request, CancellationToken token)
    {
        // Validate
        var validator = new UpdateDiscountCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);
        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Discount", validationResult);
        // Fetch and verify
        var existingDiscount = await _unitOfWork.Discounts.GetByIdAsync(request.Id);
        if (existingDiscount == null)
            throw new NotFoundException(nameof(Discount), request.Id);

        _mapper.Map(request, existingDiscount);
        // Update the Discount in the repository
        _unitOfWork.Discounts.Update(existingDiscount);
        // Save
        await _unitOfWork.SaveChangesAsync(token);
        // Return Unit
        return Unit.Value;
    }
}
