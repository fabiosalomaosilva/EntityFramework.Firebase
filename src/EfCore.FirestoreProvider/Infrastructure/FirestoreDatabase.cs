using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.FirestoreProvider.Infrastructure
{
    public class FirestoreDatabase : IFirestoreDatabase
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreDatabase(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb ?? throw new ArgumentNullException(nameof(firestoreDb));
        }

        public async Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            var collection = GetCollectionName<T>();
            var documentId = GetDocumentId(entity);
            if (string.IsNullOrEmpty(documentId))
            {
                await _firestoreDb.Collection(collection).AddAsync(entity, cancellationToken);
            }
            else
            {
                await _firestoreDb.Collection(collection).Document(documentId).SetAsync(entity, SetOptions.Overwrite, cancellationToken);
            }
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

        public async Task<int> CountAsync<T>(Func<Google.Cloud.Firestore.Query, Google.Cloud.Firestore.Query> queryBuilder = null, CancellationToken cancellationToken = default) where T : class
        {
            var collection = GetCollectionName<T>();
            var baseQuery = _firestoreDb.Collection(collection);
            var query = queryBuilder?.Invoke(baseQuery) ?? baseQuery;

            var snapshot = await query.GetSnapshotAsync(cancellationToken);
            return snapshot.Count;
        }

        public async Task<bool> ExistsAsync<T>(string id, CancellationToken cancellationToken = default) where T : class
        {
            var collection = GetCollectionName<T>();
            var document = await _firestoreDb.Collection(collection).Document(id).GetSnapshotAsync(cancellationToken);
            return document.Exists;
        }

        private string GetCollectionName<T>()
        {
            // Primeiro tenta obter o nome da coleção através de atributos ou convenções
            var entityType = typeof(T);

            // Verifica se há um atributo FirestoreCollection
            var collectionAttribute = entityType.GetCustomAttributes(typeof(Model.FirestoreCollectionAttribute), false)
                                               .FirstOrDefault() as Model.FirestoreCollectionAttribute;

            if (collectionAttribute != null)
            {
                return collectionAttribute.CollectionName;
            }

            // Se não há atributo, usa convenção: nome da classe em minúsculas e pluralizado
            return entityType.Name.ToLowerInvariant() + "s";
        }

        private string GetDocumentId<T>(T entity) where T : class
        {
            var entityType = typeof(T);

            // Procura por propriedades com atributo FirestoreDocumentId
            var documentIdProperty = entityType.GetProperties()
                .FirstOrDefault(p => p.GetCustomAttributes(typeof(Model.FirestoreDocumentIdAttribute), false).Any());

            if (documentIdProperty != null)
            {
                return documentIdProperty.GetValue(entity)?.ToString();
            }

            // Se não encontrou, procura por propriedade chamada "Id"
            var idProperty = entityType.GetProperty("Id");
            if (idProperty != null)
            {
                return idProperty.GetValue(entity)?.ToString();
            }

            return null;
        }
    }
}
