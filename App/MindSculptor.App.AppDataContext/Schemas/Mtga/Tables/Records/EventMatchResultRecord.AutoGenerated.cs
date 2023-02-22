using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class EventMatchResultRecord : DatabaseRecord<EventMatchResultRecord>
    {
        public Guid EventMatchId { get; }
        public bool MatchWon { get; }

        private EventMatchResultRecord(DatabaseContext databaseContext, EventMatchResultsTable eventMatchResultsTable, Guid eventMatchId, bool matchWon) : base(databaseContext, eventMatchResultsTable)
        {
            EventMatchId = eventMatchId;
            MatchWon = matchWon;
        }

        internal static EventMatchResultRecord Create(DatabaseContext databaseContext, EventMatchResultsTable eventMatchResultsTable, Guid eventMatchId, bool matchWon)
        {
            return new EventMatchResultRecord(databaseContext, eventMatchResultsTable, eventMatchId, matchWon);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[EventMatchResults] SET MatchWon = @MatchWon WHERE EventMatchId = @EventMatchId;";
                command.AddParameter("EventMatchId", EventMatchId);
                command.AddParameter("MatchWon", MatchWon);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[EventMatchResults] SET MatchWon = @MatchWon WHERE EventMatchId = @EventMatchId;";
                command.AddParameter("EventMatchId", EventMatchId);
                command.AddParameter("MatchWon", MatchWon);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[EventMatchResults] WHERE EventMatchId = @EventMatchId;";
            command.AddParameter("EventMatchId", EventMatchId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[EventMatchResults] WHERE EventMatchId = @EventMatchId;";
            command.AddParameter("EventMatchId", EventMatchId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}