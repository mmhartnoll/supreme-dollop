using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class FaceHasSubTypeRecord : DatabaseRecord<FaceHasSubTypeRecord>
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

        private FaceHasSubTypeRecord(DatabaseContext databaseContext, FaceHasSubTypesTable faceHasSubTypesTable, Guid faceId, Guid subTypeId, int ordinal) : base(databaseContext, faceHasSubTypesTable)
        {
            FaceId = faceId;
            SubTypeId = subTypeId;
            _ordinal = ordinal;
        }

        internal static FaceHasSubTypeRecord Create(DatabaseContext databaseContext, FaceHasSubTypesTable faceHasSubTypesTable, Guid faceId, Guid subTypeId, int ordinal)
        {
            return new FaceHasSubTypeRecord(databaseContext, faceHasSubTypesTable, faceId, subTypeId, ordinal);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[FaceHasSubTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND SubTypeId = @SubTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("SubTypeId", SubTypeId);
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
                command.CommandText = "UPDATE [Cards].[FaceHasSubTypes] SET Ordinal = @Ordinal WHERE FaceId = @FaceId AND SubTypeId = @SubTypeId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("SubTypeId", SubTypeId);
                command.AddParameter("Ordinal", Ordinal);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[FaceHasSubTypes] WHERE FaceId = @FaceId AND SubTypeId = @SubTypeId;";
            command.AddParameter("FaceId", FaceId);
            command.AddParameter("SubTypeId", SubTypeId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[FaceHasSubTypes] WHERE FaceId = @FaceId AND SubTypeId = @SubTypeId;";
            command.AddParameter("FaceId", FaceId);
            command.AddParameter("SubTypeId", SubTypeId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}