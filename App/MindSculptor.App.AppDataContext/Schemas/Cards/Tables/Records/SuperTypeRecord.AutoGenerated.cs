using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class SuperTypeRecord : DatabaseRecord<SuperTypeRecord>
    {
        public Guid Id { get; }
        public string Value { get; }

        private SuperTypeRecord(DatabaseContext databaseContext, SuperTypesTable superTypesTable, Guid id, string value) : base(databaseContext, superTypesTable)
        {
            Id = id;
            Value = value;
        }

        internal static SuperTypeRecord Create(DatabaseContext databaseContext, SuperTypesTable superTypesTable, Guid id, string value)
        {
            return new SuperTypeRecord(databaseContext, superTypesTable, id, value);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[SuperTypes] SET Value = @Value WHERE Id = @Id;";
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
                command.CommandText = "UPDATE [Cards].[SuperTypes] SET Value = @Value WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Value", Value);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[SuperTypes] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[SuperTypes] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}