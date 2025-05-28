using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.FirestoreProvider.Infrastructure
{
    public interface IFirestoreDatabase
    {
        Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class;
        Task<T> GetAsync<T>(string id, CancellationToken cancellationToken = default) where T : class;
        Task UpdateAsync<T>(string id, T entity, CancellationToken cancellationToken = default) where T : class;
        Task DeleteAsync<T>(string id, CancellationToken cancellationToken = default) where T : class; Task<IEnumerable<T>> QueryAsync<T>(Func<Google.Cloud.Firestore.Query, Google.Cloud.Firestore.Query> queryBuilder, CancellationToken cancellationToken = default) where T : class;
        Task<int> CountAsync<T>(Func<Google.Cloud.Firestore.Query, Google.Cloud.Firestore.Query> queryBuilder = null, CancellationToken cancellationToken = default) where T : class;
        Task<bool> ExistsAsync<T>(string id, CancellationToken cancellationToken = default) where T : class;
    }
}
