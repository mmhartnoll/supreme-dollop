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
    public class ProfileHasBoostersTable : DatabaseTable<ProfileHasBoostersRecord, ProfileHasBoostersRecordExpression>
    {
        private ProfileHasBoostersTable(DatabaseContext databaseContext) : base(databaseContext, "Mtga", "ProfileHasBoosters")
        {
        }

        internal static ProfileHasBoostersTable Create(DatabaseContext databaseContext)
        {
            return new ProfileHasBoostersTable(databaseContext);
        }

        public ProfileHasBoostersRecord NewRecord(Guid profileId, Guid boosterId, int count)
        {
            return DatabaseContext.Execute(command => NewRecord(command, profileId, boosterId, count));
        }

        public async Task<ProfileHasBoostersRecord> NewRecordAsync(Guid profileId, Guid boosterId, int count, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, profileId, boosterId, count, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public ProfileHasBoostersRecord NewRecord(ProfileRecord profileRecord, BoosterRecord boosterRecord, int count)
        {
            return DatabaseContext.Execute(command => NewRecord(command, profileRecord.Id, boosterRecord.SetId, count));
        }

        public async Task<ProfileHasBoostersRecord> NewRecordAsync(ProfileRecord profileRecord, BoosterRecord boosterRecord, int count, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, profileRecord.Id, boosterRecord.SetId, count, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private ProfileHasBoostersRecord NewRecord(DbCommand command, Guid profileId, Guid boosterId, int count)
        {
            var newRecord = ProfileHasBoostersRecord.Create(DatabaseContext, this, profileId, boosterId, count);
            command.CommandText = "INSERT INTO [Mtga].[ProfileHasBoosters] ( ProfileId, BoosterId, Count ) VALUES ( @ProfileId, @BoosterId, @Count );";
            command.AddParameter("ProfileId", newRecord.ProfileId);
            command.AddParameter("BoosterId", newRecord.BoosterId);
            command.AddParameter("Count", newRecord.Count);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<ProfileHasBoostersRecord> NewRecordAsync(DbCommand command, Guid profileId, Guid boosterId, int count, CancellationToken cancellationToken)
        {
            var newRecord = ProfileHasBoostersRecord.Create(DatabaseContext, this, profileId, boosterId, count);
            command.CommandText = "INSERT INTO [Mtga].[ProfileHasBoosters] ( ProfileId, BoosterId, Count ) VALUES ( @ProfileId, @BoosterId, @Count );";
            command.AddParameter("ProfileId", newRecord.ProfileId);
            command.AddParameter("BoosterId", newRecord.BoosterId);
            command.AddParameter("Count", newRecord.Count);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override ProfileHasBoostersRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var profileId = (Guid)dbDataReader["ProfileId"];
            var boosterId = (Guid)dbDataReader["BoosterId"];
            var count = Convert.ToInt32(dbDataReader["Count"]);
            return ProfileHasBoostersRecord.Create(DatabaseContext, this, profileId, boosterId, count);
        }
    }
}