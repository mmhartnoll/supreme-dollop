using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class PowerToughnessRecord : DatabaseRecord<PowerToughnessRecord>
    {
        private int _basePower;
        private string _powerFormat;
        private int _baseToughness;
        private string _toughnessFormat;

        public Guid FaceId { get; }

        public int BasePower
        {
            get => _basePower;
            set
            {
                IsModified |= _basePower != value;
                _basePower = value;
            }
        }

        public string PowerFormat
        {
            get => _powerFormat;
            set
            {
                IsModified |= _powerFormat != value;
                _powerFormat = value;
            }
        }

        public int BaseToughness
        {
            get => _baseToughness;
            set
            {
                IsModified |= _baseToughness != value;
                _baseToughness = value;
            }
        }

        public string ToughnessFormat
        {
            get => _toughnessFormat;
            set
            {
                IsModified |= _toughnessFormat != value;
                _toughnessFormat = value;
            }
        }

        private PowerToughnessRecord(DatabaseContext databaseContext, PowerToughnessTable powerToughnessTable, Guid faceId, int basePower, string powerFormat, int baseToughness, string toughnessFormat) : base(databaseContext, powerToughnessTable)
        {
            FaceId = faceId;
            _basePower = basePower;
            _powerFormat = powerFormat;
            _baseToughness = baseToughness;
            _toughnessFormat = toughnessFormat;
        }

        internal static PowerToughnessRecord Create(DatabaseContext databaseContext, PowerToughnessTable powerToughnessTable, Guid faceId, int basePower, string powerFormat, int baseToughness, string toughnessFormat)
        {
            return new PowerToughnessRecord(databaseContext, powerToughnessTable, faceId, basePower, powerFormat, baseToughness, toughnessFormat);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[PowerToughness] SET BasePower = @BasePower, PowerFormat = @PowerFormat, BaseToughness = @BaseToughness, ToughnessFormat = @ToughnessFormat WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("BasePower", BasePower);
                command.AddParameter("PowerFormat", PowerFormat);
                command.AddParameter("BaseToughness", BaseToughness);
                command.AddParameter("ToughnessFormat", ToughnessFormat);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[PowerToughness] SET BasePower = @BasePower, PowerFormat = @PowerFormat, BaseToughness = @BaseToughness, ToughnessFormat = @ToughnessFormat WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                command.AddParameter("BasePower", BasePower);
                command.AddParameter("PowerFormat", PowerFormat);
                command.AddParameter("BaseToughness", BaseToughness);
                command.AddParameter("ToughnessFormat", ToughnessFormat);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[PowerToughness] WHERE FaceId = @FaceId;";
            command.AddParameter("FaceId", FaceId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[PowerToughness] WHERE FaceId = @FaceId;";
            command.AddParameter("FaceId", FaceId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}