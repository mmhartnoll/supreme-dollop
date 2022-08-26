using MindSculptor.Tools;
using System;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents
{
    internal class MatchGameResultLogEvent : LogEvent
    {
        public Guid MatchId { get; }
        public int GameNumber { get; }
        public bool PlayedFirst { get; }
        public bool GameWon { get; }

        private MatchGameResultLogEvent(Guid matchId, int gameNumber, bool playedFirst, bool gameWon)
        {
            MatchId = matchId;
            GameNumber = gameNumber;
            PlayedFirst = playedFirst;
            GameWon = gameWon;
        }

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var payloadElement = jsonElement.GetProperty("payloadObject");

            var matchId = Guid.Parse(payloadElement.GetProperty("matchId").GetString());
            var gameNumber = payloadElement.GetProperty("gameNumber").GetInt32();

            var teamId = payloadElement.GetProperty("teamId").GetInt32();
            var startingTeamId = payloadElement.GetProperty("startingTeamId").GetInt32();
            var winningTeamId = payloadElement.GetProperty("winningTeamId").GetInt32();

            var playedFirst = teamId == startingTeamId;
            var gameWon = teamId == winningTeamId;

            result = new MatchGameResultLogEvent(matchId, gameNumber, playedFirst, gameWon);
            return true;
        }
    }
}
