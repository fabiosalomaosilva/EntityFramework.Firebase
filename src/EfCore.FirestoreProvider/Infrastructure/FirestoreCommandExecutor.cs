using Google.Cloud.Firestore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EfCore.FirestoreProvider.Infrastructure
{
    public class FirestoreCommandExecutor
    {
        private readonly FirestoreDb _firestoreDb;

        public FirestoreCommandExecutor(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            var collection = GetCollection<T>();
            await collection.AddAsync(entity);
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            var collection = GetCollection<T>();
            var documentId = GetDocumentId(entity);
            await collection.Document(documentId).SetAsync(entity, SetOptions.MergeAll);
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            var collection = GetCollection<T>();
            var documentId = GetDocumentId(entity);
            await collection.Document(documentId).DeleteAsync();
        }

        private CollectionReference GetCollection<T>()
        {
            var collectionName = typeof(T).Name.ToLower() + "s"; // Assuming pluralized collection names
            return _firestoreDb.Collection(collectionName);
        }

        private string GetDocumentId<T>(T entity)
        {
            var idProperty = typeof(T).GetProperty("Id");
            if (idProperty == null)
            {
                throw new InvalidOperationException($"Entity type {typeof(T).Name} does not have an Id property.");
            }
            return idProperty.GetValue(entity)?.ToString() ?? throw new InvalidOperationException("Id cannot be null.");
        }
    }
}