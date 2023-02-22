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
    public class ProfileHasCardsTable : DatabaseTable<ProfileHasCardRecord, ProfileHasCardRecordExpression>
    {
        private ProfileHasCardsTable(DatabaseContext databaseContext) : base(databaseContext, "Mtga", "ProfileHasCards")
        {
        }

        internal static ProfileHasCardsTable Create(DatabaseContext databaseContext)
        {
            return new ProfileHasCardsTable(databaseContext);
        }

        public ProfileHasCardRecord NewRecord(Guid profileId, Guid digitalCardId, int count)
        {
            return DatabaseContext.Execute(command => NewRecord(command, profileId, digitalCardId, count));
        }

        public async Task<ProfileHasCardRecord> NewRecordAsync(Guid profileId, Guid digitalCardId, int count, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, profileId, digitalCardId, count, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public ProfileHasCardRecord NewRecord(ProfileRecord profileRecord, DigitalCardRecord digitalCardRecord, int count)
        {
            return DatabaseContext.Execute(command => NewRecord(command, profileRecord.Id, digitalCardRecord.Id, count));
        }

        public async Task<ProfileHasCardRecord> NewRecordAsync(ProfileRecord profileRecord, DigitalCardRecord digitalCardRecord, int count, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, profileRecord.Id, digitalCardRecord.Id, count, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private ProfileHasCardRecord NewRecord(DbCommand command, Guid profileId, Guid digitalCardId, int count)
        {
            var newRecord = ProfileHasCardRecord.Create(DatabaseContext, this, profileId, digitalCardId, count);
            command.CommandText = "INSERT INTO [Mtga].[ProfileHasCards] ( ProfileId, DigitalCardId, Count ) VALUES ( @ProfileId, @DigitalCardId, @Count );";
            command.AddParameter("ProfileId", newRecord.ProfileId);
            command.AddParameter("DigitalCardId", newRecord.DigitalCardId);
            command.AddParameter("Count", newRecord.Count);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<ProfileHasCardRecord> NewRecordAsync(DbCommand command, Guid profileId, Guid digitalCardId, int count, CancellationToken cancellationToken)
        {
            var newRecord = ProfileHasCardRecord.Create(DatabaseContext, this, profileId, digitalCardId, count);
            command.CommandText = "INSERT INTO [Mtga].[ProfileHasCards] ( ProfileId, DigitalCardId, Count ) VALUES ( @ProfileId, @DigitalCardId, @Count );";
            command.AddParameter("ProfileId", newRecord.ProfileId);
            command.AddParameter("DigitalCardId", newRecord.DigitalCardId);
            command.AddParameter("Count", newRecord.Count);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override ProfileHasCardRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var profileId = (Guid)dbDataReader["ProfileId"];
            var digitalCardId = (Guid)dbDataReader["DigitalCardId"];
            var count = Convert.ToInt32(dbDataReader["Count"]);
            return ProfileHasCardRecord.Create(DatabaseContext, this, profileId, digitalCardId, count);
        }
    }
}