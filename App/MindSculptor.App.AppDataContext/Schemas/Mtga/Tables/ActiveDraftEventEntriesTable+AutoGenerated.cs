using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class ActiveDraftEventEntriesTable : DataContextTable<ActiveDraftEventEntryRecord, ActiveDraftEventEntryRecordExpression>
    {
        private ActiveDraftEventEntriesTable(DataContext dataContext) : base(dataContext, "Mtga", "ActiveDraftEventEntries")
        {
        }

        internal static ActiveDraftEventEntriesTable Create(DataContext dataContext)
        {
            return new ActiveDraftEventEntriesTable(dataContext);
        }

        public ActiveDraftEventEntryRecord NewRecord(DraftEventEntryRecord draftEventEntryRecord)
        {
            var newRecord = ActiveDraftEventEntryRecord.Create(DataContext, draftEventEntryRecord.Id, draftEventEntryRecord.DraftEventId, draftEventEntryRecord.ProfileId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[ActiveDraftEventEntries] ( DraftEventEntryId, DraftEventId, ProfileId ) VALUES ( @DraftEventEntryId, @DraftEventId, @ProfileId );";
                command.AddParameter("DraftEventEntryId", newRecord.DraftEventEntryId);
                command.AddParameter("DraftEventId", newRecord.DraftEventId);
                command.AddParameter("ProfileId", newRecord.ProfileId);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<ActiveDraftEventEntryRecord> NewRecordAsync(DraftEventEntryRecord draftEventEntryRecord)
        {
            var newRecord = ActiveDraftEventEntryRecord.Create(DataContext, draftEventEntryRecord.Id, draftEventEntryRecord.DraftEventId, draftEventEntryRecord.ProfileId);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[ActiveDraftEventEntries] ( DraftEventEntryId, DraftEventId, ProfileId ) VALUES ( @DraftEventEntryId, @DraftEventId, @ProfileId );";
                command.AddParameter("DraftEventEntryId", newRecord.DraftEventEntryId);
                command.AddParameter("DraftEventId", newRecord.DraftEventId);
                command.AddParameter("ProfileId", newRecord.ProfileId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override ActiveDraftEventEntryRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var draftEventEntryId = (Guid)dbDataReader["DraftEventEntryId"];
            var draftEventId = (Guid)dbDataReader["DraftEventId"];
            var profileId = (Guid)dbDataReader["ProfileId"];
            return ActiveDraftEventEntryRecord.Create(DataContext, draftEventEntryId, draftEventId, profileId);
        }
    }
}