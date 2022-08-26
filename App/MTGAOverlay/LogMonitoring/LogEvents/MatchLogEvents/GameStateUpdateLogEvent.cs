using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.Tools;
using MindSculptor.Tools.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents
{
    internal class GameStateUpdateLogEvent : LogEvent
    {
        private readonly ResultType? matchResult;

        public Guid MatchId { get; }
        public int GameNumber { get; }
        public GameState GameState { get; }
        public GameStage GameStage { get; }

        public int PlayerSeatId { get; }
        public GameStateZone PlayerLibraryZone { get; }
        public GameStateZone PlayerSideboardZone { get; }
        public GameStateZone PlayerHandZone { get; }

        public IReadOnlyDictionary<int, ResultType> GameResults { get; }
        public bool HasMatchResult => matchResult.HasValue;
        public ResultType MatchResult => matchResult ?? throw new PropertyUndefinedException(nameof(MatchResult), nameof(HasMatchResult));

        private GameStateUpdateLogEvent(Guid matchId, int gameNumber, GameState gameState, GameStage gameStage, int playerSeatId, GameStateZone playerLibraryZone, GameStateZone playerSideboardZone, GameStateZone playerHandZone, 
            IReadOnlyDictionary<int, ResultType> gameResults, ResultType? matchResult)
        {
            MatchId = matchId;
            GameNumber = gameNumber;
            GameState = gameState;
            GameStage = gameStage;
            PlayerSeatId = playerSeatId;
            PlayerLibraryZone = playerLibraryZone;
            PlayerSideboardZone = playerSideboardZone;
            PlayerHandZone = playerHandZone;
            GameResults = gameResults;
            this.matchResult = matchResult;
        }

        public static bool TryCreateFromFactory(GameStateUpdateLogEventFactory factory, out NullableReference<LogEvent> result)
        {
            if (factory.MatchId.HasValue && factory.PlayerSeatId.HasValue && factory.GameNumber.HasValue && factory.GameStage.HasValue)
            {
                var playerLibraryZone = factory.PlayerSeatId.Value switch
                {
                    1 => GameStateZone.PlayerOneLibrary,
                    2 => GameStateZone.PlayerTwoLibrary,
                    _ => throw new InvalidOperationException()
                };
                var playerSideboardZone = factory.PlayerSeatId.Value switch
                {
                    1 => GameStateZone.PlayerOneSideboard,
                    2 => GameStateZone.PlayerTwoSideboard,
                    _ => throw new InvalidOperationException()
                };
                var playerHandZone = factory.PlayerSeatId.Value switch
                {
                    1 => GameStateZone.PlayerOneHand,
                    2 => GameStateZone.PlayerTwoHand,
                    _ => throw new InvalidOperationException()
                };

                var gameResults = new ReadOnlyDictionary<int, ResultType>(factory.GameResults.ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Value));

                result = new GameStateUpdateLogEvent(factory.MatchId.Value, factory.GameNumber.Value, factory.GameState, factory.GameStage.Value, factory.PlayerSeatId.Value, playerLibraryZone, playerSideboardZone, playerHandZone, gameResults, factory.MatchResult);
                return true;
            }
            result = null;
            return false;
        }
    }
}
