using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FaceHasSuperTypesTable : DataContextTable<FaceHasSuperTypeRecord, FaceHasSuperTypeRecordExpression>
    {
        private FaceHasSuperTypesTable(DataContext dataContext) : base(dataContext, "Cards", "FaceHasSuperTypes")
        {
        }

        internal static FaceHasSuperTypesTable Create(DataContext dataContext)
        {
            return new FaceHasSuperTypesTable(dataContext);
        }

        public FaceHasSuperTypeRecord NewRecord(FaceRecord faceRecord, SuperTypeRecord superTypeRecord, int ordinal)
        {
            var newRecord = FaceHasSuperTypeRecord.Create(DataContext, faceRecord.Id, superTypeRecord.Id, ordinal);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FaceHasSuperTypes] ( FaceId, SuperTypeId, Ordinal ) VALUES ( @FaceId, @SuperTypeId, @Ordinal );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("SuperTypeId", newRecord.SuperTypeId);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<FaceHasSuperTypeRecord> NewRecordAsync(FaceRecord faceRecord, SuperTypeRecord superTypeRecord, int ordinal)
        {
            var newRecord = FaceHasSuperTypeRecord.Create(DataContext, faceRecord.Id, superTypeRecord.Id, ordinal);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FaceHasSuperTypes] ( FaceId, SuperTypeId, Ordinal ) VALUES ( @FaceId, @SuperTypeId, @Ordinal );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("SuperTypeId", newRecord.SuperTypeId);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override FaceHasSuperTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var superTypeId = (Guid)dbDataReader["SuperTypeId"];
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            return FaceHasSuperTypeRecord.Create(DataContext, faceId, superTypeId, ordinal);
        }
    }
}