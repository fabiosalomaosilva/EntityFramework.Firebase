using System;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.FirestoreProvider.Infrastructure
{
    public interface IFirestoreTransactionManager
    {
        Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default);
        Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        bool IsInTransaction { get; }
    }
}
