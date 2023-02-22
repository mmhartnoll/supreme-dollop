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
    public class MainTypesTable : DatabaseTable<MainTypeRecord, MainTypeRecordExpression>
    {
        private MainTypesTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "MainTypes")
        {
        }

        internal static MainTypesTable Create(DatabaseContext databaseContext)
        {
            return new MainTypesTable(databaseContext);
        }

        public MainTypeRecord NewRecord(string value)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), value));
        }

        public async Task<MainTypeRecord> NewRecordAsync(string value, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), value, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private MainTypeRecord NewRecord(DbCommand command, Guid id, string value)
        {
            var newRecord = MainTypeRecord.Create(DatabaseContext, this, id, value);
            command.CommandText = "INSERT INTO [Cards].[MainTypes] ( Id, Value ) VALUES ( @Id, @Value );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<MainTypeRecord> NewRecordAsync(DbCommand command, Guid id, string value, CancellationToken cancellationToken)
        {
            var newRecord = MainTypeRecord.Create(DatabaseContext, this, id, value);
            command.CommandText = "INSERT INTO [Cards].[MainTypes] ( Id, Value ) VALUES ( @Id, @Value );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override MainTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            return MainTypeRecord.Create(DatabaseContext, this, id, value);
        }
    }
}