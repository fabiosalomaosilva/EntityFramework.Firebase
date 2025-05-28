using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EfCore.FirestoreProvider.Sample.Data;
using EfCore.FirestoreProvider.Extensions;
using System;
using System.Threading.Tasks;

namespace EfCore.FirestoreProvider.Sample
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    // Aqui você pode adicionar lógica para inicializar o banco de dados, se necessário
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erro ao inicializar o banco de dados: {ex.Message}");
                }
            }

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((context, services) =>
                {
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseFirestore("firestore-project-id", "path/to/serviceAccount.json"));
                });
    }
}
