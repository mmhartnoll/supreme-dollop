using MindSculptor.Tools;
using System;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents
{
    internal class MatchResultLogEvent : LogEvent
    {
        public Guid MatchId { get; }
        public int WinningPlayerNumber { get; }

        private MatchResultLogEvent(Guid matchId, int winningPlayerNumber)
        {
            MatchId = matchId;
            WinningPlayerNumber = winningPlayerNumber;
        }

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var finalMatchResultElement = jsonElement
                .GetProperty("matchGameRoomStateChangedEvent")
                .GetProperty("gameRoomInfo")
                .GetProperty("finalMatchResult");

            var matchId = Guid.Parse(finalMatchResultElement.GetProperty("matchId").ToString());

            var resultListElements = finalMatchResultElement.GetProperty("resultList").EnumerateArray();
            var matchResultElement = resultListElements
                .Where(element => element.GetProperty("scope").ToString() == "MatchScope_Match")
                .Single();
            var winningPlayerNumber = matchResultElement.GetProperty("winningTeamId").GetInt32();

            result = new MatchResultLogEvent(matchId, winningPlayerNumber);
            return true;
        }
    }
}
