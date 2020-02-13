using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.Modules
{
    internal abstract class ProcessingModule
    {
        protected static async Task ProcessDataAsync(Func<DataContext, Task> task)
        {
            using (var dbConnection = new SqlConnection(DBConnectionString))
                try
                {
                    await dbConnection.OpenAsync()
                        .ConfigureAwait(false);
                    var dataContext = DataContext.Create(dbConnection);
                    await task(dataContext)
                        .ConfigureAwait(false);
                }
                finally
                {
                    await dbConnection.CloseAsync()
                        .ConfigureAwait(false);
                }
        }

        protected static async Task<TResult> ProcessDataAsync<TResult>(Func<DataContext, Task<TResult>> task)
        {
            using (var dbConnection = new SqlConnection(DBConnectionString))
                try
                {
                    await dbConnection.OpenAsync()
                        .ConfigureAwait(false);
                    var dataContext = DataContext.Create(dbConnection);
                    return await task(dataContext)
                        .ConfigureAwait(false);
                }
                finally
                {
                    await dbConnection.CloseAsync()
                        .ConfigureAwait(false);
                }
        }

        protected static async IAsyncEnumerable<TResult> StreamDataAsync<TResult>(Func<DataContext, IAsyncEnumerable<TResult>> task)
        {
            using (var dbConnection = new SqlConnection(DBConnectionString))
                try
                {
                    await dbConnection.OpenAsync()
                        .ConfigureAwait(false);
                    var dataContext = DataContext.Create(dbConnection);
                    await foreach (var item in task(dataContext).ConfigureAwait(false))
                        yield return item;
                }
                finally
                {
                    await dbConnection.CloseAsync()
                        .ConfigureAwait(false);
                }
        }

        protected static async Task ProcessDataWithTransactionAsync(Func<DataContext, Task> task, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            using (var dbConnection = new SqlConnection(DBConnectionString))
                try
                {
                    await dbConnection.OpenAsync()
                        .ConfigureAwait(false);
                    using (var dbTransaction = await dbConnection.BeginTransactionAsync(isolationLevel).ConfigureAwait(false))
                        try
                        {
                            var dataContext = DataContext.Create(dbConnection, dbTransaction);
                            await task(dataContext)
                                .ConfigureAwait(false);
                            await dbTransaction.CommitAsync()
                                .ConfigureAwait(false);
                        }
                        catch
                        {
                            await dbTransaction.RollbackAsync()
                                .ConfigureAwait(false);
                            throw;
                        }
                }
                finally
                {
                    await dbConnection.CloseAsync()
                        .ConfigureAwait(false);
                }
        }

        protected static async Task<TResult> ProcessDataWithTransactionAsync<TResult>(Func<DataContext, Task<TResult>> task, IsolationLevel isolationLevel = IsolationLevel.Serializable)
        {
            using (var dbConnection = new SqlConnection(DBConnectionString))
                try
                {
                    await dbConnection.OpenAsync()
                        .ConfigureAwait(false);
                    using (var dbTransaction = await dbConnection.BeginTransactionAsync(isolationLevel).ConfigureAwait(false))
                        try
                        {
                            var dataContext = DataContext.Create(dbConnection, dbTransaction);
                            var returnValue = await task(dataContext)
                                .ConfigureAwait(false);
                            await dbTransaction.CommitAsync()
                                .ConfigureAwait(false);
                            return returnValue;
                        }
                        catch
                        {
                            await dbTransaction.RollbackAsync()
                                .ConfigureAwait(false);
                            throw;
                        }
                }
                finally
                {
                    await dbConnection.CloseAsync()
                        .ConfigureAwait(false);
                }
        }

        protected static async Task<ProfileRecord> GetProfileRecordAsync(DataContext dataContext)
        {
            var profileResult = await dataContext.Mtga.Profiles
                .QueryWhere(profile => profile.ScreenName == "MurderousRedcap" && profile.UserId == 12207)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (!profileResult.Success)
                throw new Exception("Failed to find a valid profile.");
            return profileResult.Value;
        }

        private const string DBConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=MindSculptorApp002;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
    }
}
