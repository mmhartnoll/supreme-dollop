using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class CardTypeRecord : DatabaseRecord<CardTypeRecord>
    {
        public Guid Id { get; }
        public string Value { get; }

        private CardTypeRecord(DatabaseContext databaseContext, CardTypesTable cardTypesTable, Guid id, string value) : base(databaseContext, cardTypesTable)
        {
            Id = id;
            Value = value;
        }

        internal static CardTypeRecord Create(DatabaseContext databaseContext, CardTypesTable cardTypesTable, Guid id, string value)
        {
            return new CardTypeRecord(databaseContext, cardTypesTable, id, value);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[CardTypes] SET Value = @Value WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Value", Value);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[CardTypes] SET Value = @Value WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Value", Value);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[CardTypes] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[CardTypes] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}