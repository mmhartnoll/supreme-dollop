using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.EventLogEvents
{
    internal class EventInfoLogEvent : LogEvent
    {
        public IEnumerable<EventInfo> EventInfo { get; }

        private EventInfoLogEvent(IEnumerable<EventInfo> eventInfo)
            => EventInfo = eventInfo.Enumerate();

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var eventInfo = EnumerateEventInfo(jsonElement);
            if (eventInfo.Any())
            {
                result = new EventInfoLogEvent(eventInfo);
                return true;
            }
            result = null;
            return false;

            static IEnumerable<EventInfo> EnumerateEventInfo(JsonElement jsonElement)
            {
                var payloadElements = jsonElement.GetProperty("payload").EnumerateArray();
                foreach (var payloadElement in payloadElements)
                {
                    var mtgaEventId = payloadElement.GetProperty("InternalEventName").GetString();

                    var regexMatch = Regex.Match(mtgaEventId, $"^((?>[a-zA-Z]+)(?>_[a-zA-Z]+)*)_\\d+$");
                    if (regexMatch.Success)
                        mtgaEventId = regexMatch.Groups[1].Value;

                    Guid? activeEventEntryId = payloadElement.GetProperty("Id").GetGuid();
                    if (activeEventEntryId == Guid.Empty)
                        activeEventEntryId = null;
                    yield return new EventInfo(mtgaEventId, activeEventEntryId);
                }
            }
        }
    }
}
