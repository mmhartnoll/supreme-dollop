using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class OracleTextRecord : DatabaseRecord<OracleTextRecord>
    {
        private string _value;

        public Guid FaceId { get; }

        public string Value
        {
            get => _value;
            set
            {
                IsModified |= _value != value;
                _value = value;
            }
        }

        private OracleTextRecord(DatabaseContext databaseContext, OracleTextTable oracleTextTable, Guid faceId, string value) : base(databaseContext, oracleTextTable)
        {
            FaceId = faceId;
            _value = value;
        }

        internal static OracleTextRecord Create(DatabaseContext databaseContext, OracleTextTable oracleTextTable, Guid faceId, string value)
        {
            return new OracleTextRecord(databaseContext, oracleTextTable, faceId, value);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[OracleText] SET Value = @Value WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("Value", Value);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[OracleText] SET Value = @Value WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("Value", Value);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[OracleText] WHERE FaceId = @FaceId;";
            command.AddParameter("FaceId", FaceId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[OracleText] WHERE FaceId = @FaceId;";
            command.AddParameter("FaceId", FaceId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}