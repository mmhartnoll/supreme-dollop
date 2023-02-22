using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class EventMatchGameRecord : DatabaseRecord<EventMatchGameRecord>
    {
        public Guid Id { get; }
        public Guid EventMatchId { get; }
        public int GameNumber { get; }
        public bool PlayedFirst { get; }
        public bool GameWon { get; }

        private EventMatchGameRecord(DatabaseContext databaseContext, EventMatchGamesTable eventMatchGamesTable, Guid id, Guid eventMatchId, int gameNumber, bool playedFirst, bool gameWon) : base(databaseContext, eventMatchGamesTable)
        {
            Id = id;
            EventMatchId = eventMatchId;
            GameNumber = gameNumber;
            PlayedFirst = playedFirst;
            GameWon = gameWon;
        }

        internal static EventMatchGameRecord Create(DatabaseContext databaseContext, EventMatchGamesTable eventMatchGamesTable, Guid id, Guid eventMatchId, int gameNumber, bool playedFirst, bool gameWon)
        {
            return new EventMatchGameRecord(databaseContext, eventMatchGamesTable, id, eventMatchId, gameNumber, playedFirst, gameWon);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[EventMatchGames] SET EventMatchId = @EventMatchId, GameNumber = @GameNumber, PlayedFirst = @PlayedFirst, GameWon = @GameWon WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventMatchId", EventMatchId);
                command.AddParameter("GameNumber", GameNumber);
                command.AddParameter("PlayedFirst", PlayedFirst);
                command.AddParameter("GameWon", GameWon);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[EventMatchGames] SET EventMatchId = @EventMatchId, GameNumber = @GameNumber, PlayedFirst = @PlayedFirst, GameWon = @GameWon WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventMatchId", EventMatchId);
                command.AddParameter("GameNumber", GameNumber);
                command.AddParameter("PlayedFirst", PlayedFirst);
                command.AddParameter("GameWon", GameWon);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[EventMatchGames] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[EventMatchGames] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}