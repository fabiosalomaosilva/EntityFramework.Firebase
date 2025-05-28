using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EfCore.FirestoreProvider.Extensions;

namespace EfCore.FirestoreProvider.Tests.Unit
{
    public class ModelTests
    {
        private class TestDbContext : DbContext
        {
            public DbSet<User> Users { get; set; }
            public DbSet<Ticket> Tickets { get; set; }

            public TestDbContext(DbContextOptions<TestDbContext> options)
                : base(options) { }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<User>().ToTable("users");
                modelBuilder.Entity<Ticket>().ToTable("tickets");

                // Adicionar anotações do Firestore
                modelBuilder.Entity<User>().HasAnnotation("Firestore:CollectionName", "users");
                modelBuilder.Entity<Ticket>().HasAnnotation("Firestore:CollectionName", "tickets");
            }
        }

        private class User
        {
            public string Id { get; set; }
            public string FullName { get; set; }
            public DateTime DateOfBirth { get; set; }
        }

        private class Ticket
        {
            public string Id { get; set; }
            public string Title { get; set; }
            public DateTime CreatedAt { get; set; }
        }

        [Fact]
        public async Task CanAddUser()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseFirestore("test-project-id", "path/to/serviceAccount.json")
                .Options;

            using (var context = new TestDbContext(options))
            {
                var user = new User { Id = "1", FullName = "John Doe", DateOfBirth = new DateTime(1990, 1, 1) };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var user = await context.Users.FindAsync("1");
                Assert.NotNull(user);
                Assert.Equal("John Doe", user.FullName);
            }
        }

        [Fact]
        public async Task CanUpdateUser()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseFirestore("test-project-id", "path/to/serviceAccount.json")
                .Options;

            using (var context = new TestDbContext(options))
            {
                var user = new User { Id = "2", FullName = "Jane Doe", DateOfBirth = new DateTime(1992, 2, 2) };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var user = await context.Users.FindAsync("2");
                user.FullName = "Jane Smith";
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var user = await context.Users.FindAsync("2");
                Assert.Equal("Jane Smith", user.FullName);
            }
        }

        [Fact]
        public async Task CanDeleteUser()
        {
            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseFirestore("test-project-id", "path/to/serviceAccount.json")
                .Options;

            using (var context = new TestDbContext(options))
            {
                var user = new User { Id = "3", FullName = "Mark Smith", DateOfBirth = new DateTime(1985, 3, 3) };
                context.Users.Add(user);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var user = await context.Users.FindAsync("3");
                context.Users.Remove(user);
                await context.SaveChangesAsync();
            }

            using (var context = new TestDbContext(options))
            {
                var user = await context.Users.FindAsync("3");
                Assert.Null(user);
            }
        }
    }
}
