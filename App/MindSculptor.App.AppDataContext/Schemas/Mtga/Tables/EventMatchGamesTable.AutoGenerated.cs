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
    public class EventMatchGamesTable : DatabaseTable<EventMatchGameRecord, EventMatchGameRecordExpression>
    {
        private EventMatchGamesTable(DatabaseContext databaseContext) : base(databaseContext, "Mtga", "EventMatchGames")
        {
        }

        internal static EventMatchGamesTable Create(DatabaseContext databaseContext)
        {
            return new EventMatchGamesTable(databaseContext);
        }

        public EventMatchGameRecord NewRecord(Guid eventMatchId, int gameNumber, bool playedFirst, bool gameWon)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), eventMatchId, gameNumber, playedFirst, gameWon));
        }

        public async Task<EventMatchGameRecord> NewRecordAsync(Guid eventMatchId, int gameNumber, bool playedFirst, bool gameWon, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), eventMatchId, gameNumber, playedFirst, gameWon, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public EventMatchGameRecord NewRecord(EventMatchRecord eventMatchRecord, int gameNumber, bool playedFirst, bool gameWon)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), eventMatchRecord.Id, gameNumber, playedFirst, gameWon));
        }

        public async Task<EventMatchGameRecord> NewRecordAsync(EventMatchRecord eventMatchRecord, int gameNumber, bool playedFirst, bool gameWon, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), eventMatchRecord.Id, gameNumber, playedFirst, gameWon, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private EventMatchGameRecord NewRecord(DbCommand command, Guid id, Guid eventMatchId, int gameNumber, bool playedFirst, bool gameWon)
        {
            var newRecord = EventMatchGameRecord.Create(DatabaseContext, this, id, eventMatchId, gameNumber, playedFirst, gameWon);
            command.CommandText = "INSERT INTO [Mtga].[EventMatchGames] ( Id, EventMatchId, GameNumber, PlayedFirst, GameWon ) VALUES ( @Id, @EventMatchId, @GameNumber, @PlayedFirst, @GameWon );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventMatchId", newRecord.EventMatchId);
            command.AddParameter("GameNumber", newRecord.GameNumber);
            command.AddParameter("PlayedFirst", newRecord.PlayedFirst);
            command.AddParameter("GameWon", newRecord.GameWon);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<EventMatchGameRecord> NewRecordAsync(DbCommand command, Guid id, Guid eventMatchId, int gameNumber, bool playedFirst, bool gameWon, CancellationToken cancellationToken)
        {
            var newRecord = EventMatchGameRecord.Create(DatabaseContext, this, id, eventMatchId, gameNumber, playedFirst, gameWon);
            command.CommandText = "INSERT INTO [Mtga].[EventMatchGames] ( Id, EventMatchId, GameNumber, PlayedFirst, GameWon ) VALUES ( @Id, @EventMatchId, @GameNumber, @PlayedFirst, @GameWon );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventMatchId", newRecord.EventMatchId);
            command.AddParameter("GameNumber", newRecord.GameNumber);
            command.AddParameter("PlayedFirst", newRecord.PlayedFirst);
            command.AddParameter("GameWon", newRecord.GameWon);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override EventMatchGameRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var eventMatchId = (Guid)dbDataReader["EventMatchId"];
            var gameNumber = Convert.ToInt32(dbDataReader["GameNumber"]);
            var playedFirst = (bool)dbDataReader["PlayedFirst"];
            var gameWon = (bool)dbDataReader["GameWon"];
            return EventMatchGameRecord.Create(DatabaseContext, this, id, eventMatchId, gameNumber, playedFirst, gameWon);
        }
    }
}