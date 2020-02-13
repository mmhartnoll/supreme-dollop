using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents
{
    internal class DraftPickSelectionLogEvent : LogEvent
    {
        public DraftEventType DraftEventType { get; }
        public string SetCode { get; }
        public int PackNumber { get; }
        public int PickNumber { get; }
        public int SelectedCardId { get; }

        private DraftPickSelectionLogEvent(DraftEventType draftEventType, string setCode, int packNumber, int pickNumber, int selectedCardId)
        {
            DraftEventType  = draftEventType;
            SetCode         = setCode;
            PackNumber      = packNumber;
            PickNumber      = pickNumber;
            SelectedCardId  = selectedCardId;
        }

        public static DraftPickSelectionLogEvent FromJson(JsonElement jsonElement)
        {
            var requestElement = jsonElement.GetProperty("request");
            var requestDocument = JsonDocument.Parse(requestElement.GetString());

            var paramsElement = requestDocument.RootElement.GetProperty("params");

            var draftIdElement = paramsElement.GetProperty("draftId")
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

            var packNumber = Convert.ToInt32(paramsElement.GetProperty("packNumber").GetString()) + 1;
            var pickNumber = Convert.ToInt32(paramsElement.GetProperty("pickNumber").GetString()) + 1;

            var selectedCardId = Convert.ToInt32(paramsElement.GetProperty("cardId").GetString());

            return new DraftPickSelectionLogEvent(draftEventType, setCode, packNumber , pickNumber, selectedCardId);
        }
    }
}
