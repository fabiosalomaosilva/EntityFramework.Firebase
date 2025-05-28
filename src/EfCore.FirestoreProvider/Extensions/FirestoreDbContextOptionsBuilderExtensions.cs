using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace EfCore.FirestoreProvider.Extensions
{
    public static class FirestoreDbContextOptionsBuilderExtensions
    {
        public static DbContextOptionsBuilder UseFirestore(
            this DbContextOptionsBuilder optionsBuilder,
            string projectId,
            string serviceAccountPath)
        {
            var extension = new FirestoreDbContextOptionsExtension
            {
                ProjectId = projectId,
                ServiceAccountPath = serviceAccountPath
            };

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder UseFirestore(
            this DbContextOptionsBuilder optionsBuilder,
            string projectId)
        {
            var extension = new FirestoreDbContextOptionsExtension
            {
                ProjectId = projectId,
                ServiceAccountPath = null
            };

            ((IDbContextOptionsBuilderInfrastructure)optionsBuilder)
                .AddOrUpdateExtension(extension);

            return optionsBuilder;
        }

        public static DbContextOptionsBuilder<TContext> UseFirestore<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            string projectId,
            string serviceAccountPath)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)UseFirestore(
                (DbContextOptionsBuilder)optionsBuilder,
                projectId,
                serviceAccountPath);
        }

        public static DbContextOptionsBuilder<TContext> UseFirestore<TContext>(
            this DbContextOptionsBuilder<TContext> optionsBuilder,
            string projectId)
            where TContext : DbContext
        {
            return (DbContextOptionsBuilder<TContext>)UseFirestore(
                (DbContextOptionsBuilder)optionsBuilder,
                projectId);
        }
    }
}
