namespace PlanIt.Application.Common.Interfaces.Stores;

public interface IIdempotencyStore<T>
{
    Task<T?> GetAsync(string key);
    Task SaveAsync(string key, T record, TimeSpan expiry);
}