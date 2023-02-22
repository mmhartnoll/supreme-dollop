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
    public class SuperTypesTable : DatabaseTable<SuperTypeRecord, SuperTypeRecordExpression>
    {
        private SuperTypesTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "SuperTypes")
        {
        }

        internal static SuperTypesTable Create(DatabaseContext databaseContext)
        {
            return new SuperTypesTable(databaseContext);
        }

        public SuperTypeRecord NewRecord(string value)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), value));
        }

        public async Task<SuperTypeRecord> NewRecordAsync(string value, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), value, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private SuperTypeRecord NewRecord(DbCommand command, Guid id, string value)
        {
            var newRecord = SuperTypeRecord.Create(DatabaseContext, this, id, value);
            command.CommandText = "INSERT INTO [Cards].[SuperTypes] ( Id, Value ) VALUES ( @Id, @Value );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<SuperTypeRecord> NewRecordAsync(DbCommand command, Guid id, string value, CancellationToken cancellationToken)
        {
            var newRecord = SuperTypeRecord.Create(DatabaseContext, this, id, value);
            command.CommandText = "INSERT INTO [Cards].[SuperTypes] ( Id, Value ) VALUES ( @Id, @Value );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override SuperTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            return SuperTypeRecord.Create(DatabaseContext, this, id, value);
        }
    }
}