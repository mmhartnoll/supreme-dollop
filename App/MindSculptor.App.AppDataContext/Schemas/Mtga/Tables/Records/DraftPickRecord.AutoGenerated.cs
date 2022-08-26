using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DraftPickRecord : DatabaseRecord<DraftPickRecord>
    {
        public Guid Id { get; }
        public Guid EventEntryId { get; }
        public int PackNumber { get; }
        public int PickNumber { get; }
        public bool IsFifthCopy { get; }

        private DraftPickRecord(DatabaseContext dataContext, DraftPicksTable draftPicksTable, Guid id, Guid eventEntryId, int packNumber, int pickNumber, bool isFifthCopy) : base(dataContext, draftPicksTable)
        {
            Id = id;
            EventEntryId = eventEntryId;
            PackNumber = packNumber;
            PickNumber = pickNumber;
            IsFifthCopy = isFifthCopy;
        }

        internal static DraftPickRecord Create(DatabaseContext dataContext, DraftPicksTable draftPicksTable, Guid id, Guid eventEntryId, int packNumber, int pickNumber, bool isFifthCopy)
        {
            return new DraftPickRecord(dataContext, draftPicksTable, id, eventEntryId, packNumber, pickNumber, isFifthCopy);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[DraftPicks] SET EventEntryId = @EventEntryId, PackNumber = @PackNumber, PickNumber = @PickNumber, IsFifthCopy = @IsFifthCopy WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventEntryId", EventEntryId);
                command.AddParameter("PackNumber", PackNumber);
                command.AddParameter("PickNumber", PickNumber);
                command.AddParameter("IsFifthCopy", IsFifthCopy);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Mtga].[DraftPicks] SET EventEntryId = @EventEntryId, PackNumber = @PackNumber, PickNumber = @PickNumber, IsFifthCopy = @IsFifthCopy WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("EventEntryId", EventEntryId);
                command.AddParameter("PackNumber", PackNumber);
                command.AddParameter("PickNumber", PickNumber);
                command.AddParameter("IsFifthCopy", IsFifthCopy);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Mtga].[DraftPicks] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Mtga].[DraftPicks] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}