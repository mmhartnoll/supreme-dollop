using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class SuperTypesTable : DataContextTable<SuperTypeRecord, SuperTypeRecordExpression>
    {
        private SuperTypesTable(DataContext dataContext) : base(dataContext, "Cards", "SuperTypes")
        {
        }

        internal static SuperTypesTable Create(DataContext dataContext)
        {
            return new SuperTypesTable(dataContext);
        }

        public SuperTypeRecord NewRecord(string value)
        {
            var newRecord = SuperTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[SuperTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<SuperTypeRecord> NewRecordAsync(string value)
        {
            var newRecord = SuperTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[SuperTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override SuperTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            return SuperTypeRecord.Create(DataContext, id, value);
        }
    }
}