using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class DraftPickOptionsLogEvent : LogEvent
    {
        public DraftEventType DraftEventType { get; }
        public string SetCode { get; }
        public int PackNumber { get; }
        public int PickNumber { get; }
        public IEnumerable<int> DraftPackCardIds { get; }

        private DraftPickOptionsLogEvent(DraftEventType draftEventType, string setCode, int packNumber, int pickNumber, IEnumerable<int> draftPackCardIds)
        {
            DraftEventType      = draftEventType;
            SetCode             = setCode;
            PackNumber          = packNumber;
            PickNumber          = pickNumber;
            DraftPackCardIds    = draftPackCardIds.Enumerate();
        }

        public static DraftPickOptionsLogEvent FromJsonDocument(JsonElement jsonElement)
        {
            var payloadElement = jsonElement.GetProperty("payload");

            var draftIdElement = payloadElement.GetProperty("DraftId")
                .GetString();
            var regex = Regex.Match(draftIdElement, @"^([\w:\d]*?):(\w*?)_(\w{0,3})_\d*:(\w*?)$");
            if (!regex.Success)
                throw new Exception($"Regex failure while creating '{nameof(DraftPickOptionsLogEvent)}' with unmatchable value '{draftIdElement}'.");

            var rawEventType = regex.Groups[2].Value;
            var draftEventType = rawEventType switch
            {
                "CompDraft" => DraftEventType.Traditional,
                "QuickDraft" => DraftEventType.Ranked,
                _ => throw new NotSupportedException($"Event type '{rawEventType}' is not supported.")
            };

            var setCode = regex.Groups[3].Value;

            var packNumber = payloadElement.GetProperty("PackNumber").GetInt32() + 1;
            var pickNumber = payloadElement.GetProperty("PickNumber").GetInt32() + 1;

            var draftPackProperty = payloadElement.GetProperty("DraftPack");

            var draftPackCardIds = draftPackProperty.ValueKind == JsonValueKind.Null ?
                Enumerable.Empty<int>() : 
                draftPackProperty.EnumerateArray()
                    .Select(jsonElement => Convert.ToInt32(jsonElement.GetString()));
            
            return new DraftPickOptionsLogEvent(draftEventType, setCode, packNumber, pickNumber, draftPackCardIds);
        }

        public bool MatchesSelectionLogEvent(DraftPickSelectionLogEvent selectionLogEvent)
            => DraftEventType == selectionLogEvent.DraftEventType &&
                SetCode == selectionLogEvent.SetCode &&
                PackNumber == selectionLogEvent.PackNumber &&
                PickNumber == selectionLogEvent.PickNumber &&
                DraftPackCardIds.Contains(selectionLogEvent.SelectedCardId);
    }
}
