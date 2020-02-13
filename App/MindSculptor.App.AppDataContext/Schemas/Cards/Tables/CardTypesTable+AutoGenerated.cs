using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class CardTypesTable : DataContextTable<CardTypeRecord, CardTypeRecordExpression>
    {
        private CardTypesTable(DataContext dataContext) : base(dataContext, "Cards", "CardTypes")
        {
        }

        internal static CardTypesTable Create(DataContext dataContext)
        {
            return new CardTypesTable(dataContext);
        }

        public CardTypeRecord NewRecord(string value)
        {
            var newRecord = CardTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[CardTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<CardTypeRecord> NewRecordAsync(string value)
        {
            var newRecord = CardTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[CardTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override CardTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            return CardTypeRecord.Create(DataContext, id, value);
        }
    }
}