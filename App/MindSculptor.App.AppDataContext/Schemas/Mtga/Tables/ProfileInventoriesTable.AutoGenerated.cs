using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class ProfileInventoriesTable : DatabaseTable<ProfileInventoryRecord, ProfileInventoryRecordExpression>
    {
        private ProfileInventoriesTable(DatabaseContext dataContext) : base(dataContext, "Mtga", "ProfileInventories")
        {
        }

        internal static ProfileInventoriesTable Create(DatabaseContext dataContext)
        {
            return new ProfileInventoriesTable(dataContext);
        }

        public ProfileInventoryRecord NewRecord(Guid profileId, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount, decimal vaultProgress)
        {
            return Context.Execute(command => NewRecord(command, profileId, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount, vaultProgress));
        }

        public async Task<ProfileInventoryRecord> NewRecordAsync(Guid profileId, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount, decimal vaultProgress, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, profileId, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount, vaultProgress, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public ProfileInventoryRecord NewRecord(ProfileRecord profileRecord, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount, decimal vaultProgress)
        {
            return Context.Execute(command => NewRecord(command, profileRecord.Id, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount, vaultProgress));
        }

        public async Task<ProfileInventoryRecord> NewRecordAsync(ProfileRecord profileRecord, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount, decimal vaultProgress, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, profileRecord.Id, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount, vaultProgress, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private ProfileInventoryRecord NewRecord(DbCommand command, Guid profileId, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount, decimal vaultProgress)
        {
            var newRecord = ProfileInventoryRecord.Create(Context, this, profileId, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount, vaultProgress);
            command.CommandText = "INSERT INTO [Mtga].[ProfileInventories] ( ProfileId, MythicRareWildcardCount, RareWildcardCount, UncommonWildcardCount, CommonWildcardCount, GoldCount, GemCount, VaultProgress ) VALUES ( @ProfileId, @MythicRareWildcardCount, @RareWildcardCount, @UncommonWildcardCount, @CommonWildcardCount, @GoldCount, @GemCount, @VaultProgress );";
            command.AddParameter("ProfileId", newRecord.ProfileId);
            command.AddParameter("MythicRareWildcardCount", newRecord.MythicRareWildcardCount);
            command.AddParameter("RareWildcardCount", newRecord.RareWildcardCount);
            command.AddParameter("UncommonWildcardCount", newRecord.UncommonWildcardCount);
            command.AddParameter("CommonWildcardCount", newRecord.CommonWildcardCount);
            command.AddParameter("GoldCount", newRecord.GoldCount);
            command.AddParameter("GemCount", newRecord.GemCount);
            command.AddParameter("VaultProgress", newRecord.VaultProgress);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<ProfileInventoryRecord> NewRecordAsync(DbCommand command, Guid profileId, int mythicRareWildcardCount, int rareWildcardCount, int uncommonWildcardCount, int commonWildcardCount, int goldCount, int gemCount, decimal vaultProgress, CancellationToken cancellationToken)
        {
            var newRecord = ProfileInventoryRecord.Create(Context, this, profileId, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount, vaultProgress);
            command.CommandText = "INSERT INTO [Mtga].[ProfileInventories] ( ProfileId, MythicRareWildcardCount, RareWildcardCount, UncommonWildcardCount, CommonWildcardCount, GoldCount, GemCount, VaultProgress ) VALUES ( @ProfileId, @MythicRareWildcardCount, @RareWildcardCount, @UncommonWildcardCount, @CommonWildcardCount, @GoldCount, @GemCount, @VaultProgress );";
            command.AddParameter("ProfileId", newRecord.ProfileId);
            command.AddParameter("MythicRareWildcardCount", newRecord.MythicRareWildcardCount);
            command.AddParameter("RareWildcardCount", newRecord.RareWildcardCount);
            command.AddParameter("UncommonWildcardCount", newRecord.UncommonWildcardCount);
            command.AddParameter("CommonWildcardCount", newRecord.CommonWildcardCount);
            command.AddParameter("GoldCount", newRecord.GoldCount);
            command.AddParameter("GemCount", newRecord.GemCount);
            command.AddParameter("VaultProgress", newRecord.VaultProgress);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
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
            var vaultProgress = (decimal)dbDataReader["VaultProgress"];
            return ProfileInventoryRecord.Create(Context, this, profileId, mythicRareWildcardCount, rareWildcardCount, uncommonWildcardCount, commonWildcardCount, goldCount, gemCount, vaultProgress);
        }
    }
}