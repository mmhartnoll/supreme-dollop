using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class ActiveDraftEventEntryRecord : DataContextRecord
    {
        public Guid DraftEventEntryId { get; }
        public Guid DraftEventId { get; }
        public Guid ProfileId { get; }

        private ActiveDraftEventEntryRecord(DataContext dataContext, Guid draftEventEntryId, Guid draftEventId, Guid profileId) : base(dataContext)
        {
            DraftEventEntryId = draftEventEntryId;
            DraftEventId = draftEventId;
            ProfileId = profileId;
        }

        internal static ActiveDraftEventEntryRecord Create(DataContext dataContext, Guid draftEventEntryId, Guid draftEventId, Guid profileId)
        {
            return new ActiveDraftEventEntryRecord(dataContext, draftEventEntryId, draftEventId, profileId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[ActiveDraftEventEntries] SET  WHERE DraftEventEntryId = @DraftEventEntryId AND DraftEventId = @DraftEventId AND ProfileId = @ProfileId;";
                    command.AddParameter("DraftEventEntryId", DraftEventEntryId);
                    command.AddParameter("DraftEventId", DraftEventId);
                    command.AddParameter("ProfileId", ProfileId);
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
                    command.CommandText = "UPDATE [Mtga].[ActiveDraftEventEntries] SET  WHERE DraftEventEntryId = @DraftEventEntryId AND DraftEventId = @DraftEventId AND ProfileId = @ProfileId;";
                    command.AddParameter("DraftEventEntryId", DraftEventEntryId);
                    command.AddParameter("DraftEventId", DraftEventId);
                    command.AddParameter("ProfileId", ProfileId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[ActiveDraftEventEntries] WHERE DraftEventEntryId = @DraftEventEntryId AND DraftEventId = @DraftEventId AND ProfileId = @ProfileId;";
                command.AddParameter("DraftEventEntryId", DraftEventEntryId);
                command.AddParameter("DraftEventId", DraftEventId);
                command.AddParameter("ProfileId", ProfileId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[ActiveDraftEventEntries] WHERE DraftEventEntryId = @DraftEventEntryId AND DraftEventId = @DraftEventId AND ProfileId = @ProfileId;";
                command.AddParameter("DraftEventEntryId", DraftEventEntryId);
                command.AddParameter("DraftEventId", DraftEventId);
                command.AddParameter("ProfileId", ProfileId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}