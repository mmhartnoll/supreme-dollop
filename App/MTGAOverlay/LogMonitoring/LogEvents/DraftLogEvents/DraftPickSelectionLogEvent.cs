using MindSculptor.Tools;
using System;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.DraftLogEvents
{
    internal class DraftPickSelectionLogEvent : LogEvent
    {
        public string MtgaEventId { get; }
        public int PackNumber { get; }
        public int PickNumber { get; }
        public int SelectedCardId { get; }

        private DraftPickSelectionLogEvent(string mtgaInternalEventName, int packNumber, int pickNumber, int selectedCardId)
        {
            MtgaEventId    = mtgaInternalEventName;
            PackNumber     = packNumber;
            PickNumber     = pickNumber;
            SelectedCardId = selectedCardId;
        }

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var requestElement  = jsonElement.GetProperty("request");
            var requestDocument = JsonDocument.Parse(requestElement.GetString());
            var paramsElement   = requestDocument.RootElement.GetProperty("params");

            var draftIdElement = paramsElement.GetProperty("draftId").GetString();
            var regex = Regex.Match(draftIdElement, @"^[^:]*:([^_]*_[^_]*)_[^:]*:\w*$");
            if (regex.Success)
            {
                var mtgaEventId    = regex.Groups[1].Value;
                var packNumber     = Convert.ToInt32(paramsElement.GetProperty("packNumber").GetString()) + 1;
                var pickNumber     = Convert.ToInt32(paramsElement.GetProperty("pickNumber").GetString()) + 1;
                var selectedCardId = Convert.ToInt32(paramsElement.GetProperty("cardId").GetString());

                result = new DraftPickSelectionLogEvent(mtgaEventId, packNumber, pickNumber, selectedCardId);
                return true;
            }
            result = null;
            return false;
        }
    }
}
