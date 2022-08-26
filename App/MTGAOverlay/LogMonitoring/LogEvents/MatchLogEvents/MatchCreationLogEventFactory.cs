using MindSculptor.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents
{
    internal class MatchCreationLogEventFactory
    {
        public NullableReference<string>                        MtgaEventId         { get; private set; } = null;
        public Guid?                                            EventEntryId        { get; private set; } = null;
        public Guid?                                            MatchId             { get; private set; } = null;

        public NullableReference<string>                        PlayerOneMtgaUserId { get; private set; } = null;
        public NullableReference<string>                        PlayerOneName       { get; private set; } = null;

        public NullableReference<string>                        PlayerTwoMtgaUserId { get; private set; } = null;
        public NullableReference<string>                        PlayerTwoName       { get; private set; } = null;

        public NullableReference<string>                        DeckName            { get; private set; } = null;
        public NullableReference<IReadOnlyDictionary<int, int>> DeckCardCounts      { get; private set; } = null;
        public NullableReference<IReadOnlyDictionary<int, int>> SideboardCardCounts { get; private set; } = null;

        public bool TryParseDeckInfo(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            Invalidate();

            var payloadElement = jsonElement   .GetProperty("payload");
            var mtgaEventId    = payloadElement.GetProperty("InternalEventName").GetString();
            var eventEntryId   = payloadElement.GetProperty("Id")               .GetGuid();

            if (eventEntryId != Guid.Empty)
            {
                var regexMatch = Regex.Match(mtgaEventId, $"^((?>[a-zA-Z]+)(?>_[a-zA-Z]+)*)_\\d+$");
                if (regexMatch.Success)
                    mtgaEventId = regexMatch.Groups[1].Value;

                var deckElement = payloadElement.GetProperty("CourseDeck");
                if (deckElement.ValueKind != JsonValueKind.Null)
                {
                    var deckCardCounts      = new Dictionary<int, int>();
                    var sideboardCardCounts = new Dictionary<int, int>();

                    var deckName  = deckElement.GetProperty("name")     .GetString();
                    var mainDeck  = deckElement.GetProperty("mainDeck") .EnumerateArray();
                    var sideboard = deckElement.GetProperty("sideboard").EnumerateArray();

                    using var mainDeckEnumerator = mainDeck.GetEnumerator();
                    while (mainDeckEnumerator.MoveNext())
                    {
                        var mtgaCardId = mainDeckEnumerator.Current.GetInt32();
                        mainDeckEnumerator.MoveNext();
                        var count = mainDeckEnumerator.Current.GetInt32();
                        deckCardCounts.Add(mtgaCardId, count);
                    }

                    using var sideboardEnumerator = sideboard.GetEnumerator();
                    while (sideboardEnumerator.MoveNext())
                    {
                        var mtgaCardId = sideboardEnumerator.Current.GetInt32();
                        sideboardEnumerator.MoveNext();
                        var count = sideboardEnumerator.Current.GetInt32();
                        sideboardCardCounts.Add(mtgaCardId, count);
                    }

                    MtgaEventId         = mtgaEventId;
                    EventEntryId        = eventEntryId;
                    DeckName            = deckName;
                    DeckCardCounts      = new ReadOnlyDictionary<int, int>(deckCardCounts);
                    SideboardCardCounts = new ReadOnlyDictionary<int, int>(sideboardCardCounts);
                }
            }

            result = null;
            return false;
        }

        public bool TryParsePlayerInfo(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            if (MtgaEventId.HasValue)
            {
                var gameRoomConfigProperty = jsonElement.GetProperty("matchGameRoomStateChangedEvent")
                    .GetProperty("gameRoomInfo")
                    .GetProperty("gameRoomConfig");

                var mtgaEventId = gameRoomConfigProperty.GetProperty("eventId").GetString();
                var matchId     = gameRoomConfigProperty.GetProperty("matchId").GetGuid();

                var regexMatch = Regex.Match(mtgaEventId, $"^((?>[a-zA-Z]+)(?>_[a-zA-Z]+)*)_\\d+$");
                if (regexMatch.Success)
                    mtgaEventId = regexMatch.Groups[1].Value;

                if (MtgaEventId.Value == mtgaEventId)
                {
                    var playersElements = gameRoomConfigProperty.GetProperty("reservedPlayers").EnumerateArray();

                    var playerOneElement    = playersElements.First();
                    var playerOneMtgaUserId = playerOneElement.GetProperty("userId")    .GetString();
                    var playerOneName       = playerOneElement.GetProperty("playerName").GetString();

                    var playerTwoElement    = playersElements.Last();
                    var playerTwoMtgaUserId = playerTwoElement.GetProperty("userId")    .GetString();
                    var playerTwoName       = playerTwoElement.GetProperty("playerName").GetString();

                    MatchId             = matchId;
                    PlayerOneMtgaUserId = playerOneMtgaUserId;
                    PlayerOneName       = playerOneName;
                    PlayerTwoMtgaUserId = playerTwoMtgaUserId;
                    PlayerTwoName       = playerTwoName;

                    return MatchCreationLogEvent.TryCreateFromFactory(this, out result);
                }
            }
            result = null;
            return false;
        }

        public void Invalidate()
        {
            MtgaEventId         = null;
            EventEntryId        = null;
            MatchId             = null;
            PlayerOneMtgaUserId = null;
            PlayerOneName       = null;
            PlayerTwoMtgaUserId = null;
            PlayerTwoName       = null;
            DeckName            = null;
            DeckCardCounts      = null;
            SideboardCardCounts = null;
        }
    }
}
