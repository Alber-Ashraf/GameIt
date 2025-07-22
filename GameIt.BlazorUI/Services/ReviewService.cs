using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Services;

public class ReviewService : BaseHttpService, IReviewService
{
    public ReviewService(IClient client) : base(client)
    {

    }
}
