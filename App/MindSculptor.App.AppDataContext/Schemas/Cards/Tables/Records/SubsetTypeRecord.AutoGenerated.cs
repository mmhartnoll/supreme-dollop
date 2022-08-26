using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class SubsetTypeRecord : DatabaseRecord<SubsetTypeRecord>
    {
        public Guid Id { get; }
        public string Value { get; }
        public string CollectorsNumberFormat { get; }

        private SubsetTypeRecord(DatabaseContext dataContext, SubsetTypesTable subsetTypesTable, Guid id, string value, string collectorsNumberFormat) : base(dataContext, subsetTypesTable)
        {
            Id = id;
            Value = value;
            CollectorsNumberFormat = collectorsNumberFormat;
        }

        internal static SubsetTypeRecord Create(DatabaseContext dataContext, SubsetTypesTable subsetTypesTable, Guid id, string value, string collectorsNumberFormat)
        {
            return new SubsetTypeRecord(dataContext, subsetTypesTable, id, value, collectorsNumberFormat);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[SubsetTypes] SET Value = @Value, CollectorsNumberFormat = @CollectorsNumberFormat WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Value", Value);
                command.AddParameter("CollectorsNumberFormat", CollectorsNumberFormat);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[SubsetTypes] SET Value = @Value, CollectorsNumberFormat = @CollectorsNumberFormat WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("Value", Value);
                command.AddParameter("CollectorsNumberFormat", CollectorsNumberFormat);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[SubsetTypes] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[SubsetTypes] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}