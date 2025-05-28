using System;
using System.Threading.Tasks;
using Xunit;
using Google.Cloud.Firestore;
using EfCore.FirestoreProvider.Sample.Models;

namespace EfCore.FirestoreProvider.Tests.Integration
{
    public class FirestoreEmulatorTests : IAsyncLifetime
    {
        private readonly FirestoreDb _firestoreDb;
        private readonly string _projectId = "test-project-id";

        public FirestoreEmulatorTests()
        {
            // Configure para usar o emulador do Firestore se disponível
            Environment.SetEnvironmentVariable("FIRESTORE_EMULATOR_HOST", "localhost:8080");
            _firestoreDb = FirestoreDb.Create(_projectId);
        }

        public async Task InitializeAsync()
        {
            // Setup code to run before each test
            await ClearFirestoreCollections();
        }

        public async Task DisposeAsync()
        {
            // Cleanup code to run after each test
            await ClearFirestoreCollections();
        }

        private async Task ClearFirestoreCollections()
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
                // Ignorar erros se as coleções não existirem
            }
        }

        [Fact]
        public async Task CreateUser_ShouldAddUserToFirestore()
        {
            // Arrange
            var user = new User { Id = "1", FullName = "Test User", DateOfBirth = DateTime.UtcNow };

            // Act
            await _firestoreDb.Collection("users").Document(user.Id).SetAsync(user);

            // Assert
            var doc = await _firestoreDb.Collection("users").Document(user.Id).GetSnapshotAsync();
            Assert.True(doc.Exists);
            Assert.Equal(user.FullName, doc.ConvertTo<User>().FullName);
        }

        [Fact]
        public async Task ReadUser_ShouldReturnUserFromFirestore()
        {
            // Arrange
            var user = new User { Id = "2", FullName = "Another User", DateOfBirth = DateTime.UtcNow };
            await _firestoreDb.Collection("users").Document(user.Id).SetAsync(user);

            // Act
            var doc = await _firestoreDb.Collection("users").Document(user.Id).GetSnapshotAsync();

            // Assert
            Assert.True(doc.Exists);
            Assert.Equal(user.FullName, doc.ConvertTo<User>().FullName);
        }

        [Fact]
        public async Task UpdateUser_ShouldModifyUserInFirestore()
        {
            // Arrange
            var user = new User { Id = "3", FullName = "Original Name", DateOfBirth = DateTime.UtcNow };
            await _firestoreDb.Collection("users").Document(user.Id).SetAsync(user);

            // Act
            user.FullName = "Updated Name";
            await _firestoreDb.Collection("users").Document(user.Id).SetAsync(user);

            // Assert
            var doc = await _firestoreDb.Collection("users").Document(user.Id).GetSnapshotAsync();
            Assert.True(doc.Exists);
            Assert.Equal("Updated Name", doc.ConvertTo<User>().FullName);
        }

        [Fact]
        public async Task DeleteUser_ShouldRemoveUserFromFirestore()
        {
            // Arrange
            var user = new User { Id = "4", FullName = "To Be Deleted", DateOfBirth = DateTime.UtcNow };
            await _firestoreDb.Collection("users").Document(user.Id).SetAsync(user);

            // Act
            await _firestoreDb.Collection("users").Document(user.Id).DeleteAsync();

            // Assert
            var doc = await _firestoreDb.Collection("users").Document(user.Id).GetSnapshotAsync();
            Assert.False(doc.Exists);
        }
    }
}
