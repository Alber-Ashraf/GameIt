using GameIt.BlazorUI.Contracts;
using GameIt.BlazorUI.Services.Base;

namespace GameIt.BlazorUI.Services;

public class DiscountService : BaseHttpService, IDiscountService
{
    public DiscountService(IClient client) : base(client)
    {

    }
}
