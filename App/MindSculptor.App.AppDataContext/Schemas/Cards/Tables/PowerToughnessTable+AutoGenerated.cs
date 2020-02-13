using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class PowerToughnessTable : DataContextTable<PowerToughnessRecord, PowerToughnessRecordExpression>
    {
        private PowerToughnessTable(DataContext dataContext) : base(dataContext, "Cards", "PowerToughness")
        {
        }

        internal static PowerToughnessTable Create(DataContext dataContext)
        {
            return new PowerToughnessTable(dataContext);
        }

        public PowerToughnessRecord NewRecord(FaceRecord faceRecord, int basePower, string powerFormat, int baseToughness, string toughnessFormat)
        {
            var newRecord = PowerToughnessRecord.Create(DataContext, faceRecord.Id, basePower, powerFormat, baseToughness, toughnessFormat);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[PowerToughness] ( FaceId, BasePower, PowerFormat, BaseToughness, ToughnessFormat ) VALUES ( @FaceId, @BasePower, @PowerFormat, @BaseToughness, @ToughnessFormat );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("BasePower", newRecord.BasePower);
                command.AddParameter("PowerFormat", newRecord.PowerFormat);
                command.AddParameter("BaseToughness", newRecord.BaseToughness);
                command.AddParameter("ToughnessFormat", newRecord.ToughnessFormat);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<PowerToughnessRecord> NewRecordAsync(FaceRecord faceRecord, int basePower, string powerFormat, int baseToughness, string toughnessFormat)
        {
            var newRecord = PowerToughnessRecord.Create(DataContext, faceRecord.Id, basePower, powerFormat, baseToughness, toughnessFormat);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[PowerToughness] ( FaceId, BasePower, PowerFormat, BaseToughness, ToughnessFormat ) VALUES ( @FaceId, @BasePower, @PowerFormat, @BaseToughness, @ToughnessFormat );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("BasePower", newRecord.BasePower);
                command.AddParameter("PowerFormat", newRecord.PowerFormat);
                command.AddParameter("BaseToughness", newRecord.BaseToughness);
                command.AddParameter("ToughnessFormat", newRecord.ToughnessFormat);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override PowerToughnessRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var basePower = Convert.ToInt32(dbDataReader["BasePower"]);
            var powerFormat = (string)dbDataReader["PowerFormat"];
            var baseToughness = Convert.ToInt32(dbDataReader["BaseToughness"]);
            var toughnessFormat = (string)dbDataReader["ToughnessFormat"];
            return PowerToughnessRecord.Create(DataContext, faceId, basePower, powerFormat, baseToughness, toughnessFormat);
        }
    }
}