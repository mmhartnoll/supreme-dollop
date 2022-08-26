using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.DataAccess.Context
{
    public class TransactionScope : IDisposable, IAsyncDisposable
    {
        private readonly DatabaseContext context;
        private readonly DbConnection connection;
        private readonly DbTransaction transaction;
        private bool isActive = true;
        private bool isDisposed = false;

        private TransactionScope(DatabaseContext context, DbConnection connection, DbTransaction transaction)
        {
            this.context = context;
            this.connection = connection;
            this.transaction = transaction;
        }

        public static TransactionScope Create(DatabaseContext context, string dbConnectionString, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            var connection = new SqlConnection(dbConnectionString);
            connection.Open();
            var transaction = connection.BeginTransaction(isolationLevel);
            return new TransactionScope(context, connection, transaction);
        }

        public static async Task<TransactionScope> CreateAsync(DatabaseContext context, string dbConnectionString, IsolationLevel isolationLevel = IsolationLevel.Serializable, CancellationToken cancellationToken = default)
        {
            var connection = new SqlConnection(dbConnectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            var transaction = await connection.BeginTransactionAsync(isolationLevel).ConfigureAwait(false);
            return new TransactionScope(context, connection, transaction);
        }

        public void Execute(Action operation)
            => context.Execute(operation, this);

        public Task ExecuteAsync(Func<Task> operation)
            => context.ExecuteAsync(operation, this);

        public TResult Execute<TResult>(Func<TResult> operation)
            => context.Execute(operation, this);

        public Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation)
            => context.ExecuteAsync(operation, this);

        public IEnumerable<TResult> ExecuteEnumerable<TResult>(Func<IEnumerable<TResult>> operation)
        {
            foreach (var result in context.ExecuteEnumerable(operation, this))
                yield return result;
        }

        public async IAsyncEnumerable<TResult> ExecuteStreamAsync<TResult>(Func<IAsyncEnumerable<TResult>> operation)
        {
            await foreach (var result in context.ExecuteStreamAsync(operation, this).ConfigureAwait(false))
                yield return result;
        }

        public void Commit()
        {
            transaction.Commit();
            isActive = false;
        }

        public async Task CommitAsync()
        {
            await transaction.CommitAsync().ConfigureAwait(false);
            isActive = false;
        }

        public void Rollback()
        {
            transaction.Rollback();
            isActive = false;
        }

        public async Task RollbackAsync()
        {
            await transaction.RollbackAsync().ConfigureAwait(false);
            isActive = false;
        }

        internal DbCommand CreateCommand()
        {
            var command = connection.CreateCommand();
            command.Transaction = transaction;
            return command;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsync(true).ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
                return;
            if (disposing)
            {
                if (isActive)
                    transaction.Rollback();
                transaction.Dispose();
                connection.Close();
                connection.Dispose();
            }
            isActive = false;
            isDisposed = true;
        }

        protected async virtual Task DisposeAsync(bool disposing)
        {
            if (isDisposed)
                return;
            if (disposing)
            {
                if (isActive)
                    await transaction.RollbackAsync().ConfigureAwait(false);
                await transaction.DisposeAsync().ConfigureAwait(false);
                await connection.CloseAsync().ConfigureAwait(false);
                await connection.DisposeAsync().ConfigureAwait(false);
            }
            isActive = false;
            isDisposed = true;
        }
    }
}
