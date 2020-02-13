using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DraftEventEntryRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid DraftEventId { get; }
        public Guid ProfileId { get; }

        private DraftEventEntryRecord(DataContext dataContext, Guid id, Guid draftEventId, Guid profileId) : base(dataContext)
        {
            Id = id;
            DraftEventId = draftEventId;
            ProfileId = profileId;
        }

        internal static DraftEventEntryRecord Create(DataContext dataContext, Guid id, Guid draftEventId, Guid profileId)
        {
            return new DraftEventEntryRecord(dataContext, id, draftEventId, profileId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[DraftEventEntries] SET DraftEventId = @DraftEventId, ProfileId = @ProfileId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
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
                    command.CommandText = "UPDATE [Mtga].[DraftEventEntries] SET DraftEventId = @DraftEventId, ProfileId = @ProfileId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
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
                command.CommandText = "DELETE FROM [Mtga].[DraftEventEntries] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Mtga].[DraftEventEntries] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}