using MindSculptor.DataAccess.Context;
using MindSculptor.DataAccess.Context.Events;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class SetInclusionRecord : DatabaseRecord<SetInclusionRecord>
    {
        public Guid Id { get; }
        public Guid SetId { get; }
        public Guid SubsetTypeId { get; }
        public Guid BaseId { get; }
        public Guid RarityTypeId { get; }
        public int LogicalOrdinal { get; }
        public int? CollectorsNumber { get; }

        private SetInclusionRecord(DatabaseContext dataContext, SetInclusionsTable setInclusionsTable, Guid id, Guid setId, Guid subsetTypeId, Guid baseId, Guid rarityTypeId, int logicalOrdinal, int? collectorsNumber) : base(dataContext, setInclusionsTable)
        {
            Id = id;
            SetId = setId;
            SubsetTypeId = subsetTypeId;
            BaseId = baseId;
            RarityTypeId = rarityTypeId;
            LogicalOrdinal = logicalOrdinal;
            CollectorsNumber = collectorsNumber;
        }

        internal static SetInclusionRecord Create(DatabaseContext dataContext, SetInclusionsTable setInclusionsTable, Guid id, Guid setId, Guid subsetTypeId, Guid baseId, Guid rarityTypeId, int logicalOrdinal, int? collectorsNumber)
        {
            return new SetInclusionRecord(dataContext, setInclusionsTable, id, setId, subsetTypeId, baseId, rarityTypeId, logicalOrdinal, collectorsNumber);
        }

        protected override void UpdateRecord(DbCommand command)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[SetInclusions] SET SetId = @SetId, SubsetTypeId = @SubsetTypeId, BaseId = @BaseId, RarityTypeId = @RarityTypeId, LogicalOrdinal = @LogicalOrdinal, CollectorsNumber = @CollectorsNumber WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("SetId", SetId);
                command.AddParameter("SubsetTypeId", SubsetTypeId);
                command.AddParameter("BaseId", BaseId);
                command.AddParameter("RarityTypeId", RarityTypeId);
                command.AddParameter("LogicalOrdinal", LogicalOrdinal);
                command.AddParameter("CollectorsNumber", CollectorsNumber);
                command.ExecuteNonQuery();
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected async override Task UpdateRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            if (IsModified)
            {
                command.CommandText = "UPDATE [Cards].[SetInclusions] SET SetId = @SetId, SubsetTypeId = @SubsetTypeId, BaseId = @BaseId, RarityTypeId = @RarityTypeId, LogicalOrdinal = @LogicalOrdinal, CollectorsNumber = @CollectorsNumber WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.AddParameter("SetId", SetId);
                command.AddParameter("SubsetTypeId", SubsetTypeId);
                command.AddParameter("BaseId", BaseId);
                command.AddParameter("RarityTypeId", RarityTypeId);
                command.AddParameter("LogicalOrdinal", LogicalOrdinal);
                command.AddParameter("CollectorsNumber", CollectorsNumber);
                await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                IsModified = false;
                OnRecordChanged(RecordChangedAction.Update, this);
            }
        }

        protected override void DeleteRecord(DbCommand command)
        {
            command.CommandText = "DELETE FROM [Cards].[SetInclusions] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            command.ExecuteNonQuery();
            OnRecordChanged(RecordChangedAction.Delete, this);
        }

        protected async override Task DeleteRecordAsync(DbCommand command, CancellationToken cancellationToken)
        {
            command.CommandText = "DELETE FROM [Cards].[SetInclusions] WHERE Id = @Id;";
            command.AddParameter("Id", Id);
            await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            OnRecordChanged(RecordChangedAction.Delete, this);
        }
    }
}