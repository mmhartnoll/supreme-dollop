using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
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
    public class DraftEventsTable : DatabaseTable<DraftEventRecord, DraftEventRecordExpression>
    {
        private DraftEventsTable(DatabaseContext dataContext) : base(dataContext, "Mtga", "DraftEvents")
        {
        }

        internal static DraftEventsTable Create(DatabaseContext dataContext)
        {
            return new DraftEventsTable(dataContext);
        }

        public DraftEventRecord NewRecord(Guid eventId, Guid setId, string draftType)
        {
            return Context.Execute(command => NewRecord(command, eventId, setId, draftType));
        }

        public async Task<DraftEventRecord> NewRecordAsync(Guid eventId, Guid setId, string draftType, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, eventId, setId, draftType, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public DraftEventRecord NewRecord(EventRecord eventRecord, SetRecord setRecord, string draftType)
        {
            return Context.Execute(command => NewRecord(command, eventRecord.Id, setRecord.Id, draftType));
        }

        public async Task<DraftEventRecord> NewRecordAsync(EventRecord eventRecord, SetRecord setRecord, string draftType, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, eventRecord.Id, setRecord.Id, draftType, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private DraftEventRecord NewRecord(DbCommand command, Guid eventId, Guid setId, string draftType)
        {
            var newRecord = DraftEventRecord.Create(Context, this, eventId, setId, draftType);
            command.CommandText = "INSERT INTO [Mtga].[DraftEvents] ( EventId, SetId, DraftType ) VALUES ( @EventId, @SetId, @DraftType );";
            command.AddParameter("EventId", newRecord.EventId);
            command.AddParameter("SetId", newRecord.SetId);
            command.AddParameter("DraftType", newRecord.DraftType);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<DraftEventRecord> NewRecordAsync(DbCommand command, Guid eventId, Guid setId, string draftType, CancellationToken cancellationToken)
        {
            var newRecord = DraftEventRecord.Create(Context, this, eventId, setId, draftType);
            command.CommandText = "INSERT INTO [Mtga].[DraftEvents] ( EventId, SetId, DraftType ) VALUES ( @EventId, @SetId, @DraftType );";
            command.AddParameter("EventId", newRecord.EventId);
            command.AddParameter("SetId", newRecord.SetId);
            command.AddParameter("DraftType", newRecord.DraftType);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override DraftEventRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var eventId = (Guid)dbDataReader["EventId"];
            var setId = (Guid)dbDataReader["SetId"];
            var draftType = (string)dbDataReader["DraftType"];
            return DraftEventRecord.Create(Context, this, eventId, setId, draftType);
        }
    }
}