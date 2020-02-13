using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class DraftEventMatchResultsTable : DataContextTable<DraftEventMatchResultRecord, DraftEventMatchResultRecordExpression>
    {
        private DraftEventMatchResultsTable(DataContext dataContext) : base(dataContext, "Mtga", "DraftEventMatchResults")
        {
        }

        internal static DraftEventMatchResultsTable Create(DataContext dataContext)
        {
            return new DraftEventMatchResultsTable(dataContext);
        }

        public DraftEventMatchResultRecord NewRecord(DraftEventMatchRecord draftEventMatchRecord, bool matchWon)
        {
            var newRecord = DraftEventMatchResultRecord.Create(DataContext, draftEventMatchRecord.Id, matchWon);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEventMatchResults] ( DraftEventMatchId, MatchWon ) VALUES ( @DraftEventMatchId, @MatchWon );";
                command.AddParameter("DraftEventMatchId", newRecord.DraftEventMatchId);
                command.AddParameter("MatchWon", newRecord.MatchWon);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<DraftEventMatchResultRecord> NewRecordAsync(DraftEventMatchRecord draftEventMatchRecord, bool matchWon)
        {
            var newRecord = DraftEventMatchResultRecord.Create(DataContext, draftEventMatchRecord.Id, matchWon);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEventMatchResults] ( DraftEventMatchId, MatchWon ) VALUES ( @DraftEventMatchId, @MatchWon );";
                command.AddParameter("DraftEventMatchId", newRecord.DraftEventMatchId);
                command.AddParameter("MatchWon", newRecord.MatchWon);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override DraftEventMatchResultRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var draftEventMatchId = (Guid)dbDataReader["DraftEventMatchId"];
            var matchWon = (bool)dbDataReader["MatchWon"];
            return DraftEventMatchResultRecord.Create(DataContext, draftEventMatchId, matchWon);
        }
    }
}