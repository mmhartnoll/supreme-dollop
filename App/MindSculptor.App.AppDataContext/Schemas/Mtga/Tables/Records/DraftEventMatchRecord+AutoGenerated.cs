using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DraftEventMatchRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid DraftEventEntryId { get; }
        public Guid OpponentId { get; }

        private DraftEventMatchRecord(DataContext dataContext, Guid id, Guid draftEventEntryId, Guid opponentId) : base(dataContext)
        {
            Id = id;
            DraftEventEntryId = draftEventEntryId;
            OpponentId = opponentId;
        }

        internal static DraftEventMatchRecord Create(DataContext dataContext, Guid id, Guid draftEventEntryId, Guid opponentId)
        {
            return new DraftEventMatchRecord(dataContext, id, draftEventEntryId, opponentId);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[DraftEventMatches] SET DraftEventEntryId = @DraftEventEntryId, OpponentId = @OpponentId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("DraftEventEntryId", DraftEventEntryId);
                    command.AddParameter("OpponentId", OpponentId);
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
                    command.CommandText = "UPDATE [Mtga].[DraftEventMatches] SET DraftEventEntryId = @DraftEventEntryId, OpponentId = @OpponentId WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("DraftEventEntryId", DraftEventEntryId);
                    command.AddParameter("OpponentId", OpponentId);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[DraftEventMatches] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Mtga].[DraftEventMatches] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}