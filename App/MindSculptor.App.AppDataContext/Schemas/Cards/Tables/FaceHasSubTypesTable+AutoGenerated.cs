using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FaceHasSubTypesTable : DataContextTable<FaceHasSubTypeRecord, FaceHasSubTypeRecordExpression>
    {
        private FaceHasSubTypesTable(DataContext dataContext) : base(dataContext, "Cards", "FaceHasSubTypes")
        {
        }

        internal static FaceHasSubTypesTable Create(DataContext dataContext)
        {
            return new FaceHasSubTypesTable(dataContext);
        }

        public FaceHasSubTypeRecord NewRecord(FaceRecord faceRecord, SubTypeRecord subTypeRecord, int ordinal)
        {
            var newRecord = FaceHasSubTypeRecord.Create(DataContext, faceRecord.Id, subTypeRecord.Id, ordinal);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FaceHasSubTypes] ( FaceId, SubTypeId, Ordinal ) VALUES ( @FaceId, @SubTypeId, @Ordinal );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("SubTypeId", newRecord.SubTypeId);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<FaceHasSubTypeRecord> NewRecordAsync(FaceRecord faceRecord, SubTypeRecord subTypeRecord, int ordinal)
        {
            var newRecord = FaceHasSubTypeRecord.Create(DataContext, faceRecord.Id, subTypeRecord.Id, ordinal);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FaceHasSubTypes] ( FaceId, SubTypeId, Ordinal ) VALUES ( @FaceId, @SubTypeId, @Ordinal );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("SubTypeId", newRecord.SubTypeId);
                command.AddParameter("Ordinal", newRecord.Ordinal);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override FaceHasSubTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var subTypeId = (Guid)dbDataReader["SubTypeId"];
            var ordinal = Convert.ToInt32(dbDataReader["Ordinal"]);
            return FaceHasSubTypeRecord.Create(DataContext, faceId, subTypeId, ordinal);
        }
    }
}