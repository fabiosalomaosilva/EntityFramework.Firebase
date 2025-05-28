using Microsoft.EntityFrameworkCore;
using EfCore.FirestoreProvider.Extensions;
using EfCore.FirestoreProvider.Sample.Models;

namespace EfCore.FirestoreProvider.Sample.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Temporarily use ToTable instead of ToCollection while we debug the extension method
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Ticket>().ToTable("tickets");

            // Add Firestore-specific annotations directly
            modelBuilder.Entity<User>().HasAnnotation("Firestore:CollectionName", "users");
            modelBuilder.Entity<Ticket>().HasAnnotation("Firestore:CollectionName", "tickets");
        }
    }
}
