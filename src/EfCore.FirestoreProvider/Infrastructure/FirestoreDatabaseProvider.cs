using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.FirestoreProvider.Infrastructure
{
    public class FirestoreDatabaseProvider
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreDatabaseProvider(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb ?? throw new ArgumentNullException(nameof(firestoreDb));
        }

        public async Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            var collection = GetCollectionName<T>();
            await _firestoreDb.Collection(collection).AddAsync(entity, cancellationToken);
        }

        public async Task<T> GetAsync<T>(string id, CancellationToken cancellationToken = default) where T : class
        {
            var collection = GetCollectionName<T>();
            var document = await _firestoreDb.Collection(collection).Document(id).GetSnapshotAsync(cancellationToken);
            return document.Exists ? document.ConvertTo<T>() : null;
        }

        public async Task UpdateAsync<T>(string id, T entity, CancellationToken cancellationToken = default) where T : class
        {
            var collection = GetCollectionName<T>();
            await _firestoreDb.Collection(collection).Document(id).SetAsync(entity, SetOptions.MergeAll, cancellationToken);
        }

        public async Task DeleteAsync<T>(string id, CancellationToken cancellationToken = default) where T : class
        {
            var collection = GetCollectionName<T>();
            await _firestoreDb.Collection(collection).Document(id).DeleteAsync(precondition: null, cancellationToken);
        }

        public async Task<IEnumerable<T>> QueryAsync<T>(Func<Google.Cloud.Firestore.Query, Google.Cloud.Firestore.Query> queryBuilder, CancellationToken cancellationToken = default) where T : class
        {
            var collection = GetCollectionName<T>();
            var baseQuery = _firestoreDb.Collection(collection);
            var query = queryBuilder?.Invoke(baseQuery) ?? baseQuery;
            var snapshot = await query.GetSnapshotAsync(cancellationToken);
            var results = new List<T>();

            foreach (var document in snapshot.Documents)
            {
                results.Add(document.ConvertTo<T>());
            }

            return results;
        }

        private string GetCollectionName<T>()
        {
            return typeof(T).Name.ToLowerInvariant() + "s"; // Assumes collection name is pluralized entity name
        }
    }
}
