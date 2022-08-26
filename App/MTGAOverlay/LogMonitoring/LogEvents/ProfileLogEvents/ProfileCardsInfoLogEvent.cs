using MindSculptor.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ProfileLogEvents
{
    internal class ProfileInventoryCardsInfoLogEvent : LogEvent
    {
        public IReadOnlyDictionary<int, int> CardCounts { get; }

        private ProfileInventoryCardsInfoLogEvent(IReadOnlyDictionary<int, int> cardCounts)
            => CardCounts = cardCounts;

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var payloadProperties = jsonElement.GetProperty("payload").EnumerateObject();

            var cardCounts = new ReadOnlyDictionary<int, int>(payloadProperties.ToDictionary(KeySelector, ValueSelector));

            result = new ProfileInventoryCardsInfoLogEvent(cardCounts);
            return true;

            static int KeySelector(JsonProperty jsonProperty) => Convert.ToInt32(jsonProperty.Name);
            static int ValueSelector(JsonProperty jsonProperty) => jsonProperty.Value.GetInt32();
        }
    }
}
