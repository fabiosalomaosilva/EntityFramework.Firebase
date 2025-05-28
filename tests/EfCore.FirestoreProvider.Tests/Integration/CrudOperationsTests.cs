using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EfCore.FirestoreProvider.Tests.Helpers;
using EfCore.FirestoreProvider.Sample.Models;

namespace EfCore.FirestoreProvider.Tests.Integration
{
    public class CrudOperationsTests : IClassFixture<FirestoreFixture>
    {
        private readonly FirestoreFixture _fixture;

        public CrudOperationsTests(FirestoreFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Create_User_Should_Add_User_To_Firestore()
        {
            var user = new User { Id = "1", FullName = "John Doe", DateOfBirth = new DateTime(1990, 1, 1) };

            await _fixture.Context.Users.AddAsync(user);
            await _fixture.Context.SaveChangesAsync();

            var retrievedUser = await _fixture.Context.Users.FindAsync("1");
            Assert.NotNull(retrievedUser);
            Assert.Equal("John Doe", retrievedUser.FullName);
        }

        [Fact]
        public async Task Read_User_Should_Return_User_From_Firestore()
        {
            var user = new User { Id = "2", FullName = "Jane Doe", DateOfBirth = new DateTime(1992, 2, 2) };

            await _fixture.Context.Users.AddAsync(user);
            await _fixture.Context.SaveChangesAsync();

            var retrievedUser = await _fixture.Context.Users.FindAsync("2");
            Assert.NotNull(retrievedUser);
            Assert.Equal("Jane Doe", retrievedUser.FullName);
        }

        [Fact]
        public async Task Update_User_Should_Modify_User_In_Firestore()
        {
            var user = new User { Id = "3", FullName = "Alice Smith", DateOfBirth = new DateTime(1995, 3, 3) };

            await _fixture.Context.Users.AddAsync(user);
            await _fixture.Context.SaveChangesAsync();

            user.FullName = "Alice Johnson";
            _fixture.Context.Users.Update(user);
            await _fixture.Context.SaveChangesAsync();

            var updatedUser = await _fixture.Context.Users.FindAsync("3");
            Assert.NotNull(updatedUser);
            Assert.Equal("Alice Johnson", updatedUser.FullName);
        }

        [Fact]
        public async Task Delete_User_Should_Remove_User_From_Firestore()
        {
            var user = new User { Id = "4", FullName = "Bob Brown", DateOfBirth = new DateTime(1988, 4, 4) };

            await _fixture.Context.Users.AddAsync(user);
            await _fixture.Context.SaveChangesAsync();

            _fixture.Context.Users.Remove(user);
            await _fixture.Context.SaveChangesAsync();

            var deletedUser = await _fixture.Context.Users.FindAsync("4");
            Assert.Null(deletedUser);
        }
    }
}
