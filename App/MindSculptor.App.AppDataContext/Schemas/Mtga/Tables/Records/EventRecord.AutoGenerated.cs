using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class EventRecord : DatabaseRecord<EventRecord>
    {
        public Guid Id { get; }
        public string MtgaEventId { get; }

        private EventRecord(DatabaseContext dataContext, EventsTable eventsTable, Guid id, string mtgaEventId) : base(dataContext, eventsTable)
        {
            Id = id;
            MtgaEventId = mtgaEventId;
        }

        internal static EventRecord Create(DatabaseContext dataContext, EventsTable eventsTable, Guid id, string mtgaEventId)
        {
            return new EventRecord(dataContext, eventsTable, id, mtgaEventId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[Events] SET MtgaEventId = @MtgaEventId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("MtgaEventId", MtgaEventId);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[Events] SET MtgaEventId = @MtgaEventId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("MtgaEventId", MtgaEventId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[Events] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[Events] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}