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
    public class EventEntriesTable : DatabaseTable<EventEntryRecord, EventEntryRecordExpression>
    {
        private EventEntriesTable(DatabaseContext databaseContext) : base(databaseContext, "Mtga", "EventEntries")
        {
        }

        internal static EventEntriesTable Create(DatabaseContext databaseContext)
        {
            return new EventEntriesTable(databaseContext);
        }

        public EventEntryRecord NewRecord(Guid id, Guid eventId, Guid profileId)
        {
            return DatabaseContext.Execute(command => NewRecord(command, id, eventId, profileId));
        }

        public async Task<EventEntryRecord> NewRecordAsync(Guid id, Guid eventId, Guid profileId, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, id, eventId, profileId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public EventEntryRecord NewRecord(EventRecord eventRecord, ProfileRecord profileRecord, Guid id)
        {
            return DatabaseContext.Execute(command => NewRecord(command, id, eventRecord.Id, profileRecord.Id));
        }

        public async Task<EventEntryRecord> NewRecordAsync(EventRecord eventRecord, ProfileRecord profileRecord, Guid id, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, id, eventRecord.Id, profileRecord.Id, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private EventEntryRecord NewRecord(DbCommand command, Guid id, Guid eventId, Guid profileId)
        {
            var newRecord = EventEntryRecord.Create(DatabaseContext, this, id, eventId, profileId);
            command.CommandText = "INSERT INTO [Mtga].[EventEntries] ( Id, EventId, ProfileId ) VALUES ( @Id, @EventId, @ProfileId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventId", newRecord.EventId);
            command.AddParameter("ProfileId", newRecord.ProfileId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<EventEntryRecord> NewRecordAsync(DbCommand command, Guid id, Guid eventId, Guid profileId, CancellationToken cancellationToken)
        {
            var newRecord = EventEntryRecord.Create(DatabaseContext, this, id, eventId, profileId);
            command.CommandText = "INSERT INTO [Mtga].[EventEntries] ( Id, EventId, ProfileId ) VALUES ( @Id, @EventId, @ProfileId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventId", newRecord.EventId);
            command.AddParameter("ProfileId", newRecord.ProfileId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override EventEntryRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var eventId = (Guid)dbDataReader["EventId"];
            var profileId = (Guid)dbDataReader["ProfileId"];
            return EventEntryRecord.Create(DatabaseContext, this, id, eventId, profileId);
        }
    }
}