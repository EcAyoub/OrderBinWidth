using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure;

public static class QueryExtensions
{

    public static Task<TEntity?> SelectAsync<TEntity>(
           this IQueryable<TEntity> source,
           Expression<Func<TEntity, bool>> predicate,
           CancellationToken ct = default)
           where TEntity : class
    {
        return source.FirstOrDefaultAsync(predicate, ct);
    }
}
