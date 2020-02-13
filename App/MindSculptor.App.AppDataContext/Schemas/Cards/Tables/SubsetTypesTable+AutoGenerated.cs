using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class SubsetTypesTable : DataContextTable<SubsetTypeRecord, SubsetTypeRecordExpression>
    {
        private SubsetTypesTable(DataContext dataContext) : base(dataContext, "Cards", "SubsetTypes")
        {
        }

        internal static SubsetTypesTable Create(DataContext dataContext)
        {
            return new SubsetTypesTable(dataContext);
        }

        public SubsetTypeRecord NewRecord(string value, string collectorsNumberFormat)
        {
            var newRecord = SubsetTypeRecord.Create(DataContext, Guid.NewGuid(), value, collectorsNumberFormat);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[SubsetTypes] ( Id, Value, CollectorsNumberFormat ) VALUES ( @Id, @Value, @CollectorsNumberFormat );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                command.AddParameter("CollectorsNumberFormat", newRecord.CollectorsNumberFormat);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<SubsetTypeRecord> NewRecordAsync(string value, string collectorsNumberFormat)
        {
            var newRecord = SubsetTypeRecord.Create(DataContext, Guid.NewGuid(), value, collectorsNumberFormat);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[SubsetTypes] ( Id, Value, CollectorsNumberFormat ) VALUES ( @Id, @Value, @CollectorsNumberFormat );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("Value", newRecord.Value);
                command.AddParameter("CollectorsNumberFormat", newRecord.CollectorsNumberFormat);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override SubsetTypeRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var value = (string)dbDataReader["Value"];
            var collectorsNumberFormat = (string)dbDataReader["CollectorsNumberFormat"];
            return SubsetTypeRecord.Create(DataContext, id, value, collectorsNumberFormat);
        }
    }
}