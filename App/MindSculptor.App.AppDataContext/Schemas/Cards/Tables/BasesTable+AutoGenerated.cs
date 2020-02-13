using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class BasesTable : DataContextTable<BaseRecord, BaseRecordExpression>
    {
        private BasesTable(DataContext dataContext) : base(dataContext, "Cards", "Bases")
        {
        }

        internal static BasesTable Create(DataContext dataContext)
        {
            return new BasesTable(dataContext);
        }

        public BaseRecord NewRecord(CardTypeRecord cardTypeRecord)
        {
            var newRecord = BaseRecord.Create(DataContext, Guid.NewGuid(), cardTypeRecord.Id);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[Bases] ( Id, CardTypeId ) VALUES ( @Id, @CardTypeId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("CardTypeId", newRecord.CardTypeId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<BaseRecord> NewRecordAsync(CardTypeRecord cardTypeRecord)
        {
            var newRecord = BaseRecord.Create(DataContext, Guid.NewGuid(), cardTypeRecord.Id);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[Bases] ( Id, CardTypeId ) VALUES ( @Id, @CardTypeId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("CardTypeId", newRecord.CardTypeId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override BaseRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var cardTypeId = (Guid)dbDataReader["CardTypeId"];
            return BaseRecord.Create(DataContext, id, cardTypeId);
        }
    }
}