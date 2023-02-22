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
    public class SetInclusionsTable : DatabaseTable<SetInclusionRecord, SetInclusionRecordExpression>
    {
        private SetInclusionsTable(DatabaseContext databaseContext) : base(databaseContext, "Cards", "SetInclusions")
        {
        }

        internal static SetInclusionsTable Create(DatabaseContext databaseContext)
        {
            return new SetInclusionsTable(databaseContext);
        }

        public SetInclusionRecord NewRecord(Guid setId, Guid subsetTypeId, Guid baseId, Guid rarityTypeId, int logicalOrdinal, int? collectorsNumber)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), setId, subsetTypeId, baseId, rarityTypeId, logicalOrdinal, collectorsNumber));
        }

        public async Task<SetInclusionRecord> NewRecordAsync(Guid setId, Guid subsetTypeId, Guid baseId, Guid rarityTypeId, int logicalOrdinal, int? collectorsNumber, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), setId, subsetTypeId, baseId, rarityTypeId, logicalOrdinal, collectorsNumber, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        public SetInclusionRecord NewRecord(SetRecord setRecord, SubsetTypeRecord subsetTypeRecord, BaseRecord baseRecord, RarityTypeRecord rarityTypeRecord, int logicalOrdinal, int? collectorsNumber)
        {
            return DatabaseContext.Execute(command => NewRecord(command, Guid.NewGuid(), setRecord.Id, subsetTypeRecord.Id, baseRecord.Id, rarityTypeRecord.Id, logicalOrdinal, collectorsNumber));
        }

        public async Task<SetInclusionRecord> NewRecordAsync(SetRecord setRecord, SubsetTypeRecord subsetTypeRecord, BaseRecord baseRecord, RarityTypeRecord rarityTypeRecord, int logicalOrdinal, int? collectorsNumber, CancellationToken cancellationToken = default)
        {
            return await DatabaseContext.ExecuteAsync((command, cancellationToken) => NewRecordAsync(command, Guid.NewGuid(), setRecord.Id, subsetTypeRecord.Id, baseRecord.Id, rarityTypeRecord.Id, logicalOrdinal, collectorsNumber, cancellationToken), cancellationToken).ConfigureAwait(false);
        }

        private SetInclusionRecord NewRecord(DbCommand command, Guid id, Guid setId, Guid subsetTypeId, Guid baseId, Guid rarityTypeId, int logicalOrdinal, int? collectorsNumber)
        {
            var newRecord = SetInclusionRecord.Create(DatabaseContext, this, id, setId, subsetTypeId, baseId, rarityTypeId, logicalOrdinal, collectorsNumber);
            command.CommandText = "INSERT INTO [Cards].[SetInclusions] ( Id, SetId, SubsetTypeId, BaseId, RarityTypeId, LogicalOrdinal, CollectorsNumber ) VALUES ( @Id, @SetId, @SubsetTypeId, @BaseId, @RarityTypeId, @LogicalOrdinal, @CollectorsNumber );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("SetId", newRecord.SetId);
            command.AddParameter("SubsetTypeId", newRecord.SubsetTypeId);
            command.AddParameter("BaseId", newRecord.BaseId);
            command.AddParameter("RarityTypeId", newRecord.RarityTypeId);
            command.AddParameter("LogicalOrdinal", newRecord.LogicalOrdinal);
            command.AddParameter("CollectorsNumber", newRecord.CollectorsNumber);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Create, newRecord);
            return newRecord;
        }

        private async Task<SetInclusionRecord> NewRecordAsync(DbCommand command, Guid id, Guid setId, Guid subsetTypeId, Guid baseId, Guid rarityTypeId, int logicalOrdinal, int? collectorsNumber, CancellationToken cancellationToken)
        {
            var newRecord = SetInclusionRecord.Create(DatabaseContext, this, id, setId, subsetTypeId, baseId, rarityTypeId, logicalOrdinal, collectorsNumber);
            command.CommandText = "INSERT INTO [Cards].[SetInclusions] ( Id, SetId, SubsetTypeId, BaseId, RarityTypeId, LogicalOrdinal, CollectorsNumber ) VALUES ( @Id, @SetId, @SubsetTypeId, @BaseId, @RarityTypeId, @LogicalOrdinal, @CollectorsNumber );";
            command.AddParameter("Id", newRecord.Id);
            command.AddParameter("SetId", newRecord.SetId);
            command.AddParameter("SubsetTypeId", newRecord.SubsetTypeId);
            command.AddParameter("BaseId", newRecord.BaseId);
            command.AddParameter("RarityTypeId", newRecord.RarityTypeId);
            command.AddParameter("LogicalOrdinal", newRecord.LogicalOrdinal);
            command.AddParameter("CollectorsNumber", newRecord.CollectorsNumber);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Create, newRecord);
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
            return SetInclusionRecord.Create(DatabaseContext, this, id, setId, subsetTypeId, baseId, rarityTypeId, logicalOrdinal, collectorsNumber);
        }
    }
}