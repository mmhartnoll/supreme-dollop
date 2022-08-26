using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.Tools;
using MindSculptor.Tools.Exceptions;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class EventMatchModel : Model
    {
        private readonly EventMatchRecord eventMatchRecord;
        private readonly IList<MatchGame> matchGames;

        private bool? matchWon = null;

        public Guid Id { get; }
        public Player Opponent { get; }
        public IEnumerable<MatchGame> Games => matchGames.Enumerate();

        public bool HasMatchResult => matchWon.HasValue;
        public bool MatchWon => matchWon.HasValue ? matchWon.Value : throw new PropertyUndefinedException(nameof(MatchWon), nameof(HasMatchResult));

        private EventMatchModel(DataContext dataContext, EventMatchRecord eventMatchRecord, Player opponent, IEnumerable<MatchGame> matchGames, bool? matchWon) 
            : base(dataContext)
        {
            this.eventMatchRecord = eventMatchRecord;
            Id                    = eventMatchRecord.Id;
            Opponent              = opponent;
            this.matchGames       = matchGames.ToList();
            this.matchWon         = matchWon;
        }

        public static async Task<EventMatchModel> CreateAsync(DataContext dataContext, EventEntryRecord eventEntryRecord, Guid id, Player opponent)
        {
            var eventMatchRecord = await dataContext.Mtga.EventMatches.NewRecordAsync(id, eventEntryRecord.Id, opponent.Id).ConfigureAwait(false);
            return new EventMatchModel(dataContext, eventMatchRecord, opponent, Enumerable.Empty<MatchGame>(), null);
        }

        public static async Task<VerifiedResult<EventMatchModel>> TryLoadAsync(DataContext dataContext, Guid id)
        {
            var eventMatchResult = await dataContext.Mtga.EventMatches
                .QueryWhere(record => record.Id == id)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (eventMatchResult.Success)
            {
                var eventMatchRecord = eventMatchResult.Value;
                var opponent = await Player.LoadAsync(dataContext, eventMatchRecord.OpponentId).ConfigureAwait(false);
                var matchGames = await dataContext.Mtga.EventMatchGames
                    .QueryWhere(record => record.EventMatchRecord == eventMatchRecord)
                    .OrderBy(record => record.GameNumber)
                    .SelectAsync(record => new MatchGame(record.Id, record.GameNumber, record.PlayedFirst, record.GameWon))
                    .ToListAsync()
                    .ConfigureAwait(false);

                bool? matchWon = null;
                var eventMatchResultResult = await dataContext.Mtga.EventMatchResults
                    .QueryWhere(record => record.EventMatchRecord == eventMatchRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (eventMatchResultResult.Success)
                    matchWon = eventMatchResultResult.Value.MatchWon;

                return VerifiedResult<EventMatchModel>.Successful(new EventMatchModel(dataContext, eventMatchRecord, opponent, matchGames, matchWon));
            }
            return VerifiedResult<EventMatchModel>.Failure;
        }

        public async Task SetGameResult(int gameNumber, bool playedFirst, bool gameWon)
        {
            if (!Games.Any(game => game.GameNumber == gameNumber))
            {
                var eventMatchGameRecord = await DataContext.Mtga.EventMatchGames.NewRecordAsync(eventMatchRecord, gameNumber, playedFirst, gameWon).ConfigureAwait(false);
                matchGames.Add(new MatchGame(eventMatchGameRecord.Id, gameNumber, playedFirst, gameWon));

                var winLoss = gameWon ? "win" : "loss";
                await OnLogMessageAsync($"Game {winLoss} recorded for game {gameNumber}.").ConfigureAwait(false);
            }
        }

        public async Task SetMatchResult(bool matchWon)
        {
            if (!HasMatchResult)
            {
                await DataContext.Mtga.EventMatchResults.NewRecordAsync(eventMatchRecord, matchWon).ConfigureAwait(false);
                this.matchWon = matchWon;

                var winLoss = matchWon ? "win" : "loss";
                await OnLogMessageAsync($"Match {winLoss} recorded.").ConfigureAwait(false);
            }
        }
    }
}
