using MindSculptor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.Events.DraftEvents
{
    internal class DraftPickOptionsLogEvent : LogEvent
    {
        public int PackNumber { get; }
        public int PickNumber { get; }
        public IReadOnlyList<int> DraftPackCardIds { get; }

        private DraftPickOptionsLogEvent(int packNumber, int pickNumber, IReadOnlyList<int> draftPackCardIds)
        {
            PackNumber = packNumber;
            PickNumber = pickNumber;
            DraftPackCardIds = draftPackCardIds;
        }

        public static IEnumerable<DraftPickOptionsLogEvent> TryParseFromBotDraftFormat(JsonElement jsonElement)
        {
            var payloadElement = jsonElement.GetProperty("payload");

            var packNumber = payloadElement.GetProperty("PackNumber").GetInt32() + 1;
            var pickNumber = payloadElement.GetProperty("PickNumber").GetInt32() + 1;
            var draftPackProperty = payloadElement.GetProperty("DraftPack");

            var draftPackCardIds = draftPackProperty.ValueKind == JsonValueKind.Null ?
                Enumerable.Empty<int>() :
                draftPackProperty.EnumerateArray().Select(jsonElement => Convert.ToInt32(jsonElement.GetString()));

            yield return new DraftPickOptionsLogEvent(packNumber, pickNumber, draftPackCardIds.ToList().AsReadOnly());
        }

        public static IEnumerable<DraftPickOptionsLogEvent> TryParseFromHumanDraftFormat(JsonElement jsonElement)
        {
            var packNumber = jsonElement.GetProperty("SelfPack").GetInt32();
            var pickNumber = jsonElement.GetProperty("SelfPick").GetInt32();
            var packCardsString = jsonElement.GetProperty("PackCards").GetString().Trim();

            var draftPackCardIds = packCardsString == string.Empty ?
                Enumerable.Empty<int>() :
                packCardsString.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(cardId => Convert.ToInt32(cardId));

            yield return new DraftPickOptionsLogEvent(packNumber, pickNumber, draftPackCardIds.ToList().AsReadOnly());
        }
    }
}
