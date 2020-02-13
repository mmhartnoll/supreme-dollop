using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceHasSuperTypeRecord : DataContextRecord
    {
        private int _ordinal;

        public Guid FaceId { get; }
        public Guid SuperTypeId { get; }

        public int Ordinal
        {
            get => _ordinal;
            set
            {
                IsModified |= _ordinal != value;
                _ordinal = value;
            }
        }

        private FaceHasSuperTypeRecord(DataContext dataContext, Guid faceId, Guid superTypeId, int ordinal) : base(dataContext)
        {
            FaceId = faceId;
            SuperTypeId = superTypeId;
            _ordinal = ordinal;
        }

        internal static FaceHasSuperTypeRecord Create(DataContext dataContext, Guid faceId, Guid superTypeId, int ordinal)
        {
            return new FaceHasSuperTypeRecord(dataContext, faceId, superTypeId, ordinal);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[FaceHasSuperTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND SuperTypeId = @SuperTypeId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("SuperTypeId", SuperTypeId);
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
                    command.CommandText = "UPDATE [Cards].[FaceHasSuperTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND SuperTypeId = @SuperTypeId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("SuperTypeId", SuperTypeId);
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
                command.CommandText = "DELETE FROM [Cards].[FaceHasSuperTypes] WHERE FaceId = @FaceId AND SuperTypeId = @SuperTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("SuperTypeId", SuperTypeId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[FaceHasSuperTypes] WHERE FaceId = @FaceId AND SuperTypeId = @SuperTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("SuperTypeId", SuperTypeId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}