using Domain.Common;

namespace Application.Common.Extensions;

public static class QueryableOwnershipExtensions
{
    public static IQueryable<TEntity> WhereCreatedBy<TEntity>(this IQueryable<TEntity> query, string userId)
        where TEntity : BaseEntity =>
        query.Where(entity => entity.CreatedBy == userId);
}
