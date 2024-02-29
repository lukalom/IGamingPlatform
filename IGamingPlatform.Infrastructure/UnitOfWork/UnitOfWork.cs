using IGamingPlatform.Infrastructure.Persistence;
using IGamingPlatform.Infrastructure.UnitOfWork.Abstractions;

namespace IGamingPlatform.Infrastructure.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly GamingPlatformDBContext _dbContext;

    public UnitOfWork(GamingPlatformDBContext dbContext)
    {
        _dbContext = dbContext;

    }

    public async Task<int> SaveAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    //Todo for future adding transactions event publishing etc
}