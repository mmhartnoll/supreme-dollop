using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Cards.Tables
{
    public class BasePrintingsTable : DataContextTable<BasePrintingRecord, BasePrintingRecordExpression>
    {
        private BasePrintingsTable(DataContext dataContext) : base(dataContext, "Cards", "BasePrintings")
        {
        }

        internal static BasePrintingsTable Create(DataContext dataContext)
        {
            return new BasePrintingsTable(dataContext);
        }

        public BasePrintingRecord NewRecord(SetInclusionRecord setInclusionRecord, PrintingTypeRecord printingTypeRecord)
        {
            var newRecord = BasePrintingRecord.Create(DataContext, Guid.NewGuid(), setInclusionRecord.Id, printingTypeRecord.Id);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[BasePrintings] ( Id, SetInclusionId, PrintingTypeId ) VALUES ( @Id, @SetInclusionId, @PrintingTypeId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("SetInclusionId", newRecord.SetInclusionId);
                command.AddParameter("PrintingTypeId", newRecord.PrintingTypeId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<BasePrintingRecord> NewRecordAsync(SetInclusionRecord setInclusionRecord, PrintingTypeRecord printingTypeRecord)
        {
            var newRecord = BasePrintingRecord.Create(DataContext, Guid.NewGuid(), setInclusionRecord.Id, printingTypeRecord.Id);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Cards].[BasePrintings] ( Id, SetInclusionId, PrintingTypeId ) VALUES ( @Id, @SetInclusionId, @PrintingTypeId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("SetInclusionId", newRecord.SetInclusionId);
                command.AddParameter("PrintingTypeId", newRecord.PrintingTypeId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override BasePrintingRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var setInclusionId = (Guid)dbDataReader["SetInclusionId"];
            var printingTypeId = (Guid)dbDataReader["PrintingTypeId"];
            return BasePrintingRecord.Create(DataContext, id, setInclusionId, printingTypeId);
        }
    }
}