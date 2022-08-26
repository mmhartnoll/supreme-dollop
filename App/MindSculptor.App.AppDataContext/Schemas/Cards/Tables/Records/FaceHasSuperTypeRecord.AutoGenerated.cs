using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceHasSuperTypeRecord : DatabaseRecord<FaceHasSuperTypeRecord>
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

        private FaceHasSuperTypeRecord(DatabaseContext dataContext, FaceHasSuperTypesTable faceHasSuperTypesTable, Guid faceId, Guid superTypeId, int ordinal) : base(dataContext, faceHasSuperTypesTable)
        {
            FaceId = faceId;
            SuperTypeId = superTypeId;
            _ordinal = ordinal;
        }

        internal static FaceHasSuperTypeRecord Create(DatabaseContext dataContext, FaceHasSuperTypesTable faceHasSuperTypesTable, Guid faceId, Guid superTypeId, int ordinal)
        {
            return new FaceHasSuperTypeRecord(dataContext, faceHasSuperTypesTable, faceId, superTypeId, ordinal);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FaceHasSuperTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND SuperTypeId = @SuperTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("SuperTypeId", SuperTypeId);
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
                command.CommandText = "UPDATE [Cards].[FaceHasSuperTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND SuperTypeId = @SuperTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("SuperTypeId", SuperTypeId);
                command.AddParameter("Ordinal", Ordinal);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[FaceHasSuperTypes] WHERE FaceId = @FaceId AND SuperTypeId = @SuperTypeId;";
            command.AddParameter("FaceId", FaceId);
            command.AddParameter("SuperTypeId", SuperTypeId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[FaceHasSuperTypes] WHERE FaceId = @FaceId AND SuperTypeId = @SuperTypeId;";
            command.AddParameter("FaceId", FaceId);
            command.AddParameter("SuperTypeId", SuperTypeId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}