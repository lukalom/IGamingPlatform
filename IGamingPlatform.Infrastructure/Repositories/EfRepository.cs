using System.Linq.Expressions;
using IGamingPlatform.Domain.Abstractions;
using IGamingPlatform.Domain.Enums;
using IGamingPlatform.Infrastructure.Persistence;
using IGamingPlatform.Infrastructure.Repositories.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IGamingPlatform.Infrastructure.Repositories;

public class EfRepository<TEntity> : IRepository<TEntity>
    where TEntity : Entity
{
    protected readonly GamingPlatformDBContext Db;

    public EfRepository(GamingPlatformDBContext db)
    {
        Db = db;
    }

    public virtual async Task<TEntity?> FindAsync(int id, bool onlyActive = true)
    {
        return onlyActive
            ? await Db.Set<TEntity>().Where(x => x.EntityStatus == EntityStatus.Active).SingleOrDefaultAsync(x => x.Id == id)
            : await Db.Set<TEntity>().SingleOrDefaultAsync(x => x.Id == id);
    }

    public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>>? expression = null, bool onlyActives = true)
    {
        var query = onlyActives
            ? Db.Set<TEntity>().Where(x => x.EntityStatus == EntityStatus.Active)
            : Db.Set<TEntity>();

        return expression == null ? query : query.Where(expression);
    }

    public virtual async Task StoreAsync(TEntity document)
    {
        await Db.Set<TEntity>().AddAsync(document);
    }
}