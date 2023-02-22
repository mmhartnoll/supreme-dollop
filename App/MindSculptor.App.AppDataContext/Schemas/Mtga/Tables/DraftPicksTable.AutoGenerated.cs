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
    public class DraftPicksTable : DatabaseTable<DraftPickRecord, DraftPickRecordExpression>
    {
        private DraftPicksTable(DatabaseContext databaseContext) : base(databaseContext, "Mtga", "DraftPicks")
        {
        }

        internal static DraftPicksTable Create(DatabaseContext databaseContext)
        {
            return new DraftPicksTable(databaseContext);
        }

        public DraftPickRecord NewRecord(Guid eventEntryId, int packNumber, int pickNumber, bool isFifthCopy)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), eventEntryId, packNumber, pickNumber, isFifthCopy));
        }

        public async Task<DraftPickRecord> NewRecordAsync(Guid eventEntryId, int packNumber, int pickNumber, bool isFifthCopy, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), eventEntryId, packNumber, pickNumber, isFifthCopy, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public DraftPickRecord NewRecord(EventEntryRecord eventEntryRecord, int packNumber, int pickNumber, bool isFifthCopy)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), eventEntryRecord.Id, packNumber, pickNumber, isFifthCopy));
        }

        public async Task<DraftPickRecord> NewRecordAsync(EventEntryRecord eventEntryRecord, int packNumber, int pickNumber, bool isFifthCopy, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), eventEntryRecord.Id, packNumber, pickNumber, isFifthCopy, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private DraftPickRecord NewRecord(DbCommand command, Guid id, Guid eventEntryId, int packNumber, int pickNumber, bool isFifthCopy)
        {
            var newRecord = DraftPickRecord.Create(DatabaseContext, this, id, eventEntryId, packNumber, pickNumber, isFifthCopy);
            command.CommandText = "INSERT INTO [Mtga].[DraftPicks] ( Id, EventEntryId, PackNumber, PickNumber, IsFifthCopy ) VALUES ( @Id, @EventEntryId, @PackNumber, @PickNumber, @IsFifthCopy );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventEntryId", newRecord.EventEntryId);
            command.AddParameter("PackNumber", newRecord.PackNumber);
            command.AddParameter("PickNumber", newRecord.PickNumber);
            command.AddParameter("IsFifthCopy", newRecord.IsFifthCopy);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<DraftPickRecord> NewRecordAsync(DbCommand command, Guid id, Guid eventEntryId, int packNumber, int pickNumber, bool isFifthCopy, CancellationToken cancellationToken)
        {
            var newRecord = DraftPickRecord.Create(DatabaseContext, this, id, eventEntryId, packNumber, pickNumber, isFifthCopy);
            command.CommandText = "INSERT INTO [Mtga].[DraftPicks] ( Id, EventEntryId, PackNumber, PickNumber, IsFifthCopy ) VALUES ( @Id, @EventEntryId, @PackNumber, @PickNumber, @IsFifthCopy );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("EventEntryId", newRecord.EventEntryId);
            command.AddParameter("PackNumber", newRecord.PackNumber);
            command.AddParameter("PickNumber", newRecord.PickNumber);
            command.AddParameter("IsFifthCopy", newRecord.IsFifthCopy);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override DraftPickRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var eventEntryId = (Guid)dbDataReader["EventEntryId"];
            var packNumber = Convert.ToInt32(dbDataReader["PackNumber"]);
            var pickNumber = Convert.ToInt32(dbDataReader["PickNumber"]);
            var isFifthCopy = (bool)dbDataReader["IsFifthCopy"];
            return DraftPickRecord.Create(DatabaseContext, this, id, eventEntryId, packNumber, pickNumber, isFifthCopy);
        }
    }
}