using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EfCore.FirestoreProvider.Extensions
{
    public static class FirestoreModelBuilderExtensions
    {
        /// <summary>
        /// Configures the entity to map to a specific Firestore collection.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entityTypeBuilder">The entity type builder.</param>
        /// <param name="collectionName">The name of the Firestore collection.</param>
        /// <returns>The same entity type builder instance so that multiple configuration calls can be chained.</returns>
        public static EntityTypeBuilder<TEntity> ToCollection<TEntity>(
            this EntityTypeBuilder<TEntity> entityTypeBuilder,
            string collectionName)
            where TEntity : class
        {
            // Store the collection name in metadata
            entityTypeBuilder.HasAnnotation("Firestore:CollectionName", collectionName);

            // Use the collection name as the table name for EF Core
            entityTypeBuilder.ToTable(collectionName);

            return entityTypeBuilder;
        }

        /// <summary>
        /// Configures the entity to map to a specific Firestore collection.
        /// </summary>
        /// <param name="entityTypeBuilder">The entity type builder.</param>
        /// <param name="collectionName">The name of the Firestore collection.</param>
        /// <returns>The same entity type builder instance so that multiple configuration calls can be chained.</returns>
        public static EntityTypeBuilder ToCollection(
            this EntityTypeBuilder entityTypeBuilder,
            string collectionName)
        {
            // Store the collection name in metadata
            entityTypeBuilder.HasAnnotation("Firestore:CollectionName", collectionName);

            // Use the collection name as the table name for EF Core
            entityTypeBuilder.ToTable(collectionName);

            return entityTypeBuilder;
        }
    }
}
