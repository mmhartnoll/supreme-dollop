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
    public class DraftPickOptionsTable : DatabaseTable<DraftPickOptionRecord, DraftPickOptionRecordExpression>
    {
        private DraftPickOptionsTable(DatabaseContext dataContext) : base(dataContext, "Mtga", "DraftPickOptions")
        {
        }

        internal static DraftPickOptionsTable Create(DatabaseContext dataContext)
        {
            return new DraftPickOptionsTable(dataContext);
        }

        public DraftPickOptionRecord NewRecord(Guid eventId, Guid eventEntryId, Guid digitalCardId, int packNumber, int pickNumber, int ordinal)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), eventId, eventEntryId, digitalCardId, packNumber, pickNumber, ordinal));
        }

        public async Task<DraftPickOptionRecord> NewRecordAsync(Guid eventId, Guid eventEntryId, Guid digitalCardId, int packNumber, int pickNumber, int ordinal, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), eventId, eventEntryId, digitalCardId, packNumber, pickNumber, ordinal, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public DraftPickOptionRecord NewRecord(DraftEventRecord draftEventRecord, EventEntryRecord eventEntryRecord, DigitalCardRecord digitalCardRecord, int packNumber, int pickNumber, int ordinal)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), draftEventRecord.EventId, eventEntryRecord.Id, digitalCardRecord.Id, packNumber, pickNumber, ordinal));
        }

        public async Task<DraftPickOptionRecord> NewRecordAsync(DraftEventRecord draftEventRecord, EventEntryRecord eventEntryRecord, DigitalCardRecord digitalCardRecord, int packNumber, int pickNumber, int ordinal, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), draftEventRecord.EventId, eventEntryRecord.Id, digitalCardRecord.Id, packNumber, pickNumber, ordinal, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private DraftPickOptionRecord NewRecord(DbCommand command, Guid id, Guid eventId, Guid eventEntryId, Guid digitalCardId, int packNumber, int pickNumber, int ordinal)
        {
            var newRecord = DraftPickOptionRecord.Create(Context, this, id, eventId, eventEntryId, digitalCardId, packNumber, pickNumber, ordinal);
            command.CommandText = "INSERT INTO [Mtga].[DraftPickOptions] ( Id, EventId, EventEntryId, DigitalCardId, PackNumber, PickNumber, Ordinal ) VALUES ( @Id, @EventId, @EventEntryId, @DigitalCardId, @PackNumber, @PickNumber, @Ordinal );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventId", newRecord.EventId);
            command.AddParameter("EventEntryId", newRecord.EventEntryId);
            command.AddParameter("DigitalCardId", newRecord.DigitalCardId);
            command.AddParameter("PackNumber", newRecord.PackNumber);
            command.AddParameter("PickNumber", newRecord.PickNumber);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<DraftPickOptionRecord> NewRecordAsync(DbCommand command, Guid id, Guid eventId, Guid eventEntryId, Guid digitalCardId, int packNumber, int pickNumber, int ordinal, CancellationToken cancellationToken)
        {
            var newRecord = DraftPickOptionRecord.Create(Context, this, id, eventId, eventEntryId, digitalCardId, packNumber, pickNumber, ordinal);
            command.CommandText = "INSERT INTO [Mtga].[DraftPickOptions] ( Id, EventId, EventEntryId, DigitalCardId, PackNumber, PickNumber, Ordinal ) VALUES ( @Id, @EventId, @EventEntryId, @DigitalCardId, @PackNumber, @PickNumber, @Ordinal );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventId", newRecord.EventId);
            command.AddParameter("EventEntryId", newRecord.EventEntryId);
            command.AddParameter("DigitalCardId", newRecord.DigitalCardId);
            command.AddParameter("PackNumber", newRecord.PackNumber);
            command.AddParameter("PickNumber", newRecord.PickNumber);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override DraftPickOptionRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var eventId = (Guid)dbDataReader["EventId"];
            var eventEntryId = (Guid)dbDataReader["EventEntryId"];
            var digitalCardId = (Guid)dbDataReader["DigitalCardId"];
            var packNumber = Convert.ToInt32(dbDataReader["PackNumber"]);
            var pickNumber = Convert.ToInt32(dbDataReader["PickNumber"]);
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            return DraftPickOptionRecord.Create(Context, this, id, eventId, eventEntryId, digitalCardId, packNumber, pickNumber, ordinal);
        }
    }
}