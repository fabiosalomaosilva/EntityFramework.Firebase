using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Google.Cloud.Firestore;
using EfCore.FirestoreProvider.Sample.Data;
using EfCore.FirestoreProvider.Extensions;

namespace EfCore.FirestoreProvider.Tests.Helpers
{
    public class FirestoreFixture : IDisposable
    {
        public AppDbContext Context { get; private set; }
        private readonly FirestoreDb _firestoreDb;
        private readonly string _projectId = "test-project-id"; public FirestoreFixture()
        {
            // Configure para usar o emulador do Firestore
            Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", "localhost:8080");

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseFirestore(_projectId)
                .Options;

            Context = new AppDbContext(options);

            // Inicializar FirestoreDb para limpeza
            _firestoreDb = FirestoreDb.Create(_projectId);
        }

        public async Task ClearAllCollections()
        {
            try
            {
                // Limpar coleção users
                var usersRef = _firestoreDb.Collection("users");
                var usersSnapshot = await usersRef.GetSnapshotAsync();
                foreach (var doc in usersSnapshot.Documents)
                {
                    await doc.Reference.DeleteAsync();
                }

                // Limpar coleção tickets
                var ticketsRef = _firestoreDb.Collection("tickets");
                var ticketsSnapshot = await ticketsRef.GetSnapshotAsync();
                foreach (var doc in ticketsSnapshot.Documents)
                {
                    await doc.Reference.DeleteAsync();
                }
            }
            catch (Exception)
            {
                // Ignorar erros de limpeza (pode não haver dados)
            }
        }

        public void Dispose()
        {
            Context?.Dispose();
        }
    }
}
