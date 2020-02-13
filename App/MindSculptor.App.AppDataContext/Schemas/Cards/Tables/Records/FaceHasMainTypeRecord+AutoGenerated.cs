using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceHasMainTypeRecord : DataContextRecord
    {
        private int _ordinal;

        public Guid FaceId { get; }
        public Guid MainTypeId { get; }

        public int Ordinal
        {
            get => _ordinal;
            set
            {
                IsModified |= _ordinal != value;
                _ordinal = value;
            }
        }

        private FaceHasMainTypeRecord(DataContext dataContext, Guid faceId, Guid mainTypeId, int ordinal) : base(dataContext)
        {
            FaceId = faceId;
            MainTypeId = mainTypeId;
            _ordinal = ordinal;
        }

        internal static FaceHasMainTypeRecord Create(DataContext dataContext, Guid faceId, Guid mainTypeId, int ordinal)
        {
            return new FaceHasMainTypeRecord(dataContext, faceId, mainTypeId, ordinal);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[FaceHasMainTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND MainTypeId = @MainTypeId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("MainTypeId", MainTypeId);
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
                    command.CommandText = "UPDATE [Cards].[FaceHasMainTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND MainTypeId = @MainTypeId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("MainTypeId", MainTypeId);
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
                command.CommandText = "DELETE FROM [Cards].[FaceHasMainTypes] WHERE FaceId = @FaceId AND MainTypeId = @MainTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("MainTypeId", MainTypeId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FaceHasMainTypes] WHERE FaceId = @FaceId AND MainTypeId = @MainTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("MainTypeId", MainTypeId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}