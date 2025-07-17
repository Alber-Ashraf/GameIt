using GameIt.Application.Interfaces.IDiscount;
using GameIt.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace GameIt.Infrastructure.DiscountService;

public class DiscountUpdate : IDiscountService
{
    private readonly GameItDbContext _context;
    public DiscountUpdate(GameItDbContext context)
    {
        _context = context;
    }
    public async Task UpdateDiscountStatusesAsync()
    {
        var discounts = await _context.Discounts.ToListAsync();

        foreach (var discount in discounts)
        {
            bool shouldBeActive = DateTime.UtcNow >= discount.StartDate && DateTime.UtcNow <= discount.EndDate;

            if (discount.IsActive != shouldBeActive)
            {
                discount.IsActive = shouldBeActive;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task RemoveExpiredDiscountsAsync()
    {
        var expiredDiscounts = await _context.Discounts
            .Where(d => d.EndDate < DateTime.UtcNow)
            .ToListAsync();

        if (expiredDiscounts.Any())
        {
            _context.Discounts.RemoveRange(expiredDiscounts);
            await _context.SaveChangesAsync();
        }
    }
}