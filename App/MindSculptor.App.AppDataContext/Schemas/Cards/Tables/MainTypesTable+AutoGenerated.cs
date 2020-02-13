using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class MainTypesTable : DataContextTable<MainTypeRecord, MainTypeRecordExpression>
    {
        private MainTypesTable(DataContext dataContext) : base(dataContext, "Cards", "MainTypes")
        {
        }

        internal static MainTypesTable Create(DataContext dataContext)
        {
            return new MainTypesTable(dataContext);
        }

        public MainTypeRecord NewRecord(string value)
        {
            var newRecord = MainTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[MainTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<MainTypeRecord> NewRecordAsync(string value)
        {
            var newRecord = MainTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[MainTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override MainTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            return MainTypeRecord.Create(DataContext, id, value);
        }
    }
}