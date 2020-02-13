using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class BoostersTable : DataContextTable<BoosterRecord, BoosterRecordExpression>
    {
        private BoostersTable(DataContext dataContext) : base(dataContext, "Mtga", "Boosters")
        {
        }

        internal static BoostersTable Create(DataContext dataContext)
        {
            return new BoostersTable(dataContext);
        }

        public BoosterRecord NewRecord(SetRecord setRecord, int mtgaBoosterId)
        {
            var newRecord = BoosterRecord.Create(DataContext, setRecord.Id, mtgaBoosterId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[Boosters] ( SetId, MtgaBoosterId ) VALUES ( @SetId, @MtgaBoosterId );";
                command.AddParameter("SetId", newRecord.SetId);
                command.AddParameter("MtgaBoosterId", newRecord.MtgaBoosterId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<BoosterRecord> NewRecordAsync(SetRecord setRecord, int mtgaBoosterId)
        {
            var newRecord = BoosterRecord.Create(DataContext, setRecord.Id, mtgaBoosterId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[Boosters] ( SetId, MtgaBoosterId ) VALUES ( @SetId, @MtgaBoosterId );";
                command.AddParameter("SetId", newRecord.SetId);
                command.AddParameter("MtgaBoosterId", newRecord.MtgaBoosterId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override BoosterRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var setId = (Guid)dbDataReader["SetId"];
            var mtgaBoosterId = Convert.ToInt32(dbDataReader["MtgaBoosterId"]);
            return BoosterRecord.Create(DataContext, setId, mtgaBoosterId);
        }
    }
}