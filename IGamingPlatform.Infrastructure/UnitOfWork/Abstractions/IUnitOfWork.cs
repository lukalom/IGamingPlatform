namespace IGamingPlatform.Infrastructure.UnitOfWork.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveAsync();
}