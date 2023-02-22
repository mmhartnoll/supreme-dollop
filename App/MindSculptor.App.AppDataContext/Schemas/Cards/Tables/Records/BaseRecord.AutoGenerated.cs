using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class BaseRecord : DatabaseRecord<BaseRecord>
    {
        public Guid Id { get; }
        public Guid CardTypeId { get; }

        private BaseRecord(DatabaseContext databaseContext, BasesTable basesTable, Guid id, Guid cardTypeId) : base(databaseContext, basesTable)
        {
            Id = id;
            CardTypeId = cardTypeId;
        }

        internal static BaseRecord Create(DatabaseContext databaseContext, BasesTable basesTable, Guid id, Guid cardTypeId)
        {
            return new BaseRecord(databaseContext, basesTable, id, cardTypeId);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Bases] SET CardTypeId = @CardTypeId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("CardTypeId", CardTypeId);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[Bases] SET CardTypeId = @CardTypeId WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("CardTypeId", CardTypeId);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[Bases] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[Bases] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}