using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class ProfileInventoriesTable : DataContextTable<ProfileInventoryRecord, ProfileInventoryRecordExpression>
    {
        private ProfileInventoriesTable(DataContext dataContext) : base(dataContext, "Mtga", "ProfileInventories")
        {
        }

        internal static ProfileInventoriesTable Create(DataContext dataContext)
        {
            return new ProfileInventoriesTable(dataContext);
        }

        public ProfileInventoryRecord NewRecord(ProfileRecord profileRecord, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount)
        {
            var newRecord = ProfileInventoryRecord.Create(DataContext, profileRecord.Id, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[ProfileInventories] ( ProfileId, MythicRareWildcardCount, RareWildcardCount, UncommonWildcardCount, CommonWildcardCount, GoldCount, GemCount ) VALUES ( @ProfileId, @MythicRareWildcardCount, @RareWildcardCount, @UncommonWildcardCount, @CommonWildcardCount, @GoldCount, @GemCount );";
                command.AddParameter("ProfileId", newRecord.ProfileId);
                command.AddParameter("MythicRareWildcardCount", newRecord.MythicRareWildcardCount);
                command.AddParameter("RareWildcardCount", newRecord.RareWildcardCount);
                command.AddParameter("UncommonWildcardCount", newRecord.UncommonWildcardCount);
                command.AddParameter("CommonWildcardCount", newRecord.CommonWildcardCount);
                command.AddParameter("GoldCount", newRecord.GoldCount);
                command.AddParameter("GemCount", newRecord.GemCount);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<ProfileInventoryRecord> NewRecordAsync(ProfileRecord profileRecord, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount)
        {
            var newRecord = ProfileInventoryRecord.Create(DataContext, profileRecord.Id, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[ProfileInventories] ( ProfileId, MythicRareWildcardCount, RareWildcardCount, UncommonWildcardCount, CommonWildcardCount, GoldCount, GemCount ) VALUES ( @ProfileId, @MythicRareWildcardCount, @RareWildcardCount, @UncommonWildcardCount, @CommonWildcardCount, @GoldCount, @GemCount );";
                command.AddParameter("ProfileId", newRecord.ProfileId);
                command.AddParameter("MythicRareWildcardCount", newRecord.MythicRareWildcardCount);
                command.AddParameter("RareWildcardCount", newRecord.RareWildcardCount);
                command.AddParameter("UncommonWildcardCount", newRecord.UncommonWildcardCount);
                command.AddParameter("CommonWildcardCount", newRecord.CommonWildcardCount);
                command.AddParameter("GoldCount", newRecord.GoldCount);
                command.AddParameter("GemCount", newRecord.GemCount);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override ProfileInventoryRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var profileId = (Guid)dbDataReader["ProfileId"];
            var mythicRareWildcardCount = Convert.ToInt32(dbDataReader["MythicRareWildcardCount"]);
            var rareWildcardCount = Convert.ToInt32(dbDataReader["RareWildcardCount"]);
            var uncommonWildcardCount = Convert.ToInt32(dbDataReader["UncommonWildcardCount"]);
            var commonWildcardCount = Convert.ToInt32(dbDataReader["CommonWildcardCount"]);
            var goldCount = Convert.ToInt32(dbDataReader["GoldCount"]);
            var gemCount = Convert.ToInt32(dbDataReader["GemCount"]);
            return ProfileInventoryRecord.Create(DataContext, profileId, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount);
        }
    }
}