using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DraftEventMatchResultRecord : DataContextRecord
    {
        public Guid DraftEventMatchId { get; }
        public bool MatchWon { get; }

        private DraftEventMatchResultRecord(DataContext dataContext, Guid draftEventMatchId, bool matchWon) : base(dataContext)
        {
            DraftEventMatchId = draftEventMatchId;
            MatchWon = matchWon;
        }

        internal static DraftEventMatchResultRecord Create(DataContext dataContext, Guid draftEventMatchId, bool matchWon)
        {
            return new DraftEventMatchResultRecord(dataContext, draftEventMatchId, matchWon);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[DraftEventMatchResults] SET MatchWon = @MatchWon WHERE DraftEventMatchId = @DraftEventMatchId;";
                    command.AddParameter("DraftEventMatchId", DraftEventMatchId);
                    command.AddParameter("MatchWon", MatchWon);
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
                    command.CommandText = "UPDATE [Mtga].[DraftEventMatchResults] SET MatchWon = @MatchWon WHERE DraftEventMatchId = @DraftEventMatchId;";
                    command.AddParameter("DraftEventMatchId", DraftEventMatchId);
                    command.AddParameter("MatchWon", MatchWon);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[DraftEventMatchResults] WHERE DraftEventMatchId = @DraftEventMatchId;";
                command.AddParameter("DraftEventMatchId", DraftEventMatchId);
                command.ExecuteNonQuery();
            }
        }

        public async override Task DeleteRecordAsync()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[DraftEventMatchResults] WHERE DraftEventMatchId = @DraftEventMatchId;";
                command.AddParameter("DraftEventMatchId", DraftEventMatchId);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}