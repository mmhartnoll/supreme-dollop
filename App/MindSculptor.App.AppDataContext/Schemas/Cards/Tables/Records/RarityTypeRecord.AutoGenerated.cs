using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class RarityTypeRecord : DatabaseRecord<RarityTypeRecord>
    {
        public Guid Id { get; }
        public string Value { get; }

        private RarityTypeRecord(DatabaseContext dataContext, RarityTypesTable rarityTypesTable, Guid id, string value) : base(dataContext, rarityTypesTable)
        {
            Id = id;
            Value = value;
        }

        internal static RarityTypeRecord Create(DatabaseContext dataContext, RarityTypesTable rarityTypesTable, Guid id, string value)
        {
            return new RarityTypeRecord(dataContext, rarityTypesTable, id, value);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[RarityTypes] SET Value = @Value WHERE Id = @Id;";
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
                command.CommandText = "UPDATE [Cards].[RarityTypes] SET Value = @Value WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Value", Value);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[RarityTypes] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[RarityTypes] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}