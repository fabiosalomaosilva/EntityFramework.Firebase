using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using Grpc.Auth;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using EfCore.FirestoreProvider.Infrastructure;
using EfCore.FirestoreProvider.Query;

namespace EfCore.FirestoreProvider.Extensions;

public class FirestoreDbContextOptionsExtension : IDbContextOptionsExtension
{
    public string ProjectId { get; set; }
    public string ServiceAccountPath { get; set; }

    public void ApplyServices(IServiceCollection services)
    {
        // Registrar o FirestoreDb como singleton
        services.AddSingleton<FirestoreDb>(serviceProvider =>
        {
            if (!string.IsNullOrEmpty(ServiceAccountPath))
            {
                // Usar credenciais do arquivo de service account
                var credential = GoogleCredential.FromFile(ServiceAccountPath);
                var builder = new FirestoreClientBuilder
                {
                    Credential = credential
                };
                var firestoreClient = builder.Build();
                return FirestoreDb.Create(ProjectId, firestoreClient);
            }
            else
            {
                // Usar credenciais padrão (ADC - Application Default Credentials)
                return FirestoreDb.Create(ProjectId);
            }
        });

        // Registrar serviços do provider
        services.AddScoped<IFirestoreDatabase, FirestoreDatabase>();
        services.AddScoped<IFirestoreQueryProvider, FirestoreQueryProvider>();
        services.AddScoped<IFirestoreTransactionManager, FirestoreTransactionManager>();
    }

    public void Validate(IDbContextOptions options)
    {
        if (string.IsNullOrWhiteSpace(ProjectId))
        {
            throw new InvalidOperationException("ProjectId não pode ser nulo ou vazio. Configure o ProjectId do Firestore.");
        }

        if (!string.IsNullOrEmpty(ServiceAccountPath) && !System.IO.File.Exists(ServiceAccountPath))
        {
            throw new InvalidOperationException($"Arquivo de service account não encontrado: {ServiceAccountPath}");
        }
    }

    public DbContextOptionsExtensionInfo Info => new FirestoreDbContextOptionsExtensionInfo(this);

    public void PopulateDebugInfo(IDictionary<string, string> debugInfo)
    {
        debugInfo["ProjectId"] = ProjectId ?? "não configurado";
        debugInfo["ServiceAccountPath"] = ServiceAccountPath ?? "credenciais padrão";
        debugInfo["Provider"] = "Firestore";
    }
}
