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
    public class CardsTable : DataContextTable<CardRecord, CardRecordExpression>
    {
        private CardsTable(DataContext dataContext) : base(dataContext, "Mtga", "Cards")
        {
        }

        internal static CardsTable Create(DataContext dataContext)
        {
            return new CardsTable(dataContext);
        }

        public CardRecord NewRecord(BasePrintingRecord basePrintingRecord, int mtgaCardId)
        {
            var newRecord = CardRecord.Create(DataContext, basePrintingRecord.Id, mtgaCardId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[Cards] ( Id, MtgaCardId ) VALUES ( @Id, @MtgaCardId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("MtgaCardId", newRecord.MtgaCardId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<CardRecord> NewRecordAsync(BasePrintingRecord basePrintingRecord, int mtgaCardId)
        {
            var newRecord = CardRecord.Create(DataContext, basePrintingRecord.Id, mtgaCardId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[Cards] ( Id, MtgaCardId ) VALUES ( @Id, @MtgaCardId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("MtgaCardId", newRecord.MtgaCardId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override CardRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var mtgaCardId = Convert.ToInt32(dbDataReader["MtgaCardId"]);
            return CardRecord.Create(DataContext, id, mtgaCardId);
        }
    }
}