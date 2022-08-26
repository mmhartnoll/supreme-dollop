using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FacePrintingRecord : DatabaseRecord<FacePrintingRecord>
    {
        private Guid _imageId;

        public Guid Id { get; }
        public Guid BasePrintingId { get; }
        public Guid FaceId { get; }

        public Guid ImageId
        {
            get => _imageId;
            set
            {
                IsModified |= _imageId != value;
                _imageId = value;
            }
        }

        private FacePrintingRecord(DatabaseContext dataContext, FacePrintingsTable facePrintingsTable, Guid id, Guid basePrintingId, Guid faceId, Guid imageId) : base(dataContext, facePrintingsTable)
        {
            Id = id;
            BasePrintingId = basePrintingId;
            FaceId = faceId;
            _imageId = imageId;
        }

        internal static FacePrintingRecord Create(DatabaseContext dataContext, FacePrintingsTable facePrintingsTable, Guid id, Guid basePrintingId, Guid faceId, Guid imageId)
        {
            return new FacePrintingRecord(dataContext, facePrintingsTable, id, basePrintingId, faceId, imageId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FacePrintings] SET BasePrintingId = @BasePrintingId, FaceId = @FaceId, ImageId = @ImageId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("BasePrintingId", BasePrintingId);
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("ImageId", ImageId);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FacePrintings] SET BasePrintingId = @BasePrintingId, FaceId = @FaceId, ImageId = @ImageId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("BasePrintingId", BasePrintingId);
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("ImageId", ImageId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[FacePrintings] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[FacePrintings] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}