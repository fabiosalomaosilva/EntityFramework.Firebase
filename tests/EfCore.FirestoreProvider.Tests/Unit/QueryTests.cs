using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xunit;
using EfCore.FirestoreProvider.Sample.Data;
using EfCore.FirestoreProvider.Sample.Models;
using EfCore.FirestoreProvider.Extensions;

namespace EfCore.FirestoreProvider.Tests.Unit
{
    public class QueryTests
    {
        private readonly AppDbContext _context;

        public QueryTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseFirestore("firestore-project-id")
                .Options;

            _context = new AppDbContext(options);
        }

        [Fact]
        public async Task Can_Query_Users()
        {
            // Arrange
            var user = new User { Id = "1", FullName = "John Doe", DateOfBirth = new DateTime(1990, 1, 1) };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _context.Users.Where(u => u.FullName == "John Doe").ToListAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("John Doe", result.First().FullName);
        }

        [Fact]
        public async Task Can_Query_Tickets()
        {
            // Arrange
            var ticket = new Ticket { Id = "1", Title = "Sample Ticket", CreatedAt = DateTime.UtcNow };
            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            // Act
            var result = await _context.Tickets.Where(t => t.Title == "Sample Ticket").ToListAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Sample Ticket", result.First().Title);
        }

        [Fact]
        public async Task Can_Use_AsNoTracking()
        {
            // Arrange
            var user = new User { Id = "2", FullName = "Jane Doe", DateOfBirth = new DateTime(1992, 2, 2) };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            // Act
            var result = await _context.Users.AsNoTracking().ToListAsync();

            // Assert
            Assert.Single(result);
            Assert.Equal("Jane Doe", result.First().FullName);
        }

        [Fact]
        public async Task Can_Count_Users()
        {
            // Arrange
            await _context.Users.AddAsync(new User { Id = "3", FullName = "Alice", DateOfBirth = new DateTime(1995, 3, 3) });
            await _context.SaveChangesAsync();

            // Act
            var count = await _context.Users.CountAsync();

            // Assert
            Assert.Equal(1, count);
        }
    }
}
