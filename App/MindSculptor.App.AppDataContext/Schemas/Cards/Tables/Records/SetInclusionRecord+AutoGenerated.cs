using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class SetInclusionRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid SetId { get; }
        public Guid SubsetTypeId { get; }
        public Guid BaseId { get; }
        public Guid RarityTypeId { get; }
        public int LogicalOrdinal { get; }
        public int? CollectorsNumber { get; }

        private SetInclusionRecord(DataContext dataContext, Guid id, Guid setId, Guid subsetTypeId, Guid baseId, Guid rarityTypeId, int logicalOrdinal, int? collectorsNumber) : base(dataContext)
        {
            Id = id;
            SetId = setId;
            SubsetTypeId = subsetTypeId;
            BaseId = baseId;
            RarityTypeId = rarityTypeId;
            LogicalOrdinal = logicalOrdinal;
            CollectorsNumber = collectorsNumber;
        }

        internal static SetInclusionRecord Create(DataContext dataContext, Guid id, Guid setId, Guid subsetTypeId, Guid baseId, Guid rarityTypeId, int logicalOrdinal, int? collectorsNumber)
        {
            return new SetInclusionRecord(dataContext, id, setId, subsetTypeId, baseId, rarityTypeId, logicalOrdinal, collectorsNumber);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[SetInclusions] SET SetId = @SetId, SubsetTypeId = @SubsetTypeId, BaseId = @BaseId, RarityTypeId = @RarityTypeId, LogicalOrdinal = @LogicalOrdinal, CollectorsNumber = @CollectorsNumber WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("SetId", SetId);
                    command.AddParameter("SubsetTypeId", SubsetTypeId);
                    command.AddParameter("BaseId", BaseId);
                    command.AddParameter("RarityTypeId", RarityTypeId);
                    command.AddParameter("LogicalOrdinal", LogicalOrdinal);
                    command.AddParameter("CollectorsNumber", CollectorsNumber);
                    command.ExecuteNonQuery();
                }
        }

        public async override Task UpdateRecordAsync()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[SetInclusions] SET SetId = @SetId, SubsetTypeId = @SubsetTypeId, BaseId = @BaseId, RarityTypeId = @RarityTypeId, LogicalOrdinal = @LogicalOrdinal, CollectorsNumber = @CollectorsNumber WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("SetId", SetId);
                    command.AddParameter("SubsetTypeId", SubsetTypeId);
                    command.AddParameter("BaseId", BaseId);
                    command.AddParameter("RarityTypeId", RarityTypeId);
                    command.AddParameter("LogicalOrdinal", LogicalOrdinal);
                    command.AddParameter("CollectorsNumber", CollectorsNumber);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[SetInclusions] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[SetInclusions] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}