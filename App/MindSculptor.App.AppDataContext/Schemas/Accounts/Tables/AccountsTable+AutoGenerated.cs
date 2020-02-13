using MindSculptor.App.AppDataContext.Schemas.Accounts.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Accounts.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Accounts.Tables
{
    public class AccountsTable : DataContextTable<AccountRecord, AccountRecordExpression>
    {
        private AccountsTable(DataContext dataContext) : base(dataContext, "Accounts", "Accounts")
        {
        }

        internal static AccountsTable Create(DataContext dataContext)
        {
            return new AccountsTable(dataContext);
        }

        public AccountRecord NewRecord(string emailAddress)
        {
            var newRecord = AccountRecord.Create(DataContext, Guid.NewGuid(), emailAddress);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Accounts].[Accounts] ( Id, EmailAddress ) VALUES ( @Id, @EmailAddress );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("EmailAddress", newRecord.EmailAddress);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<AccountRecord> NewRecordAsync(string emailAddress)
        {
            var newRecord = AccountRecord.Create(DataContext, Guid.NewGuid(), emailAddress);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Accounts].[Accounts] ( Id, EmailAddress ) VALUES ( @Id, @EmailAddress );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("EmailAddress", newRecord.EmailAddress);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override AccountRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var emailAddress = (string)dbDataReader["EmailAddress"];
            return AccountRecord.Create(DataContext, id, emailAddress);
        }
    }
}