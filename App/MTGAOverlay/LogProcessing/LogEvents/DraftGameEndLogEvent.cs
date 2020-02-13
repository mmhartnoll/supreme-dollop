using System;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class DraftGameEndLogEvent : LogEvent
    {
        public Guid MatchId { get; }
        public int GameNumber { get; }
        public bool PlayedFirst { get; }
        public bool GameWon { get; }

        private DraftGameEndLogEvent(Guid matchId, int gameNumber, bool playedFirst, bool gameWon)
        {
            MatchId     = matchId;
            GameNumber  = gameNumber;
            PlayedFirst = playedFirst;
            GameWon     = gameWon;
        }

        public static DraftGameEndLogEvent FromJson(JsonElement jsonElement)
        {
            var requestElement = jsonElement.GetProperty("request");
            var requestDocument = JsonDocument.Parse(requestElement.GetString());

            var paramsElement = requestDocument.RootElement.GetProperty("params");
            var payloadElement = paramsElement.GetProperty("payloadObject");

            var matchId = Guid.Parse(payloadElement.GetProperty("matchId").GetString());
            var gameNumber = payloadElement.GetProperty("gameNumber").GetInt32();

            var teamId = payloadElement.GetProperty("teamId").GetInt32();
            var startingTeamId = payloadElement.GetProperty("startingTeamId").GetInt32();
            var winningTeamId = payloadElement.GetProperty("winningTeamId").GetInt32();

            var playedFirst = teamId == startingTeamId;
            var gameWon = teamId == winningTeamId;

            return new DraftGameEndLogEvent(matchId, gameNumber, playedFirst, gameWon);
        }
    }
}
