using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class DraftMatchEndLogEvent : LogEvent
    {
        public Guid MatchId { get; }
        public int WinningPlayerNumber { get; }
        public IReadOnlyDictionary<int, int> WinningPlayerNumberForEachGame { get; }

        private DraftMatchEndLogEvent(Guid matchId, int winningPlayerNumber, IReadOnlyDictionary<int, int> winningPlayerNumberForEachGame)
        {
            MatchId                         = matchId;
            WinningPlayerNumber             = winningPlayerNumber;
            WinningPlayerNumberForEachGame  = winningPlayerNumberForEachGame;
        }

        public static DraftMatchEndLogEvent FromJson(JsonElement jsonElement)
        {
            var finalMatchResultElement = jsonElement
                .GetProperty("matchGameRoomStateChangedEvent")
                .GetProperty("gameRoomInfo")
                .GetProperty("finalMatchResult");

            var matchId = Guid.Parse(finalMatchResultElement.GetProperty("matchId").ToString());

            var gameResultList = new List<int>();
            var resultListElements = finalMatchResultElement.GetProperty("resultList").EnumerateArray();
            foreach (var element in resultListElements)
            {
                var resultScope = element.GetProperty("scope").ToString();
                if (resultScope == "MatchScope_Game")
                {
                    var winningTeamId = element.GetProperty("winningTeamId").GetInt32();
                    gameResultList.Add(winningTeamId);
                }
            }

            var matchResultElement = resultListElements
                .Where(element => element.GetProperty("scope").ToString() == "MatchScope_Match")
                .Single();
            var winningPlayerNumber = matchResultElement.GetProperty("winningTeamId").GetInt32();

            var ordinal = 0;
            var winningPlayerNumberForEachGame = new ReadOnlyDictionary<int, int>(gameResultList.ToDictionary(_ => ordinal++));

            return new DraftMatchEndLogEvent(matchId, winningPlayerNumber, winningPlayerNumberForEachGame);
        }
    }
}
