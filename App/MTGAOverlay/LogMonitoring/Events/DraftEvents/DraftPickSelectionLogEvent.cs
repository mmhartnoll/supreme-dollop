using MindSculptor.Tools;
using System;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.Events.DraftEvents
{
    internal class DraftPickSelectionLogEvent : LogEvent
    {
        public int PackNumber { get; }
        public int PickNumber { get; }
        public int SelectedCardId { get; }

        private DraftPickSelectionLogEvent(int packNumber, int pickNumber, int selectedCardId)
        {
            PackNumber = packNumber;
            PickNumber = pickNumber;
            SelectedCardId = selectedCardId;
        }

        public static bool TryParseFromBotDraftFormat(JsonElement jsonElement, out NullableReference<DraftPickSelectionLogEvent> result)
        {
            var requestElement = jsonElement.GetProperty("request");
            var requestDocument = JsonDocument.Parse(requestElement.GetString());
            var paramsElement = requestDocument.RootElement.GetProperty("params");

            var packNumber = Convert.ToInt32(paramsElement.GetProperty("packNumber").GetString()) + 1;
            var pickNumber = Convert.ToInt32(paramsElement.GetProperty("pickNumber").GetString()) + 1;
            var selectedCardId = Convert.ToInt32(paramsElement.GetProperty("cardId").GetString());

            result = new DraftPickSelectionLogEvent(packNumber, pickNumber, selectedCardId);
            return true;
        }

        public static bool TryParseFromHumanDraftFormat(JsonElement jsonElement, out NullableReference<DraftPickSelectionLogEvent> result)
        {
            var requestElement = jsonElement.GetProperty("request");
            var requestDocument = JsonDocument.Parse(requestElement.GetString());
            var paramsElement = requestDocument.RootElement.GetProperty("params");

            var packNumber = Convert.ToInt32(paramsElement.GetProperty("packNumber").GetString());
            var pickNumber = Convert.ToInt32(paramsElement.GetProperty("pickNumber").GetString());
            var selectedCardId = Convert.ToInt32(paramsElement.GetProperty("cardId").GetString());

            result = new DraftPickSelectionLogEvent(packNumber, pickNumber, selectedCardId);
            return true;
        }
    }
}
