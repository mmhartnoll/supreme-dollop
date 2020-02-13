using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Accounts.Tables.Records
{
    public class AccountRecord : DataContextRecord
    {
        private string _emailAddress;

        public Guid Id { get; }

        public string EmailAddress
        {
            get => _emailAddress;
            set
            {
                IsModified |= _emailAddress != value;
                _emailAddress = value;
            }
        }

        private AccountRecord(DataContext dataContext, Guid id, string emailAddress) : base(dataContext)
        {
            Id = id;
            _emailAddress = emailAddress;
        }

        internal static AccountRecord Create(DataContext dataContext, Guid id, string emailAddress)
        {
            return new AccountRecord(dataContext, id, emailAddress);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Accounts].[Accounts] SET EmailAddress = @EmailAddress WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("EmailAddress", EmailAddress);
                    command.ExecuteNonQuery();
                }
        }

        public async override Task UpdateRecordAsync()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Accounts].[Accounts] SET EmailAddress = @EmailAddress WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("EmailAddress", EmailAddress);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Accounts].[Accounts] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Accounts].[Accounts] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}