using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class EventMatchRecord : DatabaseRecord<EventMatchRecord>
    {
        public Guid Id { get; }
        public Guid EventEntryId { get; }
        public Guid OpponentId { get; }

        private EventMatchRecord(DatabaseContext dataContext, EventMatchesTable eventMatchesTable, Guid id, Guid eventEntryId, Guid opponentId) : base(dataContext, eventMatchesTable)
        {
            Id = id;
            EventEntryId = eventEntryId;
            OpponentId = opponentId;
        }

        internal static EventMatchRecord Create(DatabaseContext dataContext, EventMatchesTable eventMatchesTable, Guid id, Guid eventEntryId, Guid opponentId)
        {
            return new EventMatchRecord(dataContext, eventMatchesTable, id, eventEntryId, opponentId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[EventMatches] SET EventEntryId = @EventEntryId, OpponentId = @OpponentId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventEntryId", EventEntryId);
                command.AddParameter("OpponentId", OpponentId);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[EventMatches] SET EventEntryId = @EventEntryId, OpponentId = @OpponentId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventEntryId", EventEntryId);
                command.AddParameter("OpponentId", OpponentId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[EventMatches] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[EventMatches] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}