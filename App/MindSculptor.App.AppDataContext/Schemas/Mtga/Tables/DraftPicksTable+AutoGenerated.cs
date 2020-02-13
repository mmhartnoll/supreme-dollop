using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class DraftPicksTable : DataContextTable<DraftPickRecord, DraftPickRecordExpression>
    {
        private DraftPicksTable(DataContext dataContext) : base(dataContext, "Mtga", "DraftPicks")
        {
        }

        internal static DraftPicksTable Create(DataContext dataContext)
        {
            return new DraftPicksTable(dataContext);
        }

        public DraftPickRecord NewRecord(DraftEventEntryRecord draftEventEntryRecord, CardRecord cardRecord, int packNumber, int pickNumber, int ordinal, bool isPick)
        {
            var newRecord = DraftPickRecord.Create(DataContext, Guid.NewGuid(), draftEventEntryRecord.Id, cardRecord.Id, packNumber, pickNumber, ordinal, isPick);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftPicks] ( Id, DraftEventEntryId, DigitalCardId, PackNumber, PickNumber, Ordinal, IsPick ) VALUES ( @Id, @DraftEventEntryId, @DigitalCardId, @PackNumber, @PickNumber, @Ordinal, @IsPick );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("DraftEventEntryId", newRecord.DraftEventEntryId);
                command.AddParameter("DigitalCardId", newRecord.DigitalCardId);
                command.AddParameter("PackNumber", newRecord.PackNumber);
                command.AddParameter("PickNumber", newRecord.PickNumber);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                command.AddParameter("IsPick", newRecord.IsPick);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<DraftPickRecord> NewRecordAsync(DraftEventEntryRecord draftEventEntryRecord, CardRecord cardRecord, int packNumber, int pickNumber, int ordinal, bool isPick)
        {
            var newRecord = DraftPickRecord.Create(DataContext, Guid.NewGuid(), draftEventEntryRecord.Id, cardRecord.Id, packNumber, pickNumber, ordinal, isPick);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftPicks] ( Id, DraftEventEntryId, DigitalCardId, PackNumber, PickNumber, Ordinal, IsPick ) VALUES ( @Id, @DraftEventEntryId, @DigitalCardId, @PackNumber, @PickNumber, @Ordinal, @IsPick );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("DraftEventEntryId", newRecord.DraftEventEntryId);
                command.AddParameter("DigitalCardId", newRecord.DigitalCardId);
                command.AddParameter("PackNumber", newRecord.PackNumber);
                command.AddParameter("PickNumber", newRecord.PickNumber);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                command.AddParameter("IsPick", newRecord.IsPick);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override DraftPickRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var draftEventEntryId = (Guid)dbDataReader["DraftEventEntryId"];
            var digitalCardId = (Guid)dbDataReader["DigitalCardId"];
            var packNumber = Convert.ToInt32(dbDataReader["PackNumber"]);
            var pickNumber = Convert.ToInt32(dbDataReader["PickNumber"]);
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            var isPick = (bool)dbDataReader["IsPick"];
            return DraftPickRecord.Create(DataContext, id, draftEventEntryId, digitalCardId, packNumber, pickNumber, ordinal, isPick);
        }
    }
}