namespace Common.Domain;

public interface IRepository<T> where T : AggregateRoot
{
    IUnitOfWork UnitOfWork { get; }
}

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default);
}
