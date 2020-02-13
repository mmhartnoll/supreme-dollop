using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceHasSubTypeRecord : DataContextRecord
    {
        private int _ordinal;

        public Guid FaceId { get; }
        public Guid SubTypeId { get; }

        public int Ordinal
        {
            get => _ordinal;
            set
            {
                IsModified |= _ordinal != value;
                _ordinal = value;
            }
        }

        private FaceHasSubTypeRecord(DataContext dataContext, Guid faceId, Guid subTypeId, int ordinal) : base(dataContext)
        {
            FaceId = faceId;
            SubTypeId = subTypeId;
            _ordinal = ordinal;
        }

        internal static FaceHasSubTypeRecord Create(DataContext dataContext, Guid faceId, Guid subTypeId, int ordinal)
        {
            return new FaceHasSubTypeRecord(dataContext, faceId, subTypeId, ordinal);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[FaceHasSubTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND SubTypeId = @SubTypeId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("SubTypeId", SubTypeId);
                    command.AddParameter("Ordinal", Ordinal);
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
                    command.CommandText = "UPDATE [Cards].[FaceHasSubTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND SubTypeId = @SubTypeId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("SubTypeId", SubTypeId);
                    command.AddParameter("Ordinal", Ordinal);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FaceHasSubTypes] WHERE FaceId = @FaceId AND SubTypeId = @SubTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("SubTypeId", SubTypeId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FaceHasSubTypes] WHERE FaceId = @FaceId AND SubTypeId = @SubTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("SubTypeId", SubTypeId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}