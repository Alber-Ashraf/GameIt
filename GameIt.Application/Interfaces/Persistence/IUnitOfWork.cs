
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameIt.Application.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        IGameRepository Games { get; }
        IWishlistRepository Wishlists { get; }
        IDiscountRepository Discounts { get; }
        IReviewRepository Reviews { get; }
        ICommentRepository Comments { get; }
        ICategoryRepository Categories { get; }
        IPurchaseRepository Purchases { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
