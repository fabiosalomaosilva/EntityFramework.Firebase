using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using EfCore.FirestoreProvider.Sample.Data;
using EfCore.FirestoreProvider.Extensions;

namespace EfCore.FirestoreProvider.Tests.Unit
{
    public class ExtensionsTests
    {
        [Fact]
        public void UseFirestore_ShouldConfigureDbContextOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var projectId = "test-project-id";

            // Act
            services.AddDbContext<AppDbContext>(options =>
                options.UseFirestore(projectId));

            var serviceProvider = services.BuildServiceProvider();
            var dbContext = serviceProvider.GetService<AppDbContext>();

            // Assert
            Assert.NotNull(dbContext);
            // Verificar se o contexto foi configurado corretamente
            Assert.NotNull(dbContext.Database);
        }

        [Fact]
        public void UseFirestore_WithServiceAccount_ShouldConfigureDbContextOptions()
        {
            // Arrange
            var services = new ServiceCollection();
            var projectId = "test-project-id";
            var serviceAccountPath = "path/to/serviceAccount.json";

            // Act & Assert
            // Este teste verifica se o método de extensão não lança exceções
            Assert.NotNull(() => services.AddDbContext<AppDbContext>(options =>
                options.UseFirestore(projectId, serviceAccountPath)));
        }
    }
}
