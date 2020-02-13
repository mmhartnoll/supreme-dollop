using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records.Expressions;
using MindSculptor.DataAccess.DataContext;
using MindSculptor.Tools.Extensions;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace MindSculptor.App.AppDataContext.Schemas.Mtga.Tables
{
    public class DraftEventGamesTable : DataContextTable<DraftEventGameRecord, DraftEventGameRecordExpression>
    {
        private DraftEventGamesTable(DataContext dataContext) : base(dataContext, "Mtga", "DraftEventGames")
        {
        }

        internal static DraftEventGamesTable Create(DataContext dataContext)
        {
            return new DraftEventGamesTable(dataContext);
        }

        public DraftEventGameRecord NewRecord(DraftEventMatchRecord draftEventMatchRecord, int gameNumber, bool playedFirst, bool gameWon)
        {
            var newRecord = DraftEventGameRecord.Create(DataContext, Guid.NewGuid(), draftEventMatchRecord.Id, gameNumber, playedFirst, gameWon);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEventGames] ( Id, DraftEventMatchId, GameNumber, PlayedFirst, GameWon ) VALUES ( @Id, @DraftEventMatchId, @GameNumber, @PlayedFirst, @GameWon );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("DraftEventMatchId", newRecord.DraftEventMatchId);
                command.AddParameter("GameNumber", newRecord.GameNumber);
                command.AddParameter("PlayedFirst", newRecord.PlayedFirst);
                command.AddParameter("GameWon", newRecord.GameWon);
                command.ExecuteNonQuery();
            }

            return newRecord;
        }

        public async Task<DraftEventGameRecord> NewRecordAsync(DraftEventMatchRecord draftEventMatchRecord, int gameNumber, bool playedFirst, bool gameWon)
        {
            var newRecord = DraftEventGameRecord.Create(DataContext, Guid.NewGuid(), draftEventMatchRecord.Id, gameNumber, playedFirst, gameWon);
            using (var command = DataContext.Connection.CreateCommand())
            {
                if (DataContext.HasTransaction)
                    command.Transaction = DataContext.Transaction;
                command.CommandText = "INSERT INTO [Mtga].[DraftEventGames] ( Id, DraftEventMatchId, GameNumber, PlayedFirst, GameWon ) VALUES ( @Id, @DraftEventMatchId, @GameNumber, @PlayedFirst, @GameWon );";
                command.AddParameter("Id", newRecord.Id);
                command.AddParameter("DraftEventMatchId", newRecord.DraftEventMatchId);
                command.AddParameter("GameNumber", newRecord.GameNumber);
                command.AddParameter("PlayedFirst", newRecord.PlayedFirst);
                command.AddParameter("GameWon", newRecord.GameWon);
                await command.ExecuteNonQueryAsync().ConfigureAwait(false);
            }

            return newRecord;
        }

        protected override DraftEventGameRecord MapRecordFromDataReader(DbDataReader dbDataReader)
        {
            var id = (Guid)dbDataReader["Id"];
            var draftEventMatchId = (Guid)dbDataReader["DraftEventMatchId"];
            var gameNumber = Convert.ToInt32(dbDataReader["GameNumber"]);
            var playedFirst = (bool)dbDataReader["PlayedFirst"];
            var gameWon = (bool)dbDataReader["GameWon"];
            return DraftEventGameRecord.Create(DataContext, id, draftEventMatchId, gameNumber, playedFirst, gameWon);
        }
    }
}