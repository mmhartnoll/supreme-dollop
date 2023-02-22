using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class EventEntryRecord : DatabaseRecord<EventEntryRecord>
    {
        public Guid Id { get; }
        public Guid EventId { get; }
        public Guid ProfileId { get; }

        private EventEntryRecord(DatabaseContext databaseContext, EventEntriesTable eventEntriesTable, Guid id, Guid eventId, Guid profileId) : base(databaseContext, eventEntriesTable)
        {
            Id = id;
            EventId = eventId;
            ProfileId = profileId;
        }

        internal static EventEntryRecord Create(DatabaseContext databaseContext, EventEntriesTable eventEntriesTable, Guid id, Guid eventId, Guid profileId)
        {
            return new EventEntryRecord(databaseContext, eventEntriesTable, id, eventId, profileId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[EventEntries] SET EventId = @EventId, ProfileId = @ProfileId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventId", EventId);
                command.AddParameter("ProfileId", ProfileId);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[EventEntries] SET EventId = @EventId, ProfileId = @ProfileId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventId", EventId);
                command.AddParameter("ProfileId", ProfileId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[EventEntries] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[EventEntries] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}