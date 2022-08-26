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
    public class PrintingTypesTable : DatabaseTable<PrintingTypeRecord, PrintingTypeRecordExpression>
    {
        private PrintingTypesTable(DatabaseContext dataContext) : base(dataContext, "Cards", "PrintingTypes")
        {
        }

        internal static PrintingTypesTable Create(DatabaseContext dataContext)
        {
            return new PrintingTypesTable(dataContext);
        }

        public PrintingTypeRecord NewRecord(string value)
        {
            return Context.Execute(command => NewRecord(command, Guid.NewGuid(), value));
        }

        public async Task<PrintingTypeRecord> NewRecordAsync(string value, CancellationToken cancellationToken = default)
        {
            return await Context.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), value, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private PrintingTypeRecord NewRecord(DbCommand command, Guid id, string value)
        {
            var newRecord = PrintingTypeRecord.Create(Context, this, id, value);
            command.CommandText = "INSERT INTO [Cards].[PrintingTypes] ( Id, Value ) VALUES ( @Id, @Value );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<PrintingTypeRecord> NewRecordAsync(DbCommand command, Guid id, string value, CancellationToken cancellationToken)
        {
            var newRecord = PrintingTypeRecord.Create(Context, this, id, value);
            command.CommandText = "INSERT INTO [Cards].[PrintingTypes] ( Id, Value ) VALUES ( @Id, @Value );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("Value", newRecord.Value);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        protected override PrintingTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            return PrintingTypeRecord.Create(Context, this, id, value);
        }
    }
}