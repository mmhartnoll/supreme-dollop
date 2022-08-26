using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class ActiveEventEntryRecord : DatabaseRecord<ActiveEventEntryRecord>
    {
        public Guid EventEntryId { get; }
        public Guid EventId { get; }
        public Guid ProfileId { get; }

        private ActiveEventEntryRecord(DatabaseContext dataContext, ActiveEventEntriesTable activeEventEntriesTable, Guid eventEntryId, Guid eventId, Guid profileId) : base(dataContext, activeEventEntriesTable)
        {
            EventEntryId = eventEntryId;
            EventId = eventId;
            ProfileId = profileId;
        }

        internal static ActiveEventEntryRecord Create(DatabaseContext dataContext, ActiveEventEntriesTable activeEventEntriesTable, Guid eventEntryId, Guid eventId, Guid profileId)
        {
            return new ActiveEventEntryRecord(dataContext, activeEventEntriesTable, eventEntryId, eventId, profileId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[ActiveEventEntries] SET  WHERE EventEntryId = @EventEntryId AND EventId = @EventId AND ProfileId = @ProfileId;";
                command.AddParameter("EventEntryId", EventEntryId);
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
                command.CommandText = "UPDATE [Mtga].[ActiveEventEntries] SET  WHERE EventEntryId = @EventEntryId AND EventId = @EventId AND ProfileId = @ProfileId;";
                command.AddParameter("EventEntryId", EventEntryId);
                command.AddParameter("EventId", EventId);
                command.AddParameter("ProfileId", ProfileId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[ActiveEventEntries] WHERE EventEntryId = @EventEntryId AND EventId = @EventId AND ProfileId = @ProfileId;";
            command.AddParameter("EventEntryId", EventEntryId);
            command.AddParameter("EventId", EventId);
            command.AddParameter("ProfileId", ProfileId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[ActiveEventEntries] WHERE EventEntryId = @EventEntryId AND EventId = @EventId AND ProfileId = @ProfileId;";
            command.AddParameter("EventEntryId", EventEntryId);
            command.AddParameter("EventId", EventId);
            command.AddParameter("ProfileId", ProfileId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}