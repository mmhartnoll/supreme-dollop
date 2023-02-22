using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class PowerToughnessTable : DatabaseTable<PowerToughnessRecord, PowerToughnessRecordExpression>
    {
        private PowerToughnessTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "PowerToughness")
        {
        }

        internal static PowerToughnessTable Create(DatabaseContext databaseContext)
        {
            return new PowerToughnessTable(databaseContext);
        }

        public PowerToughnessRecord NewRecord(Guid faceId, int basePower, string powerFormat, int baseToughness, string toughnessFormat)
        {
            return DatabaseContext.Execute(command => NewRecord(command, faceId, basePower, powerFormat, baseToughness, toughnessFormat));
        }

        public async Task<PowerToughnessRecord> NewRecordAsync(Guid faceId, int basePower, string powerFormat, int baseToughness, string toughnessFormat, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceId, basePower, powerFormat, baseToughness, toughnessFormat, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public PowerToughnessRecord NewRecord(FaceRecord faceRecord, int basePower, string powerFormat, int baseToughness, string toughnessFormat)
        {
            return DatabaseContext.Execute(command => NewRecord(command, faceRecord.Id, basePower, powerFormat, baseToughness, toughnessFormat));
        }

        public async Task<PowerToughnessRecord> NewRecordAsync(FaceRecord faceRecord, int basePower, string powerFormat, int baseToughness, string toughnessFormat, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, faceRecord.Id, basePower, powerFormat, baseToughness, toughnessFormat, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private PowerToughnessRecord NewRecord(DbCommand command, Guid faceId, int basePower, string powerFormat, int baseToughness, string toughnessFormat)
        {
            var newRecord = PowerToughnessRecord.Create(DatabaseContext, this, faceId, basePower, powerFormat, baseToughness, toughnessFormat);
            command.CommandText = "INSERT INTO [Cards].[PowerToughness] ( FaceId, BasePower, PowerFormat, BaseToughness, ToughnessFormat ) VALUES ( @FaceId, @BasePower, @PowerFormat, @BaseToughness, @ToughnessFormat );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("BasePower", newRecord.BasePower);
            command.AddParameter("PowerFormat", newRecord.PowerFormat);
            command.AddParameter("BaseToughness", newRecord.BaseToughness);
            command.AddParameter("ToughnessFormat", newRecord.ToughnessFormat);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<PowerToughnessRecord> NewRecordAsync(DbCommand command, Guid faceId, int basePower, string powerFormat, int baseToughness, string toughnessFormat, CancellationToken cancellationToken)
        {
            var newRecord = PowerToughnessRecord.Create(DatabaseContext, this, faceId, basePower, powerFormat, baseToughness, toughnessFormat);
            command.CommandText = "INSERT INTO [Cards].[PowerToughness] ( FaceId, BasePower, PowerFormat, BaseToughness, ToughnessFormat ) VALUES ( @FaceId, @BasePower, @PowerFormat, @BaseToughness, @ToughnessFormat );";
            command.AddParameter("FaceId", newRecord.FaceId);
            command.AddParameter("BasePower", newRecord.BasePower);
            command.AddParameter("PowerFormat", newRecord.PowerFormat);
            command.AddParameter("BaseToughness", newRecord.BaseToughness);
            command.AddParameter("ToughnessFormat", newRecord.ToughnessFormat);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override PowerToughnessRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var basePower = Convert.ToInt32(dbDataReader["BasePower"]);
            var powerFormat = (string)dbDataReader["PowerFormat"];
            var baseToughness = Convert.ToInt32(dbDataReader["BaseToughness"]);
            var toughnessFormat = (string)dbDataReader["ToughnessFormat"];
            return PowerToughnessRecord.Create(DatabaseContext, this, faceId, basePower, powerFormat, baseToughness, toughnessFormat);
        }
    }
}