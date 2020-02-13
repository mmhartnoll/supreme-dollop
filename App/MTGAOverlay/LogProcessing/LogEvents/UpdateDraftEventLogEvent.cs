using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class UpdateDraftEventLogEvent : LogEvent
    {
        private readonly Guid? activeEventId;

        public DraftEventType DraftEventType { get; }
        public string SetCode { get; }

        public bool HasActiveDraftEventEntry => activeEventId != null;
        public Guid ActiveDraftEventEntryId => activeEventId ?? throw new InvalidOperationException($"Value of '{nameof(ActiveDraftEventEntryId)}' is not defined. Please check the value of '{nameof(HasActiveDraftEventEntry)}' before accessing this property.");

        private UpdateDraftEventLogEvent(DraftEventType draftEventType, string setCode, Guid? activeEventId)
        {
            DraftEventType      = draftEventType;
            SetCode             = setCode;
            this.activeEventId  = activeEventId;
        }

        public static IEnumerable<UpdateDraftEventLogEvent> FromJson(JsonElement jsonElement)
        {
            var payloadElements = jsonElement.GetProperty("payload").EnumerateArray();
            foreach (var payloadElement in payloadElements)
            {
                var draftIdElement = payloadElement.GetProperty("InternalEventName")
                .GetString();
                var regex = Regex.Match(draftIdElement, @"^(\w*?)_(\w{0,3})_\d*$");
                if (!regex.Success)
                    continue;

                var rawEventType = regex.Groups[1].Value;
                if (!rawEventType.Contains("Draft"))
                    continue;

                var draftEventType = rawEventType switch
                {
                    "CompDraft" => DraftEventType.Traditional,
                    "QuickDraft" => DraftEventType.Ranked,
                    _ => throw new NotSupportedException($"Event type '{rawEventType}' is not supported.")
                };

                var setCode = regex.Groups[2].Value;

                Guid? activeEventEntryId = payloadElement.GetProperty("Id").GetGuid();
                if (activeEventEntryId == Guid.Empty)
                    activeEventEntryId = null;

                yield return new UpdateDraftEventLogEvent(draftEventType, setCode, activeEventEntryId);
            }
        }
    }
}
