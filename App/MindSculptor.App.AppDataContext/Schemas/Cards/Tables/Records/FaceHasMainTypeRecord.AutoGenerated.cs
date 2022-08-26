using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceHasMainTypeRecord : DatabaseRecord<FaceHasMainTypeRecord>
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

        private FaceHasMainTypeRecord(DatabaseContext dataContext, FaceHasMainTypesTable faceHasMainTypesTable, Guid faceId, Guid mainTypeId, int ordinal) : base(dataContext, faceHasMainTypesTable)
        {
            FaceId = faceId;
            MainTypeId = mainTypeId;
            _ordinal = ordinal;
        }

        internal static FaceHasMainTypeRecord Create(DatabaseContext dataContext, FaceHasMainTypesTable faceHasMainTypesTable, Guid faceId, Guid mainTypeId, int ordinal)
        {
            return new FaceHasMainTypeRecord(dataContext, faceHasMainTypesTable, faceId, mainTypeId, ordinal);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FaceHasMainTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND MainTypeId = @MainTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("MainTypeId", MainTypeId);
                command.AddParameter("Ordinal", Ordinal);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FaceHasMainTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND MainTypeId = @MainTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("MainTypeId", MainTypeId);
                command.AddParameter("Ordinal", Ordinal);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[FaceHasMainTypes] WHERE FaceId = @FaceId AND MainTypeId = @MainTypeId;";
            command.AddParameter("FaceId", FaceId);
            command.AddParameter("MainTypeId", MainTypeId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[FaceHasMainTypes] WHERE FaceId = @FaceId AND MainTypeId = @MainTypeId;";
            command.AddParameter("FaceId", FaceId);
            command.AddParameter("MainTypeId", MainTypeId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}