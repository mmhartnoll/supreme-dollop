using MindSculptor.App.AppDataContext.Schemas.Cards.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class DraftEventsTable : DataContextTable<DraftEventRecord, DraftEventRecordExpression>
    {
        private DraftEventsTable(DataContext dataContext) : base(dataContext, "Mtga", "DraftEvents")
        {
        }

        internal static DraftEventsTable Create(DataContext dataContext)
        {
            return new DraftEventsTable(dataContext);
        }

        public DraftEventRecord NewRecord(SetRecord setRecord, string draftType)
        {
            var newRecord = DraftEventRecord.Create(DataContext, Guid.NewGuid(), setRecord.Id, draftType);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEvents] ( Id, SetId, DraftType ) VALUES ( @Id, @SetId, @DraftType );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("SetId", newRecord.SetId);
                command.AddParameter("DraftType", newRecord.DraftType);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<DraftEventRecord> NewRecordAsync(SetRecord setRecord, string draftType)
        {
            var newRecord = DraftEventRecord.Create(DataContext, Guid.NewGuid(), setRecord.Id, draftType);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEvents] ( Id, SetId, DraftType ) VALUES ( @Id, @SetId, @DraftType );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("SetId", newRecord.SetId);
                command.AddParameter("DraftType", newRecord.DraftType);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override DraftEventRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var setId = (Guid)dbDataReader["SetId"];
            var draftType = (string)dbDataReader["DraftType"];
            return DraftEventRecord.Create(DataContext, id, setId, draftType);
        }
    }
}