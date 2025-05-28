using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace EfCore.FirestoreProvider.Infrastructure
{
    public class FirestoreProviderServices
    {
        private readonly IServiceProvider _serviceProvider;

        public FirestoreProviderServices(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<FirestoreDatabaseProvider>();
            services.AddScoped<FirestoreCommandExecutor>();
            services.AddScoped<FirestoreClientFactory>();
        }

        public FirestoreDatabaseProvider GetDatabaseProvider()
        {
            return _serviceProvider.GetRequiredService<FirestoreDatabaseProvider>();
        }

        public FirestoreCommandExecutor GetCommandExecutor()
        {
            return _serviceProvider.GetRequiredService<FirestoreCommandExecutor>();
        }

        public FirestoreClientFactory GetClientFactory()
        {
            return _serviceProvider.GetRequiredService<FirestoreClientFactory>();
        }
    }
}
