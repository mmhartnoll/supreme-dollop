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
    public class FacePrintingsTable : DatabaseTable<FacePrintingRecord, FacePrintingRecordExpression>
    {
        private FacePrintingsTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "FacePrintings")
        {
        }

        internal static FacePrintingsTable Create(DatabaseContext databaseContext)
        {
            return new FacePrintingsTable(databaseContext);
        }

        public FacePrintingRecord NewRecord(Guid basePrintingId, Guid faceId)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), basePrintingId, faceId, Guid.NewGuid()));
        }

        public async Task<FacePrintingRecord> NewRecordAsync(Guid basePrintingId, Guid faceId, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), basePrintingId, faceId, Guid.NewGuid(), cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public FacePrintingRecord NewRecord(BasePrintingRecord basePrintingRecord, FaceRecord faceRecord)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), basePrintingRecord.Id, faceRecord.Id, Guid.NewGuid()));
        }

        public async Task<FacePrintingRecord> NewRecordAsync(BasePrintingRecord basePrintingRecord, FaceRecord faceRecord, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), basePrintingRecord.Id, faceRecord.Id, Guid.NewGuid(), cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private FacePrintingRecord NewRecord(DbCommand command, Guid id, Guid basePrintingId, Guid faceId, Guid imageId)
        {
            var newRecord = FacePrintingRecord.Create(DatabaseContext, this, id, basePrintingId, faceId, imageId);
            command.CommandText = "INSERT INTO [Cards].[FacePrintings] ( Id, BasePrintingId, FaceId, ImageId ) VALUES ( @Id, @BasePrintingId, @FaceId, @ImageId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("BasePrintingId", newRecord.BasePrintingId);
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("ImageId", newRecord.ImageId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<FacePrintingRecord> NewRecordAsync(DbCommand command, Guid id, Guid basePrintingId, Guid faceId, Guid imageId, CancellationToken cancellationToken)
        {
            var newRecord = FacePrintingRecord.Create(DatabaseContext, this, id, basePrintingId, faceId, imageId);
            command.CommandText = "INSERT INTO [Cards].[FacePrintings] ( Id, BasePrintingId, FaceId, ImageId ) VALUES ( @Id, @BasePrintingId, @FaceId, @ImageId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("BasePrintingId", newRecord.BasePrintingId);
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("ImageId", newRecord.ImageId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override FacePrintingRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var basePrintingId = (Guid)dbDataReader["BasePrintingId"];
            var faceId = (Guid)dbDataReader["FaceId"];
            var imageId = (Guid)dbDataReader["ImageId"];
            return FacePrintingRecord.Create(DatabaseContext, this, id, basePrintingId, faceId, imageId);
        }
    }
}