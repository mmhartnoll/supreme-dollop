using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class SetInclusionsTable : DataContextTable<SetInclusionRecord, SetInclusionRecordExpression>
    {
        private SetInclusionsTable(DataContext dataContext) : base(dataContext, "Cards", "SetInclusions")
        {
        }

        internal static SetInclusionsTable Create(DataContext dataContext)
        {
            return new SetInclusionsTable(dataContext);
        }

        public SetInclusionRecord NewRecord(SetRecord setRecord, SubsetTypeRecord subsetTypeRecord, BaseRecord baseRecord, RarityTypeRecord rarityTypeRecord, int logicalOrdinal, int? collectorsNumber)
        {
            var newRecord = SetInclusionRecord.Create(DataContext, Guid.NewGuid(), setRecord.Id, subsetTypeRecord.Id, baseRecord.Id, rarityTypeRecord.Id, logicalOrdinal, collectorsNumber);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[SetInclusions] ( Id, SetId, SubsetTypeId, BaseId, RarityTypeId, LogicalOrdinal, CollectorsNumber ) VALUES ( @Id, @SetId, @SubsetTypeId, @BaseId, @RarityTypeId, @LogicalOrdinal, @CollectorsNumber );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("SetId", newRecord.SetId);
                command.AddParameter("SubsetTypeId", newRecord.SubsetTypeId);
                command.AddParameter("BaseId", newRecord.BaseId);
                command.AddParameter("RarityTypeId", newRecord.RarityTypeId);
                command.AddParameter("LogicalOrdinal", newRecord.LogicalOrdinal);
                command.AddParameter("CollectorsNumber", newRecord.CollectorsNumber);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<SetInclusionRecord> NewRecordAsync(SetRecord setRecord, SubsetTypeRecord subsetTypeRecord, BaseRecord baseRecord, RarityTypeRecord rarityTypeRecord, int logicalOrdinal, int? collectorsNumber)
        {
            var newRecord = SetInclusionRecord.Create(DataContext, Guid.NewGuid(), setRecord.Id, subsetTypeRecord.Id, baseRecord.Id, rarityTypeRecord.Id, logicalOrdinal, collectorsNumber);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[SetInclusions] ( Id, SetId, SubsetTypeId, BaseId, RarityTypeId, LogicalOrdinal, CollectorsNumber ) VALUES ( @Id, @SetId, @SubsetTypeId, @BaseId, @RarityTypeId, @LogicalOrdinal, @CollectorsNumber );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("SetId", newRecord.SetId);
                command.AddParameter("SubsetTypeId", newRecord.SubsetTypeId);
                command.AddParameter("BaseId", newRecord.BaseId);
                command.AddParameter("RarityTypeId", newRecord.RarityTypeId);
                command.AddParameter("LogicalOrdinal", newRecord.LogicalOrdinal);
                command.AddParameter("CollectorsNumber", newRecord.CollectorsNumber);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override SetInclusionRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var setId = (Guid)dbDataReader["SetId"];
            var subsetTypeId = (Guid)dbDataReader["SubsetTypeId"];
            var baseId = (Guid)dbDataReader["BaseId"];
            var rarityTypeId = (Guid)dbDataReader["RarityTypeId"];
            var logicalOrdinal = Convert.ToInt32(dbDataReader["LogicalOrdinal"]);
            var collectorsNumber = dbDataReader["CollectorsNumber"] == DBNull.Value ? null : (int?)Convert.ToInt32(dbDataReader["CollectorsNumber"]);
            return SetInclusionRecord.Create(DataContext, id, setId, subsetTypeId, baseId, rarityTypeId, logicalOrdinal, collectorsNumber);
        }
    }
}