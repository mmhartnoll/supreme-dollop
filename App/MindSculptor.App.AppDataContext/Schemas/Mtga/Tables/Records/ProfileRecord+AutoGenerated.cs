using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class ProfileRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid AccountId { get; }
        public string ScreenName { get; }
        public int UserId { get; }

        private ProfileRecord(DataContext dataContext, Guid id, Guid accountId, string screenName, int userId) : base(dataContext)
        {
            Id = id;
            AccountId = accountId;
            ScreenName = screenName;
            UserId = userId;
        }

        internal static ProfileRecord Create(DataContext dataContext, Guid id, Guid accountId, string screenName, int userId)
        {
            return new ProfileRecord(dataContext, id, accountId, screenName, userId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[Profiles] SET AccountId = @AccountId, ScreenName = @ScreenName, UserId = @UserId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("AccountId", AccountId);
                    command.AddParameter("ScreenName", ScreenName);
                    command.AddParameter("UserId", UserId);
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
                    command.CommandText = "UPDATE [Mtga].[Profiles] SET AccountId = @AccountId, ScreenName = @ScreenName, UserId = @UserId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("AccountId", AccountId);
                    command.AddParameter("ScreenName", ScreenName);
                    command.AddParameter("UserId", UserId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[Profiles] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Mtga].[Profiles] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}