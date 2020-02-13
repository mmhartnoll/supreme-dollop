using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class FlavorTextTable : DataContextTable<FlavorTextRecord, FlavorTextRecordExpression>
    {
        private FlavorTextTable(DataContext dataContext) : base(dataContext, "Cards", "FlavorText")
        {
        }

        internal static FlavorTextTable Create(DataContext dataContext)
        {
            return new FlavorTextTable(dataContext);
        }

        public FlavorTextRecord NewRecord(FacePrintingRecord facePrintingRecord, string value)
        {
            var newRecord = FlavorTextRecord.Create(DataContext, facePrintingRecord.Id, value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FlavorText] ( FacePrintingId, Value ) VALUES ( @FacePrintingId, @Value );";
                command.AddParameter("FacePrintingId", newRecord.FacePrintingId);
                command.AddParameter("Value", newRecord.Value);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<FlavorTextRecord> NewRecordAsync(FacePrintingRecord facePrintingRecord, string value)
        {
            var newRecord = FlavorTextRecord.Create(DataContext, facePrintingRecord.Id, value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[FlavorText] ( FacePrintingId, Value ) VALUES ( @FacePrintingId, @Value );";
                command.AddParameter("FacePrintingId", newRecord.FacePrintingId);
                command.AddParameter("Value", newRecord.Value);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override FlavorTextRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var facePrintingId = (Guid)dbDataReader["FacePrintingId"];
            var value = (string)dbDataReader["Value"];
            return FlavorTextRecord.Create(DataContext, facePrintingId, value);
        }
    }
}