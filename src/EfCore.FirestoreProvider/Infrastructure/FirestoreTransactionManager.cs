using Google.Cloud.Firestore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace EfCore.FirestoreProvider.Infrastructure
{
    public class FirestoreTransactionManager : IFirestoreTransactionManager
    {
        private readonly FirestoreDb _firestoreDb;
        private Transaction _currentTransaction;

        public FirestoreTransactionManager(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb ?? throw new ArgumentNullException(nameof(firestoreDb));
        }

        public bool IsInTransaction => _currentTransaction != null;

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            return await _firestoreDb.RunTransactionAsync(async transaction =>
            {
                _currentTransaction = transaction;
                try
                {
                    return await operation();
                }
                finally
                {
                    _currentTransaction = null;
                }
            }, cancellationToken: cancellationToken);
        }

        public async Task ExecuteInTransactionAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));

            await _firestoreDb.RunTransactionAsync(async transaction =>
            {
                _currentTransaction = transaction;
                try
                {
                    await operation();
                    return 0; // Retorna um valor dummy para satisfazer a assinatura
                }
                finally
                {
                    _currentTransaction = null;
                }
            }, cancellationToken: cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
                throw new InvalidOperationException("Uma transação já está em andamento.");

            // Nota: O Firestore não suporta transações explícitas como bancos relacionais
            // Este método é fornecido para compatibilidade com o padrão EF Core
            // As transações no Firestore são atômicas e devem ser executadas através de RunTransactionAsync
            await Task.CompletedTask;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("Não há transação ativa para fazer commit.");

            // Nota: O commit é feito automaticamente pelo Firestore quando RunTransactionAsync completa com sucesso
            await Task.CompletedTask;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction == null)
                throw new InvalidOperationException("Não há transação ativa para fazer rollback.");

            // Nota: O rollback é feito automaticamente pelo Firestore quando RunTransactionAsync falha
            _currentTransaction = null;
            await Task.CompletedTask;
        }
    }
}
