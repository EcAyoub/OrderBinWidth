using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure;

public static class QueryExtensions
{

    public static async Task<TEntity?> SelectAsync<TEntity>(
      this IQueryable<TEntity> source,
      Expression<Func<TEntity, bool>> predicate,
      CancellationToken ct = default)
      where TEntity : class
    {
        var filtered = source.Where(predicate);

        var list = await filtered.Take(1).ToListAsync(ct);

        return list.Count > 0 ? list[0] : null;
    }
}
