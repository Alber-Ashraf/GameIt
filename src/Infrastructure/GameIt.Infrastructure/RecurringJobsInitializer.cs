using GameIt.Application.Interfaces.IDiscount;
using Hangfire;
using Microsoft.Extensions.DependencyInjection;

namespace GameIt.Infrastructure;

public static class RecurringJobsInitializer
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        var recurringJobManager = serviceProvider.GetRequiredService<IRecurringJobManager>();
        var discountService = serviceProvider.GetRequiredService<IDiscountService>();

        recurringJobManager.AddOrUpdate(
            "update-discount-status",
            () => discountService.UpdateDiscountStatusesAsync(),
            Cron.Hourly
        );
        recurringJobManager.AddOrUpdate(
            "remove-expired-discounts",
            () => discountService.RemoveExpiredDiscountsAsync(),
            Cron.Hourly
        );
    }
}
