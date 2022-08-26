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
    public class BasesTable : DatabaseTable<BaseRecord, BaseRecordExpression>
    {
        private BasesTable(DatabaseContext dataContext) : base(dataContext, "Cards", "Bases")
        {
        }

        internal static BasesTable Create(DatabaseContext dataContext)
        {
            return new BasesTable(dataContext);
        }

        public BaseRecord NewRecord(Guid cardTypeId)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), cardTypeId));
        }

        public async Task<BaseRecord> NewRecordAsync(Guid cardTypeId, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), cardTypeId, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public BaseRecord NewRecord(CardTypeRecord cardTypeRecord)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), cardTypeRecord.Id));
        }

        public async Task<BaseRecord> NewRecordAsync(CardTypeRecord cardTypeRecord, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), cardTypeRecord.Id, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private BaseRecord NewRecord(DbCommand command, Guid id, Guid cardTypeId)
        {
            var newRecord = BaseRecord.Create(Context, this, id, cardTypeId);
            command.CommandText = "INSERT INTO [Cards].[Bases] ( Id, CardTypeId ) VALUES ( @Id, @CardTypeId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("CardTypeId", newRecord.CardTypeId);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<BaseRecord> NewRecordAsync(DbCommand command, Guid id, Guid cardTypeId, CancellationToken cancellationToken)
        {
            var newRecord = BaseRecord.Create(Context, this, id, cardTypeId);
            command.CommandText = "INSERT INTO [Cards].[Bases] ( Id, CardTypeId ) VALUES ( @Id, @CardTypeId );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("CardTypeId", newRecord.CardTypeId);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override BaseRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var cardTypeId = (Guid)dbDataReader["CardTypeId"];
            return BaseRecord.Create(Context, this, id, cardTypeId);
        }
    }
}