using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class OracleTextTable : DataContextTable<OracleTextRecord, OracleTextRecordExpression>
    {
        private OracleTextTable(DataContext dataContext) : base(dataContext, "Cards", "OracleText")
        {
        }

        internal static OracleTextTable Create(DataContext dataContext)
        {
            return new OracleTextTable(dataContext);
        }

        public OracleTextRecord NewRecord(FaceRecord faceRecord, string value)
        {
            var newRecord = OracleTextRecord.Create(DataContext, faceRecord.Id, value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[OracleText] ( FaceId, Value ) VALUES ( @FaceId, @Value );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("Value", newRecord.Value);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<OracleTextRecord> NewRecordAsync(FaceRecord faceRecord, string value)
        {
            var newRecord = OracleTextRecord.Create(DataContext, faceRecord.Id, value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[OracleText] ( FaceId, Value ) VALUES ( @FaceId, @Value );";
                command.AddParameter("FaceId", newRecord.FaceId);
                command.AddParameter("Value", newRecord.Value);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override OracleTextRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var faceId = (Guid)dbDataReader["FaceId"];
            var value = (string)dbDataReader["Value"];
            return OracleTextRecord.Create(DataContext, faceId, value);
        }
    }
}