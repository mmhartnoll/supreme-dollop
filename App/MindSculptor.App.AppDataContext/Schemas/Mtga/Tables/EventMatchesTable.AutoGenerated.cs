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
    public class EventMatchesTable : DatabaseTable<EventMatchRecord, EventMatchRecordExpression>
    {
        private EventMatchesTable(DatabaseContext databaseContext) : base(databaseContext, "Mtga", "EventMatches")
        {
        }

        internal static EventMatchesTable Create(DatabaseContext databaseContext)
        {
            return new EventMatchesTable(databaseContext);
        }

        public EventMatchRecord NewRecord(Guid id, Guid eventEntryId, Guid opponentId)
        {
            return DatabaseContext.Execute(command => NewRecord(command, id, eventEntryId, opponentId));
        }

        public async Task<EventMatchRecord> NewRecordAsync(Guid id, Guid eventEntryId, Guid opponentId, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, id, eventEntryId, opponentId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public EventMatchRecord NewRecord(EventEntryRecord eventEntryRecord, PlayerRecord playerRecord, Guid id)
        {
            return DatabaseContext.Execute(command => NewRecord(command, id, eventEntryRecord.Id, playerRecord.Id));
        }

        public async Task<EventMatchRecord> NewRecordAsync(EventEntryRecord eventEntryRecord, PlayerRecord playerRecord, Guid id, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, id, eventEntryRecord.Id, playerRecord.Id, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private EventMatchRecord NewRecord(DbCommand command, Guid id, Guid eventEntryId, Guid opponentId)
        {
            var newRecord = EventMatchRecord.Create(DatabaseContext, this, id, eventEntryId, opponentId);
            command.CommandText = "INSERT INTO [Mtga].[EventMatches] ( Id, EventEntryId, OpponentId ) VALUES ( @Id, @EventEntryId, @OpponentId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventEntryId", newRecord.EventEntryId);
            command.AddParameter("OpponentId", newRecord.OpponentId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<EventMatchRecord> NewRecordAsync(DbCommand command, Guid id, Guid eventEntryId, Guid opponentId, CancellationToken cancellationToken)
        {
            var newRecord = EventMatchRecord.Create(DatabaseContext, this, id, eventEntryId, opponentId);
            command.CommandText = "INSERT INTO [Mtga].[EventMatches] ( Id, EventEntryId, OpponentId ) VALUES ( @Id, @EventEntryId, @OpponentId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventEntryId", newRecord.EventEntryId);
            command.AddParameter("OpponentId", newRecord.OpponentId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override EventMatchRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var eventEntryId = (Guid)dbDataReader["EventEntryId"];
            var opponentId = (Guid)dbDataReader["OpponentId"];
            return EventMatchRecord.Create(DatabaseContext, this, id, eventEntryId, opponentId);
        }
    }
}