using MindSculptor.Tools;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.EventLogEvents
{
    internal class EventJoinLogEvent : LogEvent
    {
        public string MtgaEventId { get; }
        public Guid EventEntryId { get; }

        private EventJoinLogEvent(string mtgaEventId, Guid eventEntryId)
        {
            MtgaEventId  = mtgaEventId;
            EventEntryId = eventEntryId;
        }

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var payloadElement = jsonElement.GetProperty("payload");

            var mtgaEventId = payloadElement.GetProperty("InternalEventName").GetString();
            var regexMatch = Regex.Match(mtgaEventId, $"^((?>[a-zA-Z]+)(?>_[a-zA-Z]+)*)_\\d+$");
            if (regexMatch.Success)
                mtgaEventId = regexMatch.Groups[1].Value;

            var eventEntryId = payloadElement.GetProperty("Id").GetGuid();

            result = new EventJoinLogEvent(mtgaEventId, eventEntryId);
            return true;
        }
    }
}
