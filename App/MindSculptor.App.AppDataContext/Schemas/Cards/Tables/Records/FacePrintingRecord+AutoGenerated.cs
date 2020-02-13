using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FacePrintingRecord : DataContextRecord
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

        private FacePrintingRecord(DataContext dataContext, Guid id, Guid basePrintingId, Guid faceId, Guid imageId) : base(dataContext)
        {
            Id = id;
            BasePrintingId = basePrintingId;
            FaceId = faceId;
            _imageId = imageId;
        }

        internal static FacePrintingRecord Create(DataContext dataContext, Guid id, Guid basePrintingId, Guid faceId, Guid imageId)
        {
            return new FacePrintingRecord(dataContext, id, basePrintingId, faceId, imageId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[FacePrintings] SET BasePrintingId = @BasePrintingId, FaceId = @FaceId, ImageId = @ImageId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("BasePrintingId", BasePrintingId);
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("ImageId", ImageId);
                    command.ExecuteNonQuery();
                }
        }

        public async override Task UpdateRecordAsync()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[FacePrintings] SET BasePrintingId = @BasePrintingId, FaceId = @FaceId, ImageId = @ImageId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("BasePrintingId", BasePrintingId);
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("ImageId", ImageId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FacePrintings] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FacePrintings] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}