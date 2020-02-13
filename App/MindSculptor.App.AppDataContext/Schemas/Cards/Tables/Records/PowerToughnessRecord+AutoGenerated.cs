using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class PowerToughnessRecord : DataContextRecord
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

        private PowerToughnessRecord(DataContext dataContext, Guid faceId, int basePower, string powerFormat, int baseToughness, string toughnessFormat) : base(dataContext)
        {
            FaceId = faceId;
            _basePower = basePower;
            _powerFormat = powerFormat;
            _baseToughness = baseToughness;
            _toughnessFormat = toughnessFormat;
        }

        internal static PowerToughnessRecord Create(DataContext dataContext, Guid faceId, int basePower, string powerFormat, int baseToughness, string toughnessFormat)
        {
            return new PowerToughnessRecord(dataContext, faceId, basePower, powerFormat, baseToughness, toughnessFormat);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[PowerToughness] SET BasePower = @BasePower, PowerFormat = @PowerFormat, BaseToughness = @BaseToughness, ToughnessFormat = @ToughnessFormat WHERE FaceId = @FaceId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("BasePower", BasePower);
                    command.AddParameter("PowerFormat", PowerFormat);
                    command.AddParameter("BaseToughness", BaseToughness);
                    command.AddParameter("ToughnessFormat", ToughnessFormat);
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
                    command.CommandText = "UPDATE [Cards].[PowerToughness] SET BasePower = @BasePower, PowerFormat = @PowerFormat, BaseToughness = @BaseToughness, ToughnessFormat = @ToughnessFormat WHERE FaceId = @FaceId;";
                    command.AddParameter("FaceId", FaceId);
                    command.AddParameter("BasePower", BasePower);
                    command.AddParameter("PowerFormat", PowerFormat);
                    command.AddParameter("BaseToughness", BaseToughness);
                    command.AddParameter("ToughnessFormat", ToughnessFormat);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[PowerToughness] WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[PowerToughness] WHERE FaceId = @FaceId;";
                command.AddParameter("FaceId", FaceId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}