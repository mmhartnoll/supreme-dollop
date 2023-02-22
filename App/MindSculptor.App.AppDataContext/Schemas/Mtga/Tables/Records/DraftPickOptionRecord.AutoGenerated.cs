using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DraftPickOptionRecord : DatabaseRecord<DraftPickOptionRecord>
    {
        public Guid Id { get; }
        public Guid EventId { get; }
        public Guid EventEntryId { get; }
        public Guid DigitalCardId { get; }
        public int PackNumber { get; }
        public int PickNumber { get; }
        public int Ordinal { get; }

        private DraftPickOptionRecord(DatabaseContext databaseContext, DraftPickOptionsTable draftPickOptionsTable, Guid id, Guid eventId, Guid eventEntryId, Guid digitalCardId, int packNumber, int pickNumber, int ordinal) : base(databaseContext, draftPickOptionsTable)
        {
            Id = id;
            EventId = eventId;
            EventEntryId = eventEntryId;
            DigitalCardId = digitalCardId;
            PackNumber = packNumber;
            PickNumber = pickNumber;
            Ordinal = ordinal;
        }

        internal static DraftPickOptionRecord Create(DatabaseContext databaseContext, DraftPickOptionsTable draftPickOptionsTable, Guid id, Guid eventId, Guid eventEntryId, Guid digitalCardId, int packNumber, int pickNumber, int ordinal)
        {
            return new DraftPickOptionRecord(databaseContext, draftPickOptionsTable, id, eventId, eventEntryId, digitalCardId, packNumber, pickNumber, ordinal);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[DraftPickOptions] SET EventId = @EventId, EventEntryId = @EventEntryId, DigitalCardId = @DigitalCardId, PackNumber = @PackNumber, PickNumber = @PickNumber, Ordinal = @Ordinal WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventId", EventId);
                command.AddParameter("EventEntryId", EventEntryId);
                command.AddParameter("DigitalCardId", DigitalCardId);
                command.AddParameter("PackNumber", PackNumber);
                command.AddParameter("PickNumber", PickNumber);
                command.AddParameter("Ordinal", Ordinal);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[DraftPickOptions] SET EventId = @EventId, EventEntryId = @EventEntryId, DigitalCardId = @DigitalCardId, PackNumber = @PackNumber, PickNumber = @PickNumber, Ordinal = @Ordinal WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventId", EventId);
                command.AddParameter("EventEntryId", EventEntryId);
                command.AddParameter("DigitalCardId", DigitalCardId);
                command.AddParameter("PackNumber", PackNumber);
                command.AddParameter("PickNumber", PickNumber);
                command.AddParameter("Ordinal", Ordinal);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[DraftPickOptions] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[DraftPickOptions] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}