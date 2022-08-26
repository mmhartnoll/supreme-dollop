using MindSculptor.Tools;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ViewChangeLogEvents
{
    internal class EventLandingViewChangeLogEvent : ViewChangeLogEvent
    {
        public string EventCode { get; }

        protected EventLandingViewChangeLogEvent(ViewType currentViewType, ViewType targetViewType, string eventCode)
            : base(currentViewType, targetViewType)
        {
            EventCode = eventCode;
        }

        public static new bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var payloadElement = jsonElement   .GetProperty("payloadObject");
            var fromSceneName  = payloadElement.GetProperty("fromSceneName").GetString();
            var toSceneName    = payloadElement.GetProperty("toSceneName")  .GetString();
            var eventCode      = payloadElement.GetProperty("context")      .GetString();

            var currentViewType = ParseViewType(fromSceneName);
            var targetViewType  = ParseViewType(toSceneName);

            result = new EventLandingViewChangeLogEvent(currentViewType, targetViewType, eventCode);
            return true;
        }
    }
}
