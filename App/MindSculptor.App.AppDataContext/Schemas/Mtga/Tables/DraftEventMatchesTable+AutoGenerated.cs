using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class DraftEventMatchesTable : DataContextTable<DraftEventMatchRecord, DraftEventMatchRecordExpression>
    {
        private DraftEventMatchesTable(DataContext dataContext) : base(dataContext, "Mtga", "DraftEventMatches")
        {
        }

        internal static DraftEventMatchesTable Create(DataContext dataContext)
        {
            return new DraftEventMatchesTable(dataContext);
        }

        public DraftEventMatchRecord NewRecord(DraftEventEntryRecord draftEventEntryRecord, OpponentRecord opponentRecord, Guid id)
        {
            var newRecord = DraftEventMatchRecord.Create(DataContext, id, draftEventEntryRecord.Id, opponentRecord.Id);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEventMatches] ( Id, DraftEventEntryId, OpponentId ) VALUES ( @Id, @DraftEventEntryId, @OpponentId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("DraftEventEntryId", newRecord.DraftEventEntryId);
                command.AddParameter("OpponentId", newRecord.OpponentId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<DraftEventMatchRecord> NewRecordAsync(DraftEventEntryRecord draftEventEntryRecord, OpponentRecord opponentRecord, Guid id)
        {
            var newRecord = DraftEventMatchRecord.Create(DataContext, id, draftEventEntryRecord.Id, opponentRecord.Id);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEventMatches] ( Id, DraftEventEntryId, OpponentId ) VALUES ( @Id, @DraftEventEntryId, @OpponentId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("DraftEventEntryId", newRecord.DraftEventEntryId);
                command.AddParameter("OpponentId", newRecord.OpponentId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override DraftEventMatchRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var draftEventEntryId = (Guid)dbDataReader["DraftEventEntryId"];
            var opponentId = (Guid)dbDataReader["OpponentId"];
            return DraftEventMatchRecord.Create(DataContext, id, draftEventEntryId, opponentId);
        }
    }
}