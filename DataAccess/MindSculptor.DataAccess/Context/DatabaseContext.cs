using MindSculptor.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.DataAccess.Context
{
    public abstract class DatabaseContext : IDisposable, IAsyncDisposable
    {
        private readonly string connectionString;
        private readonly AsyncLocal<NullableReference<TransactionScope>> localTransactionScope = new AsyncLocal<NullableReference<TransactionScope>>();

        private bool isDisposed = false;

        private NullableReference<TransactionScope> LocalTransactionScope => localTransactionScope.Value;

        protected DatabaseContext(string connectionString)
        {
            this.connectionString = connectionString;
            localTransactionScope.Value = null;
        }

        public TransactionScope BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable)
            => TransactionScope.Create(this, connectionString, isolationLevel);

        public async Task<TransactionScope> BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable, CancellationToken cancellationToken = default)
            => await TransactionScope.CreateAsync(this, connectionString, isolationLevel, cancellationToken).ConfigureAwait(false);

        internal void Execute(Action operation, TransactionScope transactionScope)
        {
            SetLocalTransactionScope(transactionScope);
            operation.Invoke();
        }

        internal async Task ExecuteAsync(Func<Task> operation, TransactionScope transactionScope)
        {
            SetLocalTransactionScope(transactionScope);
            await operation.Invoke().ConfigureAwait(false);
        }

        internal TResult Execute<TResult>(Func<TResult> operation, TransactionScope transactionScope)
        {
            SetLocalTransactionScope(transactionScope);
            return operation.Invoke();
        }

        internal async Task<TResult> ExecuteAsync<TResult>(Func<Task<TResult>> operation, TransactionScope transactionScope)
        {
            SetLocalTransactionScope(transactionScope);
            return await operation.Invoke().ConfigureAwait(false);
        }

        internal IEnumerable<TResult> ExecuteEnumerable<TResult>(Func<IEnumerable<TResult>> operation, TransactionScope transactionScope)
        {
            SetLocalTransactionScope(transactionScope);
            foreach (var result in operation.Invoke())
                yield return result;
        }

        internal async IAsyncEnumerable<TResult> ExecuteStreamAsync<TResult>(Func<IAsyncEnumerable<TResult>> operation, TransactionScope transactionScope)
        {
            SetLocalTransactionScope(transactionScope);
            await foreach (var result in operation.Invoke().ConfigureAwait(false))
                yield return result;
        }

        public void Execute(Action<DbCommand> operation)
        {
            if (LocalTransactionScope.HasValue)
            {
                using var command = LocalTransactionScope.Value.CreateCommand();
                operation.Invoke(command);
            }
            else
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                operation.Invoke(command);
                connection.Close();
            }
        }

        public async Task ExecuteAsync(Func<DbCommand, CancellationToken, Task> operation, CancellationToken cancellationToken = default)
        {
            if (LocalTransactionScope.HasValue)
            {
                var command = LocalTransactionScope.Value.CreateCommand();
                await using (command.ConfigureAwait(false))
                    await operation.Invoke(command, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var connection = new SqlConnection(connectionString);
                await using (connection.ConfigureAwait(false))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    var command = connection.CreateCommand();
                    await using (command.ConfigureAwait(false))
                    {
                        await operation.Invoke(command, cancellationToken).ConfigureAwait(false);
                        await connection.CloseAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        public TResult Execute<TResult>(Func<DbCommand, TResult> operation)
        {
            if (LocalTransactionScope.HasValue)
            {
                using var command = LocalTransactionScope.Value.CreateCommand();
                return operation.Invoke(command);
            }
            else
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                var result = operation.Invoke(command);
                connection.Close();
                return result;
            }
        }

        public async Task<TResult> ExecuteAsync<TResult>(Func<DbCommand, CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            if (LocalTransactionScope.HasValue)
            {
                var command = LocalTransactionScope.Value.CreateCommand();
                await using (command.ConfigureAwait(false))
                    return await operation.Invoke(command, cancellationToken).ConfigureAwait(false);
            }
            else
            {
                var connection = new SqlConnection(connectionString);
                await using (connection.ConfigureAwait(false))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    var command = connection.CreateCommand();
                    await using (command.ConfigureAwait(false))
                    {
                        var result = await operation.Invoke(command, cancellationToken).ConfigureAwait(false);
                        await connection.CloseAsync().ConfigureAwait(false);
                        return result;
                    }
                }
            }
        }

        public IEnumerable<TResult> Enumerate<TResult>(Func<DbCommand, IEnumerable<TResult>> operation)
        {
            if (LocalTransactionScope.HasValue)
            {
                using var command = LocalTransactionScope.Value.CreateCommand();
                foreach (var result in operation.Invoke(command))
                    yield return result;
            }
            else
            {
                using var connection = new SqlConnection(connectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                foreach (var result in operation.Invoke(command))
                    yield return result;
                connection.CloseAsync();
            }
        }

        public async IAsyncEnumerable<TResult> StreamAsync<TResult>(Func<DbCommand, CancellationToken, IAsyncEnumerable<TResult>> operation, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            if (LocalTransactionScope.HasValue)
            {
                var command = LocalTransactionScope.Value.CreateCommand();
                await using (command.ConfigureAwait(false))
                    await foreach (var result in operation.Invoke(command, cancellationToken).ConfigureAwait(false))
                        yield return result;
            }
            else
            {
                var connection = new SqlConnection(connectionString);
                await using (connection.ConfigureAwait(false))
                {
                    await connection.OpenAsync().ConfigureAwait(false);
                    var command = connection.CreateCommand();
                    await using (command.ConfigureAwait(false))
                    {
                        await foreach (var result in operation.Invoke(command, cancellationToken).ConfigureAwait(false))
                            yield return result;
                        await connection.CloseAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        private void SetLocalTransactionScope(TransactionScope transactionScope)
            => localTransactionScope.Value = transactionScope;

        #region Obsolete

        //public void ProcessWithTransaction(Action action, IsolationLevel isolationLevel = IsolationLevel.Serializable, Action<Exception>? onException = null)
        //{
        //    try
        //    {
        //        BeginTransaction(isolationLevel);
        //        action.Invoke();
        //        CommitTransaction();
        //    }
        //    catch (Exception ex)
        //    {
        //        RollbackTransaction();
        //        if (onException == null)
        //            throw;
        //        onException.Invoke(ex);
        //    }
        //}

        //public TResult ProcessWithTransaction<TResult>(Func<TResult> function, IsolationLevel isolationLevel = IsolationLevel.Serializable, Func<Exception, TResult>? onException = null)
        //{
        //    try
        //    {
        //        BeginTransaction(isolationLevel);
        //        var value = function.Invoke();
        //        CommitTransaction();
        //        return value;
        //    }
        //    catch (Exception ex)
        //    {
        //        RollbackTransaction();
        //        if (onException == null)
        //            throw;
        //        return onException.Invoke(ex);
        //    }
        //}

        //public async Task ProcessWithTransactionAsync(Func<Task> function, IsolationLevel isolationLevel = IsolationLevel.Serializable, Action<Exception>? onException = null)
        //{
        //    try
        //    {
        //        await BeginTransactionAsync(isolationLevel).ConfigureAwait(false);
        //        await function.Invoke().ConfigureAwait(false);
        //        await CommitTransactionAsync().ConfigureAwait(false);
        //    }
        //    catch (Exception ex)
        //    {
        //        await RollbackTransactionAsync().ConfigureAwait(false);
        //        if (onException == null)
        //            throw;
        //        onException.Invoke(ex);
        //    }
        //}

        //public async Task<TResult> ProcessWithTransactionAsync<TResult>(Func<Task<TResult>> function, IsolationLevel isolationLevel = IsolationLevel.Serializable, Func<Exception, Task<TResult>>? onException = null)
        //{
        //    try
        //    {
        //        await BeginTransactionAsync(isolationLevel).ConfigureAwait(false);
        //        var value = await function.Invoke().ConfigureAwait(false);
        //        await CommitTransactionAsync().ConfigureAwait(false);
        //        return value;
        //    }
        //    catch (Exception ex)
        //    {
        //        await RollbackTransactionAsync().ConfigureAwait(false);
        //        if (onException == null)
        //            throw;
        //        return await onException.Invoke(ex).ConfigureAwait(false);
        //    }
        //}

        //public void BeginTransaction(IsolationLevel isolationLevel = IsolationLevel.Serializable)
        //{
        //    try
        //    {
        //        semaphore.Wait();
        //        if (transaction != null)
        //            throw new InvalidOperationException($"This {nameof(DatabaseContext)} instance already has an active transaction associated with it.");
        //        connection = new SqlConnection(connectionString);
        //        connection.Open();
        //        transaction = connection.BeginTransaction(isolationLevel);
        //    }
        //    finally
        //    {
        //        semaphore.Release();
        //    }
        //}

        //public void CommitTransaction()
        //{
        //    try
        //    {
        //        semaphore.Wait();
        //        if (transaction == null)
        //            throw new InvalidOperationException($"This {nameof(DatabaseContext)} instance does not have an active transaction associated with it.");
        //        transaction.Commit();
        //    }
        //    finally
        //    {
        //        if (transaction != null)
        //        {
        //            transaction.Dispose();
        //            transaction = null;
        //        }
        //        if (connection != null)
        //        {
        //            connection.Dispose();
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        //public void RollbackTransaction()
        //{
        //    try
        //    {
        //        semaphore.Wait();
        //        if (transaction == null)
        //            throw new InvalidOperationException($"This {nameof(DatabaseContext)} instance does not have an active transaction associated with it.");
        //        transaction.Rollback();
        //    }
        //    finally
        //    {
        //        if (transaction != null)
        //        {
        //            transaction.Dispose();
        //            transaction = null;
        //        }
        //        if (connection != null)
        //        {
        //            connection.Dispose();
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        //public async Task BeginTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.Serializable, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        //        if (transaction != null)
        //            throw new InvalidOperationException($"This {nameof(DatabaseContext)} instance already has an active transaction associated with it.");
        //        connection = new SqlConnection(connectionString);
        //        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        //        transaction = await connection.BeginTransactionAsync(isolationLevel, cancellationToken).ConfigureAwait(false);
        //    }
        //    finally
        //    {
        //        semaphore.Release();
        //    }
        //}

        //public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        //        if (transaction == null)
        //            throw new InvalidOperationException($"This {nameof(DatabaseContext)} instance does not have an active transaction associated with it.");
        //        await transaction.CommitAsync(cancellationToken).ConfigureAwait(false);
        //    }
        //    finally
        //    {
        //        if (transaction != null)
        //        {
        //            await transaction.DisposeAsync().ConfigureAwait(false);
        //            transaction = null;
        //        }
        //        if (connection != null)
        //        {
        //            await connection.DisposeAsync().ConfigureAwait(false);
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        //public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        //        if (transaction == null)
        //            throw new InvalidOperationException($"This {nameof(DatabaseContext)} instance does not have an active transaction associated with it.");
        //        await transaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
        //    }
        //    finally
        //    {
        //        if (transaction != null)
        //        {
        //            await transaction.DisposeAsync().ConfigureAwait(false);
        //            transaction = null;
        //        }
        //        if (connection != null)
        //        {
        //            await connection.DisposeAsync().ConfigureAwait(false);
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        //public void Execute(Action<DbCommand> operation)
        //{
        //    try
        //    {
        //        semaphore.Wait();
        //        if (connection == null)
        //        {
        //            connection = new SqlConnection(connectionString);
        //            connection.Open();
        //        }
        //        using var dbCommand = connection.CreateCommand();
        //        if (transaction != null)
        //            dbCommand.Transaction = transaction;
        //        operation.Invoke(dbCommand);
        //    }
        //    finally
        //    {
        //        if (transaction == null && connection != null)
        //        {
        //            connection.Close();
        //            connection.Dispose();
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        //public TResult Execute<TResult>(Func<DbCommand, TResult> operation)
        //{
        //    try
        //    {
        //        semaphore.Wait();
        //        if (connection == null)
        //        {
        //            connection = new SqlConnection(connectionString);
        //            connection.Open();
        //        }
        //        using var dbCommand = connection.CreateCommand();
        //        if (transaction != null)
        //            dbCommand.Transaction = transaction;
        //        return operation.Invoke(dbCommand);
        //    }
        //    finally
        //    {
        //        if (transaction == null && connection != null)
        //        {
        //            connection.Close();
        //            connection.Dispose();
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        //public IEnumerable<TResult> Enumerate<TResult>(Func<DbCommand, IEnumerable<TResult>> operation)
        //{
        //    try
        //    {
        //        semaphore.Wait();
        //        if (connection == null)
        //        {
        //            connection = new SqlConnection(connectionString);
        //            connection.Open();
        //        }
        //        using var dbCommand = connection.CreateCommand();
        //        if (transaction != null)
        //            dbCommand.Transaction = transaction;
        //        foreach (var result in operation.Invoke(dbCommand))
        //            yield return result;
        //    }
        //    finally
        //    {
        //        if (transaction == null && connection != null)
        //        {
        //            connection.Close();
        //            connection.Dispose();
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        //public async Task ExecuteAsync(Func<DbCommand, CancellationToken, Task> operation, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        //        if (connection == null)
        //        {
        //            connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        //        }
        //        await using var dbCommand = connection.CreateCommand();
        //        if (transaction != null)
        //            dbCommand.Transaction = transaction;
        //        await operation.Invoke(dbCommand, cancellationToken).ConfigureAwait(false);
        //    }
        //    finally
        //    {
        //        if (transaction == null && connection != null)
        //        {
        //            await connection.CloseAsync().ConfigureAwait(false);
        //            await connection.DisposeAsync().ConfigureAwait(false);
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        //public async Task<TResult> ExecuteAsync<TResult>(Func<DbCommand, CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        //        if (connection == null)
        //        {
        //            connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        //        }
        //        await using var dbCommand = connection.CreateCommand();
        //        if (transaction != null)
        //            dbCommand.Transaction = transaction;
        //        return await operation.Invoke(dbCommand, cancellationToken).ConfigureAwait(false);
        //    }
        //    finally
        //    {
        //        if (transaction == null && connection != null)
        //        {
        //            await connection.CloseAsync().ConfigureAwait(false);
        //            await connection.DisposeAsync().ConfigureAwait(false);
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        //public async IAsyncEnumerable<TResult> StreamAsync<TResult>(Func<DbCommand, CancellationToken, IAsyncEnumerable<TResult>> operation, [EnumeratorCancellation] CancellationToken cancellationToken = default)
        //{
        //    try
        //    {
        //        await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        //        if (connection == null)
        //        {
        //            connection = new SqlConnection(connectionString);
        //            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        //        }
        //        await using var dbCommand = connection.CreateCommand();
        //        if (transaction != null)
        //            dbCommand.Transaction = transaction;
        //        await foreach (var result in operation.Invoke(dbCommand, cancellationToken).ConfigureAwait(false))
        //            yield return result;
        //    }
        //    finally
        //    {
        //        if (transaction == null && connection != null)
        //        {
        //            await connection.CloseAsync().ConfigureAwait(false);
        //            await connection.DisposeAsync().ConfigureAwait(false);
        //            connection = null;
        //        }
        //        semaphore.Release();
        //    }
        //}

        #endregion

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
            if (disposing) { }
            isDisposed = true;
        }

        protected virtual Task DisposeAsync(bool disposing)
        {
            if (isDisposed)
                return Task.CompletedTask;
            if (disposing) { }
            isDisposed = true;
            return Task.CompletedTask;
        }

        #region ObsoleteDispose

        //protected virtual void Dispose(bool disposing)
        //{
        //    if (isDisposed)
        //        return;
        //    if (disposing)
        //    {
        //        if (transaction != null)
        //        {
        //            transaction.Rollback();
        //            transaction.Dispose();
        //        }
        //        if (connection != null)
        //            connection.Dispose();
        //    }
        //    isDisposed = true;
        //}

        //protected async virtual Task DisposeAsync(bool disposing)
        //{
        //    if (isDisposed)
        //        return;
        //    if (disposing)
        //    {
        //        if (transaction != null)
        //        {
        //            await transaction.RollbackAsync().ConfigureAwait(false);
        //            await transaction.DisposeAsync().ConfigureAwait(false);
        //        }
        //        if (connection != null)
        //            await connection.DisposeAsync().ConfigureAwait(false);
        //    }
        //    isDisposed = true;
        //}

        #endregion
    }
}
