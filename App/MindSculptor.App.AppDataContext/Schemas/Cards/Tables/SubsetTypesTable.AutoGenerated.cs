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
    public class SubsetTypesTable : DatabaseTable<SubsetTypeRecord, SubsetTypeRecordExpression>
    {
        private SubsetTypesTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "SubsetTypes")
        {
        }

        internal static SubsetTypesTable Create(DatabaseContext databaseContext)
        {
            return new SubsetTypesTable(databaseContext);
        }

        public SubsetTypeRecord NewRecord(string value, string collectorsNumberFormat)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), value, collectorsNumberFormat));
        }

        public async Task<SubsetTypeRecord> NewRecordAsync(string value, string collectorsNumberFormat, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), value, collectorsNumberFormat, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private SubsetTypeRecord NewRecord(DbCommand command, Guid id, string value, string collectorsNumberFormat)
        {
            var newRecord = SubsetTypeRecord.Create(DatabaseContext, this, id, value, collectorsNumberFormat);
            command.CommandText = "INSERT INTO [Cards].[SubsetTypes] ( Id, Value, CollectorsNumberFormat ) VALUES ( @Id, @Value, @CollectorsNumberFormat );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            command.AddParameter("CollectorsNumberFormat", newRecord.CollectorsNumberFormat);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<SubsetTypeRecord> NewRecordAsync(DbCommand command, Guid id, string value, string collectorsNumberFormat, CancellationToken cancellationToken)
        {
            var newRecord = SubsetTypeRecord.Create(DatabaseContext, this, id, value, collectorsNumberFormat);
            command.CommandText = "INSERT INTO [Cards].[SubsetTypes] ( Id, Value, CollectorsNumberFormat ) VALUES ( @Id, @Value, @CollectorsNumberFormat );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            command.AddParameter("CollectorsNumberFormat", newRecord.CollectorsNumberFormat);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override SubsetTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            var collectorsNumberFormat = (string)dbDataReader["CollectorsNumberFormat"];
            return SubsetTypeRecord.Create(DatabaseContext, this, id, value, collectorsNumberFormat);
        }
    }
}