using GameIt.Application.Interfaces.Persistence;
using GameIt.Domain;
using GameIt.Persistence.DatabaseContext;

namespace GameIt.Persistence.Repositories;

public class CommentRepository : GenericRepository<Comment>, ICommentRepository
{
    public CommentRepository(GameItDbContext context) : base(context) { }
}

