using MindSculptor.Tools;
using System.Text.Json;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ViewChangeLogEvents
{
    internal class ViewChangeLogEvent : LogEvent
    {
        public ViewType CurrentViewType { get; }
        public ViewType TargetViewType { get; }

        protected ViewChangeLogEvent(ViewType currentViewType, ViewType targetViewType) 
        {
            CurrentViewType = currentViewType;
            TargetViewType  = targetViewType;
        }

        public static bool TryParse(JsonElement jsonElement, out NullableReference<LogEvent> result)
        {
            var fromSceneName  = jsonElement.GetProperty("fromSceneName").GetString();
            var toSceneName    = jsonElement.GetProperty("toSceneName")  .GetString();

            var currentViewType = ParseViewType(fromSceneName);
            var targetViewType  = ParseViewType(toSceneName);

            return targetViewType switch
            {
                ViewType.EventLanding => EventLandingViewChangeLogEvent.TryParse(jsonElement, out result),
                _                     => Default(out result)
            };

            bool Default(out NullableReference<LogEvent> result)
            {
                result = new ViewChangeLogEvent(currentViewType, targetViewType);
                return true;
            }
        }

        protected static ViewType ParseViewType(string mtgaSceneName)
            => mtgaSceneName switch
            {
                "BoosterChamber" => ViewType.BoosterInventory,
                "EventLanding"   => ViewType.EventLanding,
                "Home"           => ViewType.Home,
                _                => ViewType.Unspecified
            };
    }
}
