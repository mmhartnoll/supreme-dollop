using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class DraftEventEntriesTable : DataContextTable<DraftEventEntryRecord, DraftEventEntryRecordExpression>
    {
        private DraftEventEntriesTable(DataContext dataContext) : base(dataContext, "Mtga", "DraftEventEntries")
        {
        }

        internal static DraftEventEntriesTable Create(DataContext dataContext)
        {
            return new DraftEventEntriesTable(dataContext);
        }

        public DraftEventEntryRecord NewRecord(DraftEventRecord draftEventRecord, ProfileRecord profileRecord, Guid id)
        {
            var newRecord = DraftEventEntryRecord.Create(DataContext, id, draftEventRecord.Id, profileRecord.Id);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEventEntries] ( Id, DraftEventId, ProfileId ) VALUES ( @Id, @DraftEventId, @ProfileId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("DraftEventId", newRecord.DraftEventId);
                command.AddParameter("ProfileId", newRecord.ProfileId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<DraftEventEntryRecord> NewRecordAsync(DraftEventRecord draftEventRecord, ProfileRecord profileRecord, Guid id)
        {
            var newRecord = DraftEventEntryRecord.Create(DataContext, id, draftEventRecord.Id, profileRecord.Id);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEventEntries] ( Id, DraftEventId, ProfileId ) VALUES ( @Id, @DraftEventId, @ProfileId );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("DraftEventId", newRecord.DraftEventId);
                command.AddParameter("ProfileId", newRecord.ProfileId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override DraftEventEntryRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var draftEventId = (Guid)dbDataReader["DraftEventId"];
            var profileId = (Guid)dbDataReader["ProfileId"];
            return DraftEventEntryRecord.Create(DataContext, id, draftEventId, profileId);
        }
    }
}