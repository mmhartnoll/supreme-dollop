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
    public class EventMatchResultsTable : DatabaseTable<EventMatchResultRecord, EventMatchResultRecordExpression>
    {
        private EventMatchResultsTable(DatabaseContext databaseContext) : base(databaseContext, "Mtga", "EventMatchResults")
        {
        }

        internal static EventMatchResultsTable Create(DatabaseContext databaseContext)
        {
            return new EventMatchResultsTable(databaseContext);
        }

        public EventMatchResultRecord NewRecord(Guid eventMatchId, bool matchWon)
        {
            return DatabaseContext.Execute(command => NewRecord(command, eventMatchId, matchWon));
        }

        public async Task<EventMatchResultRecord> NewRecordAsync(Guid eventMatchId, bool matchWon, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, eventMatchId, matchWon, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public EventMatchResultRecord NewRecord(EventMatchRecord eventMatchRecord, bool matchWon)
        {
            return DatabaseContext.Execute(command => NewRecord(command, eventMatchRecord.Id, matchWon));
        }

        public async Task<EventMatchResultRecord> NewRecordAsync(EventMatchRecord eventMatchRecord, bool matchWon, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, eventMatchRecord.Id, matchWon, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private EventMatchResultRecord NewRecord(DbCommand command, Guid eventMatchId, bool matchWon)
        {
            var newRecord = EventMatchResultRecord.Create(DatabaseContext, this, eventMatchId, matchWon);
            command.CommandText = "INSERT INTO [Mtga].[EventMatchResults] ( EventMatchId, MatchWon ) VALUES ( @EventMatchId, @MatchWon );";
            command.AddParameter("EventMatchId", newRecord.EventMatchId);
            command.AddParameter("MatchWon", newRecord.MatchWon);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<EventMatchResultRecord> NewRecordAsync(DbCommand command, Guid eventMatchId, bool matchWon, CancellationToken cancellationToken)
        {
            var newRecord = EventMatchResultRecord.Create(DatabaseContext, this, eventMatchId, matchWon);
            command.CommandText = "INSERT INTO [Mtga].[EventMatchResults] ( EventMatchId, MatchWon ) VALUES ( @EventMatchId, @MatchWon );";
            command.AddParameter("EventMatchId", newRecord.EventMatchId);
            command.AddParameter("MatchWon", newRecord.MatchWon);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override EventMatchResultRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var eventMatchId = (Guid)dbDataReader["EventMatchId"];
            var matchWon = (bool)dbDataReader["MatchWon"];
            return EventMatchResultRecord.Create(DatabaseContext, this, eventMatchId, matchWon);
        }
    }
}