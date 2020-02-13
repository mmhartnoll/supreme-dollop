using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class PrintingTypesTable : DataContextTable<PrintingTypeRecord, PrintingTypeRecordExpression>
    {
        private PrintingTypesTable(DataContext dataContext) : base(dataContext, "Cards", "PrintingTypes")
        {
        }

        internal static PrintingTypesTable Create(DataContext dataContext)
        {
            return new PrintingTypesTable(dataContext);
        }

        public PrintingTypeRecord NewRecord(string value)
        {
            var newRecord = PrintingTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[PrintingTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<PrintingTypeRecord> NewRecordAsync(string value)
        {
            var newRecord = PrintingTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[PrintingTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override PrintingTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            return PrintingTypeRecord.Create(DataContext, id, value);
        }
    }
}