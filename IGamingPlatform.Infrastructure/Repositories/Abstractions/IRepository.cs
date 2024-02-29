using IGamingPlatform.Domain.Abstractions;
using System.Linq.Expressions;

namespace IGamingPlatform.Infrastructure.Repositories.Abstractions;

public interface IRepository<T> where T : Entity
{
    Task<T?> FindAsync(int id, bool onlyActive = true);

    IQueryable<T> Query(Expression<Func<T, bool>>? expression = null, bool onlyActives = true);

    Task StoreAsync(T document);
}