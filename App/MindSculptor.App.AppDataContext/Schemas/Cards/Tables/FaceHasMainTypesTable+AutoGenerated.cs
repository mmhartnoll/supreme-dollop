using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FaceHasMainTypesTable : DataContextTable<FaceHasMainTypeRecord, FaceHasMainTypeRecordExpression>
    {
        private FaceHasMainTypesTable(DataContext dataContext) : base(dataContext, "Cards", "FaceHasMainTypes")
        {
        }

        internal static FaceHasMainTypesTable Create(DataContext dataContext)
        {
            return new FaceHasMainTypesTable(dataContext);
        }

        public FaceHasMainTypeRecord NewRecord(FaceRecord faceRecord, MainTypeRecord mainTypeRecord, int ordinal)
        {
            var newRecord = FaceHasMainTypeRecord.Create(DataContext, faceRecord.Id, mainTypeRecord.Id, ordinal);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FaceHasMainTypes] ( FaceId, MainTypeId, Ordinal ) VALUES ( @FaceId, @MainTypeId, @Ordinal );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("MainTypeId", newRecord.MainTypeId);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<FaceHasMainTypeRecord> NewRecordAsync(FaceRecord faceRecord, MainTypeRecord mainTypeRecord, int ordinal)
        {
            var newRecord = FaceHasMainTypeRecord.Create(DataContext, faceRecord.Id, mainTypeRecord.Id, ordinal);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FaceHasMainTypes] ( FaceId, MainTypeId, Ordinal ) VALUES ( @FaceId, @MainTypeId, @Ordinal );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("MainTypeId", newRecord.MainTypeId);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override FaceHasMainTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var mainTypeId = (Guid)dbDataReader["MainTypeId"];
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            return FaceHasMainTypeRecord.Create(DataContext, faceId, mainTypeId, ordinal);
        }
    }
}