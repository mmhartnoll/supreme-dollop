using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class UpdateProfileCardsLogEvent : LogEvent
    {
        public IReadOnlyDictionary<int, int> CardCounts { get; }

        private UpdateProfileCardsLogEvent(IReadOnlyDictionary<int, int> cardCounts)
            => CardCounts = cardCounts;

        public static UpdateProfileCardsLogEvent FromJson(JsonElement jsonElement)
        {
            var payloadProperties = jsonElement.GetProperty("payload").EnumerateObject();

            var cardCounts = new ReadOnlyDictionary<int, int>(payloadProperties.ToDictionary(KeySelector, ValueSelector));

            return new UpdateProfileCardsLogEvent(cardCounts);

            static int KeySelector(JsonProperty jsonProperty) => Convert.ToInt32(jsonProperty.Name);
            static int ValueSelector(JsonProperty jsonProperty) => jsonProperty.Value.GetInt32();
        }
    }
}
