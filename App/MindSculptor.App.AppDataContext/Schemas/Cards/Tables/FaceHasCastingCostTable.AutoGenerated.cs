using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FaceHasCastingCostTable : DatabaseTable<FaceHasCastingCostRecord, FaceHasCastingCostRecordExpression>
    {
        private FaceHasCastingCostTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "FaceHasCastingCost")
        {
        }

        internal static FaceHasCastingCostTable Create(DatabaseContext databaseContext)
        {
            return new FaceHasCastingCostTable(databaseContext);
        }

        public FaceHasCastingCostRecord NewRecord(Guid faceId, Guid manaSymbolId, int ordinal, int count)
        {
            return DatabaseContext.Execute(command => NewRecord(command, faceId, manaSymbolId, ordinal, count));
        }

        public async Task<FaceHasCastingCostRecord> NewRecordAsync(Guid faceId, Guid manaSymbolId, int ordinal, int count, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceId, manaSymbolId, ordinal, count, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public FaceHasCastingCostRecord NewRecord(FaceRecord faceRecord, ManaSymbolRecord manaSymbolRecord, int ordinal, int count)
        {
            return DatabaseContext.Execute(command => NewRecord(command, faceRecord.Id, manaSymbolRecord.Id, ordinal, count));
        }

        public async Task<FaceHasCastingCostRecord> NewRecordAsync(FaceRecord faceRecord, ManaSymbolRecord manaSymbolRecord, int ordinal, int count, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceRecord.Id, manaSymbolRecord.Id, ordinal, count, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private FaceHasCastingCostRecord NewRecord(DbCommand command, Guid faceId, Guid manaSymbolId, int ordinal, int count)
        {
            var newRecord = FaceHasCastingCostRecord.Create(DatabaseContext, this, faceId, manaSymbolId, ordinal, count);
            command.CommandText = "INSERT INTO [Cards].[FaceHasCastingCost] ( FaceId, ManaSymbolId, Ordinal, Count ) VALUES ( @FaceId, @ManaSymbolId, @Ordinal, @Count );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("ManaSymbolId", newRecord.ManaSymbolId);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            command.AddParameter("Count", newRecord.Count);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<FaceHasCastingCostRecord> NewRecordAsync(DbCommand command, Guid faceId, Guid manaSymbolId, int ordinal, int count, CancellationToken cancellationToken)
        {
            var newRecord = FaceHasCastingCostRecord.Create(DatabaseContext, this, faceId, manaSymbolId, ordinal, count);
            command.CommandText = "INSERT INTO [Cards].[FaceHasCastingCost] ( FaceId, ManaSymbolId, Ordinal, Count ) VALUES ( @FaceId, @ManaSymbolId, @Ordinal, @Count );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("ManaSymbolId", newRecord.ManaSymbolId);
            command.AddParameter("Ordinal", newRecord.Ordinal);
            command.AddParameter("Count", newRecord.Count);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override FaceHasCastingCostRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var manaSymbolId = (Guid)dbDataReader["ManaSymbolId"];
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            var count = Convert.ToInt32(dbDataReader["Count"]);
            return FaceHasCastingCostRecord.Create(DatabaseContext, this, faceId, manaSymbolId, ordinal, count);
        }
    }
}