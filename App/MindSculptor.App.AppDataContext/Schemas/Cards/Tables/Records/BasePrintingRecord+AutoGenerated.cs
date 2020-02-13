using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records
{
    public class BasePrintingRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid SetInclusionId { get; }
        public Guid PrintingTypeId { get; }

        private BasePrintingRecord(DataContext dataContext, Guid id, Guid setInclusionId, Guid printingTypeId) : base(dataContext)
        {
            Id = id;
            SetInclusionId = setInclusionId;
            PrintingTypeId = printingTypeId;
        }

        internal static BasePrintingRecord Create(DataContext dataContext, Guid id, Guid setInclusionId, Guid printingTypeId)
        {
            return new BasePrintingRecord(dataContext, id, setInclusionId, printingTypeId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Cards].[BasePrintings] SET SetInclusionId = @SetInclusionId, PrintingTypeId = @PrintingTypeId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("SetInclusionId", SetInclusionId);
                    command.AddParameter("PrintingTypeId", PrintingTypeId);
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
                    command.CommandText = "UPDATE [Cards].[BasePrintings] SET SetInclusionId = @SetInclusionId, PrintingTypeId = @PrintingTypeId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("SetInclusionId", SetInclusionId);
                    command.AddParameter("PrintingTypeId", PrintingTypeId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Cards].[BasePrintings] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Cards].[BasePrintings] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}