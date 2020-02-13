using MindSculptor.App.AppDataContext.Schemas.Accounts.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class ProfilesTable : DataContextTable<ProfileRecord, ProfileRecordExpression>
    {
        private ProfilesTable(DataContext dataContext) : base(dataContext, "Mtga", "Profiles")
        {
        }

        internal static ProfilesTable Create(DataContext dataContext)
        {
            return new ProfilesTable(dataContext);
        }

        public ProfileRecord NewRecord(AccountRecord accountRecord, string screenName, int userId)
        {
            var newRecord = ProfileRecord.Create(DataContext, Guid.NewGuid(), accountRecord.Id, screenName, userId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[Profiles] ( Id, AccountId, ScreenName, UserId ) VALUES ( @Id, @AccountId, @ScreenName, @UserId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("AccountId", newRecord.AccountId);
                command.AddParameter("ScreenName", newRecord.ScreenName);
                command.AddParameter("UserId", newRecord.UserId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<ProfileRecord> NewRecordAsync(AccountRecord accountRecord, string screenName, int userId)
        {
            var newRecord = ProfileRecord.Create(DataContext, Guid.NewGuid(), accountRecord.Id, screenName, userId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[Profiles] ( Id, AccountId, ScreenName, UserId ) VALUES ( @Id, @AccountId, @ScreenName, @UserId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("AccountId", newRecord.AccountId);
                command.AddParameter("ScreenName", newRecord.ScreenName);
                command.AddParameter("UserId", newRecord.UserId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override ProfileRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var accountId = (Guid)dbDataReader["AccountId"];
            var screenName = (string)dbDataReader["ScreenName"];
            var userId = Convert.ToInt32(dbDataReader["UserId"]);
            return ProfileRecord.Create(DataContext, id, accountId, screenName, userId);
        }
    }
}