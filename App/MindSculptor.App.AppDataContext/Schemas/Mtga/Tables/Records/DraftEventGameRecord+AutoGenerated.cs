using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records
{
    public class DraftEventGameRecord : DataContextRecord
    {
        public Guid Id { get; }
        public Guid DraftEventMatchId { get; }
        public int GameNumber { get; }
        public bool PlayedFirst { get; }
        public bool GameWon { get; }

        private DraftEventGameRecord(DataContext dataContext, Guid id, Guid draftEventMatchId, int gameNumber, bool playedFirst, bool gameWon) : base(dataContext)
        {
            Id = id;
            DraftEventMatchId = draftEventMatchId;
            GameNumber = gameNumber;
            PlayedFirst = playedFirst;
            GameWon = gameWon;
        }

        internal static DraftEventGameRecord Create(DataContext dataContext, Guid id, Guid draftEventMatchId, int gameNumber, bool playedFirst, bool gameWon)
        {
            return new DraftEventGameRecord(dataContext, id, draftEventMatchId, gameNumber, playedFirst, gameWon);
        }

        public override void UpdateRecord()
        {
            if (IsModified)
                using (var command = DataContext.Connection.CreateCommand())
                {
                    if (DataContext.HasTransaction)
                        command.Transaction = DataContext.Transaction;
                    command.CommandText = "UPDATE [Mtga].[DraftEventGames] SET DraftEventMatchId = @DraftEventMatchId, GameNumber = @GameNumber, PlayedFirst = @PlayedFirst, GameWon = @GameWon WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("DraftEventMatchId", DraftEventMatchId);
                    command.AddParameter("GameNumber", GameNumber);
                    command.AddParameter("PlayedFirst", PlayedFirst);
                    command.AddParameter("GameWon", GameWon);
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
                    command.CommandText = "UPDATE [Mtga].[DraftEventGames] SET DraftEventMatchId = @DraftEventMatchId, GameNumber = @GameNumber, PlayedFirst = @PlayedFirst, GameWon = @GameWon WHERE Id = @Id;";
                    command.AddParameter("Id", Id);
                    command.AddParameter("DraftEventMatchId", DraftEventMatchId);
                    command.AddParameter("GameNumber", GameNumber);
                    command.AddParameter("PlayedFirst", PlayedFirst);
                    command.AddParameter("GameWon", GameWon);
                    await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                }
        }

        public override void DeleteRecord()
        {
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "DELETE FROM [Mtga].[DraftEventGames] WHERE Id = @Id;";
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
                command.CommandText = "DELETE FROM [Mtga].[DraftEventGames] WHERE Id = @Id;";
                command.AddParameter("Id", Id);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }
        }
    }
}