using Google.Cloud.Firestore;
using Google.Apis.Auth.OAuth2;
using System;
using System.IO;

namespace EfCore.FirestoreProvider.Infrastructure
{
    public class FirestoreClientFactory
    {
        private readonly string _projectId;
        private readonly string _serviceAccountPath;

        public FirestoreClientFactory(string projectId, string serviceAccountPath = null)
        {
            _projectId = projectId;
            _serviceAccountPath = serviceAccountPath;
        }

        public FirestoreDb CreateClient()
        {
            if (!string.IsNullOrEmpty(_serviceAccountPath) && File.Exists(_serviceAccountPath))
            {
                // Usar credenciais do arquivo de service account
                var credential = GoogleCredential.FromFile(_serviceAccountPath);
                Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", _serviceAccountPath);
            }

            // Criar o client do Firestore
            return FirestoreDb.Create(_projectId);
        }
    }
}
