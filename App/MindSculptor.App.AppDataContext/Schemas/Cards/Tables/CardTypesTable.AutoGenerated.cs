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
    public class CardTypesTable : DatabaseTable<CardTypeRecord, CardTypeRecordExpression>
    {
        private CardTypesTable(DatabaseContext dataContext) : base(dataContext, "Cards", "CardTypes")
        {
        }

        internal static CardTypesTable Create(DatabaseContext dataContext)
        {
            return new CardTypesTable(dataContext);
        }

        public CardTypeRecord NewRecord(string value)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), value));
        }

        public async Task<CardTypeRecord> NewRecordAsync(string value, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), value, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private CardTypeRecord NewRecord(DbCommand command, Guid id, string value)
        {
            var newRecord = CardTypeRecord.Create(Context, this, id, value);
            command.CommandText = "INSERT INTO [Cards].[CardTypes] ( Id, Value ) VALUES ( @Id, @Value );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<CardTypeRecord> NewRecordAsync(DbCommand command, Guid id, string value, CancellationToken cancellationToken)
        {
            var newRecord = CardTypeRecord.Create(Context, this, id, value);
            command.CommandText = "INSERT INTO [Cards].[CardTypes] ( Id, Value ) VALUES ( @Id, @Value );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override CardTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            return CardTypeRecord.Create(Context, this, id, value);
        }
    }
}