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
    public class EventsTable : DatabaseTable<EventRecord, EventRecordExpression>
    {
        private EventsTable(DatabaseContext dataContext) : base(dataContext, "Mtga", "Events")
        {
        }

        internal static EventsTable Create(DatabaseContext dataContext)
        {
            return new EventsTable(dataContext);
        }

        public EventRecord NewRecord(string mtgaEventId)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), mtgaEventId));
        }

        public async Task<EventRecord> NewRecordAsync(string mtgaEventId, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), mtgaEventId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private EventRecord NewRecord(DbCommand command, Guid id, string mtgaEventId)
        {
            var newRecord = EventRecord.Create(Context, this, id, mtgaEventId);
            command.CommandText = "INSERT INTO [Mtga].[Events] ( Id, MtgaEventId ) VALUES ( @Id, @MtgaEventId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("MtgaEventId", newRecord.MtgaEventId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<EventRecord> NewRecordAsync(DbCommand command, Guid id, string mtgaEventId, CancellationToken cancellationToken)
        {
            var newRecord = EventRecord.Create(Context, this, id, mtgaEventId);
            command.CommandText = "INSERT INTO [Mtga].[Events] ( Id, MtgaEventId ) VALUES ( @Id, @MtgaEventId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("MtgaEventId", newRecord.MtgaEventId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override EventRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var mtgaEventId = (string)dbDataReader["MtgaEventId"];
            return EventRecord.Create(Context, this, id, mtgaEventId);
        }
    }
}