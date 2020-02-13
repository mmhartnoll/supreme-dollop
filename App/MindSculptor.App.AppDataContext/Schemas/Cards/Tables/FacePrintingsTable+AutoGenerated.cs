using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FacePrintingsTable : DataContextTable<FacePrintingRecord, FacePrintingRecordExpression>
    {
        private FacePrintingsTable(DataContext dataContext) : base(dataContext, "Cards", "FacePrintings")
        {
        }

        internal static FacePrintingsTable Create(DataContext dataContext)
        {
            return new FacePrintingsTable(dataContext);
        }

        public FacePrintingRecord NewRecord(BasePrintingRecord basePrintingRecord, FaceRecord faceRecord)
        {
            var newRecord = FacePrintingRecord.Create(DataContext, Guid.NewGuid(), basePrintingRecord.Id, faceRecord.Id, Guid.NewGuid());
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FacePrintings] ( Id, BasePrintingId, FaceId, ImageId ) VALUES ( @Id, @BasePrintingId, @FaceId, @ImageId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("BasePrintingId", newRecord.BasePrintingId);
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("ImageId", newRecord.ImageId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<FacePrintingRecord> NewRecordAsync(BasePrintingRecord basePrintingRecord, FaceRecord faceRecord)
        {
            var newRecord = FacePrintingRecord.Create(DataContext, Guid.NewGuid(), basePrintingRecord.Id, faceRecord.Id, Guid.NewGuid());
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FacePrintings] ( Id, BasePrintingId, FaceId, ImageId ) VALUES ( @Id, @BasePrintingId, @FaceId, @ImageId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("BasePrintingId", newRecord.BasePrintingId);
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("ImageId", newRecord.ImageId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override FacePrintingRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var basePrintingId = (Guid)dbDataReader["BasePrintingId"];
            var faceId = (Guid)dbDataReader["FaceId"];
            var imageId = (Guid)dbDataReader["ImageId"];
            return FacePrintingRecord.Create(DataContext, id, basePrintingId, faceId, imageId);
        }
    }
}