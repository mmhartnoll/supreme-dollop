using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class SubTypesTable : DataContextTable<SubTypeRecord, SubTypeRecordExpression>
    {
        private SubTypesTable(DataContext dataContext) : base(dataContext, "Cards", "SubTypes")
        {
        }

        internal static SubTypesTable Create(DataContext dataContext)
        {
            return new SubTypesTable(dataContext);
        }

        public SubTypeRecord NewRecord(string value)
        {
            var newRecord = SubTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[SubTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<SubTypeRecord> NewRecordAsync(string value)
        {
            var newRecord = SubTypeRecord.Create(DataContext, Guid.NewGuid(), value);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[SubTypes] ( Id, Value ) VALUES ( @Id, @Value );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override SubTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            return SubTypeRecord.Create(DataContext, id, value);
        }
    }
}