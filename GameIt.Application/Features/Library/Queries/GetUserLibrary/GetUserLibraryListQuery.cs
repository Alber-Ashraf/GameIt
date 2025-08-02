using GameIt.Application.Features.Purchase.Queries.GetUserPurchase;
using MediatR;

namespace GameIt.Application.Features.Library.Queries.GetUserLibrary;

public record GetUserLibraryListQuery() : IRequest<List<LibraryListDto>> {}