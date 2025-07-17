namespace GameIt.Application.Interfaces.IDiscount;

public interface IDiscountService
{
    Task UpdateDiscountStatusesAsync();
    Task RemoveExpiredDiscountsAsync();
}
