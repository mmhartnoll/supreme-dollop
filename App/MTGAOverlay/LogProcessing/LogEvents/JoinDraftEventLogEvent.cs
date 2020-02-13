using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class JoinDraftEventLogEvent : LogEvent
    {
        public DraftEventType DraftEventType { get; }
        public string SetCode { get; }
        public Guid DraftEventEntryId { get; }

        private JoinDraftEventLogEvent(DraftEventType draftEventType, string setCode, Guid draftEventEntryId)
        {
            DraftEventType      = draftEventType;
            SetCode             = setCode;
            DraftEventEntryId   = draftEventEntryId;
        }

        public static JoinDraftEventLogEvent FromJson(JsonElement jsonElement)
        {
            var payloadElement = jsonElement.GetProperty("payload");

            var draftIdElement = payloadElement.GetProperty("InternalEventName")
                .GetString();
            var regex = Regex.Match(draftIdElement, @"^(\w*?)_(\w{0,3})_\d*$");
            if (!regex.Success)
                throw new Exception($"Regex failure while creating '{nameof(DraftPickOptionsLogEvent)}' with unmatchable value '{draftIdElement}'.");

            var rawEventType = regex.Groups[1].Value;
            var draftEventType = rawEventType switch
            {
                "CompDraft" => DraftEventType.Traditional,
                "QuickDraft" => DraftEventType.Ranked,
                _ => throw new NotSupportedException($"Event type '{rawEventType}' is not supported.")
            };

            var setCode = regex.Groups[2].Value;

            var eventEntryId = payloadElement.GetProperty("Id").GetGuid();

            return new JoinDraftEventLogEvent(draftEventType, setCode, eventEntryId);
        }
    }
}
