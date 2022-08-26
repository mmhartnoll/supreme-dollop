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
    public class ActiveEventEntriesTable : DatabaseTable<ActiveEventEntryRecord, ActiveEventEntryRecordExpression>
    {
        private ActiveEventEntriesTable(DatabaseContext dataContext) : base(dataContext, "Mtga", "ActiveEventEntries")
        {
        }

        internal static ActiveEventEntriesTable Create(DatabaseContext dataContext)
        {
            return new ActiveEventEntriesTable(dataContext);
        }

        public ActiveEventEntryRecord NewRecord(Guid eventEntryId, Guid eventId, Guid profileId)
        {
            return Context.Execute(command => NewRecord(command, eventEntryId, eventId, profileId));
        }

        public async Task<ActiveEventEntryRecord> NewRecordAsync(Guid eventEntryId, Guid eventId, Guid profileId, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, eventEntryId, eventId, profileId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public ActiveEventEntryRecord NewRecord(EventEntryRecord eventEntryRecord)
        {
            return Context.Execute(command => NewRecord(command, eventEntryRecord.Id, eventEntryRecord.EventId, eventEntryRecord.ProfileId));
        }

        public async Task<ActiveEventEntryRecord> NewRecordAsync(EventEntryRecord eventEntryRecord, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, eventEntryRecord.Id, eventEntryRecord.EventId, eventEntryRecord.ProfileId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private ActiveEventEntryRecord NewRecord(DbCommand command, Guid eventEntryId, Guid eventId, Guid profileId)
        {
            var newRecord = ActiveEventEntryRecord.Create(Context, this, eventEntryId, eventId, profileId);
            command.CommandText = "INSERT INTO [Mtga].[ActiveEventEntries] ( EventEntryId, EventId, ProfileId ) VALUES ( @EventEntryId, @EventId, @ProfileId );";
            command.AddParameter("EventEntryId", newRecord.EventEntryId);
            command.AddParameter("EventId", newRecord.EventId);
            command.AddParameter("ProfileId", newRecord.ProfileId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<ActiveEventEntryRecord> NewRecordAsync(DbCommand command, Guid eventEntryId, Guid eventId, Guid profileId, CancellationToken cancellationToken)
        {
            var newRecord = ActiveEventEntryRecord.Create(Context, this, eventEntryId, eventId, profileId);
            command.CommandText = "INSERT INTO [Mtga].[ActiveEventEntries] ( EventEntryId, EventId, ProfileId ) VALUES ( @EventEntryId, @EventId, @ProfileId );";
            command.AddParameter("EventEntryId", newRecord.EventEntryId);
            command.AddParameter("EventId", newRecord.EventId);
            command.AddParameter("ProfileId", newRecord.ProfileId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override ActiveEventEntryRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var eventEntryId = (Guid)dbDataReader["EventEntryId"];
            var eventId = (Guid)dbDataReader["EventId"];
            var profileId = (Guid)dbDataReader["ProfileId"];
            return ActiveEventEntryRecord.Create(Context, this, eventEntryId, eventId, profileId);
        }
    }
}