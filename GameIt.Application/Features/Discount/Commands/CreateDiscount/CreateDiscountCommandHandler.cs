using AutoMapper;
using GameIt.Application.Exeptions;
using GameIt.Application.Interfaces.Persistence;
using MediatR;

namespace GameIt.Application.Features.Discount.Commands.CreateDiscount;

public class CreateDiscountCommandHandler : IRequestHandler<CreateDiscountCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    public CreateDiscountCommandHandler(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }
    public async Task<Guid> Handle(
        CreateDiscountCommand request,
        CancellationToken token)
    {
        // Validate the request 
        var validator = new CreateDiscountCommandValidator(_unitOfWork);
        var validationResult = await validator.ValidateAsync(request, token);

        if (!validationResult.IsValid)
            throw new BadRequestException("Invalid Discount", validationResult);

        // Map the request to a Discount entity
        var DiscountToCreate = _mapper.Map<Domain.Discount>(request);

        // Create the Discount in the repository
        await _unitOfWork.Discounts.CreateAsync(DiscountToCreate);

        // Save changes
        await _unitOfWork.SaveChangesAsync(token);

        // Return the new Discount's ID
        return DiscountToCreate.Id;
    }
}
    