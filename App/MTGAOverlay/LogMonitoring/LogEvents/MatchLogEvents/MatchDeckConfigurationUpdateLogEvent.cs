using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents
{
    internal class MatchDeckConfigurationUpdateLogEvent : LogEvent
    {
        public IEnumerable<int> DeckCardIds { get; }
        public IEnumerable<int> SideboardCardIds { get; }

        private MatchDeckConfigurationUpdateLogEvent(IEnumerable<int> deckCardIds, IEnumerable<int> sideboardCardIds)
        {
            DeckCardIds         = deckCardIds       .ToList().Enumerate();
            SideboardCardIds    = sideboardCardIds  .ToList().Enumerate();
        }

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            if (jsonElement.TryGetProperty("deckCards", out _) && jsonElement.TryGetProperty("sideboardCards", out _))
                return TryParseDeckMessage(jsonElement, out result);

            if (jsonElement.TryGetProperty("payload", out var payloadElement) && payloadElement.ValueKind == JsonValueKind.Object)
                if (payloadElement.TryGetProperty("submitDeckResp", out var submitDeckElement))
                    return TryParseDeckMessage(submitDeckElement.GetProperty("deck"), out result);

            result = null;
            return false;
        }

        private static bool TryParseDeckMessage(JsonElement deckMessageElement, out NullableReference<LogEvent> result)
        {
            var deckCardIds = deckMessageElement.GetProperty("deckCards")
                .EnumerateArray()
                .Select(element => element.GetInt32());
            var sideboardCardIds = deckMessageElement.GetProperty("sideboardCards")
                .EnumerateArray()
                .Select(element => element.GetInt32());

            result = new MatchDeckConfigurationUpdateLogEvent(deckCardIds, sideboardCardIds);
            return true;
        }
    }
}
