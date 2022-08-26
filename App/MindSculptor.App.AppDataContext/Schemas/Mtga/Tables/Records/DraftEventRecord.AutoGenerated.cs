using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DraftEventRecord : DatabaseRecord<DraftEventRecord>
    {
        public Guid EventId { get; }
        public Guid SetId { get; }
        public string DraftType { get; }

        private DraftEventRecord(DatabaseContext dataContext, DraftEventsTable draftEventsTable, Guid eventId, Guid setId, string draftType) : base(dataContext, draftEventsTable)
        {
            EventId = eventId;
            SetId = setId;
            DraftType = draftType;
        }

        internal static DraftEventRecord Create(DatabaseContext dataContext, DraftEventsTable draftEventsTable, Guid eventId, Guid setId, string draftType)
        {
            return new DraftEventRecord(dataContext, draftEventsTable, eventId, setId, draftType);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[DraftEvents] SET SetId = @SetId, DraftType = @DraftType WHERE EventId = @EventId;";
                command.AddParameter("EventId", EventId);
                command.AddParameter("SetId", SetId);
                command.AddParameter("DraftType", DraftType);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[DraftEvents] SET SetId = @SetId, DraftType = @DraftType WHERE EventId = @EventId;";
                command.AddParameter("EventId", EventId);
                command.AddParameter("SetId", SetId);
                command.AddParameter("DraftType", DraftType);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[DraftEvents] WHERE EventId = @EventId;";
            command.AddParameter("EventId", EventId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[DraftEvents] WHERE EventId = @EventId;";
            command.AddParameter("EventId", EventId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}