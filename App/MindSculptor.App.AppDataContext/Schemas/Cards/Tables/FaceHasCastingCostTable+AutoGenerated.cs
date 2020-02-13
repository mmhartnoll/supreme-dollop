using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FaceHasCastingCostTable : DataContextTable<FaceHasCastingCostRecord, FaceHasCastingCostRecordExpression>
    {
        private FaceHasCastingCostTable(DataContext dataContext) : base(dataContext, "Cards", "FaceHasCastingCost")
        {
        }

        internal static FaceHasCastingCostTable Create(DataContext dataContext)
        {
            return new FaceHasCastingCostTable(dataContext);
        }

        public FaceHasCastingCostRecord NewRecord(FaceRecord faceRecord, ManaSymbolRecord manaSymbolRecord, int ordinal, int count)
        {
            var newRecord = FaceHasCastingCostRecord.Create(DataContext, faceRecord.Id, manaSymbolRecord.Id, ordinal, count);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FaceHasCastingCost] ( FaceId, ManaSymbolId, Ordinal, Count ) VALUES ( @FaceId, @ManaSymbolId, @Ordinal, @Count );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("ManaSymbolId", newRecord.ManaSymbolId);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                command.AddParameter("Count", newRecord.Count);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<FaceHasCastingCostRecord> NewRecordAsync(FaceRecord faceRecord, ManaSymbolRecord manaSymbolRecord, int ordinal, int count)
        {
            var newRecord = FaceHasCastingCostRecord.Create(DataContext, faceRecord.Id, manaSymbolRecord.Id, ordinal, count);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FaceHasCastingCost] ( FaceId, ManaSymbolId, Ordinal, Count ) VALUES ( @FaceId, @ManaSymbolId, @Ordinal, @Count );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("ManaSymbolId", newRecord.ManaSymbolId);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                command.AddParameter("Count", newRecord.Count);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override FaceHasCastingCostRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var manaSymbolId = (Guid)dbDataReader["ManaSymbolId"];
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            var count = Convert.ToInt32(dbDataReader["Count"]);
            return FaceHasCastingCostRecord.Create(DataContext, faceId, manaSymbolId, ordinal, count);
        }
    }
}