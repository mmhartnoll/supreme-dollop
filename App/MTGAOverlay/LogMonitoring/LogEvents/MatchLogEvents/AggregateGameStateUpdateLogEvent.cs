using MindSculptor.Tools;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents
{
    internal class AggregateGameStateUpdateLogEvent : LogEvent
    {
        public IEnumerable<LogEvent> LogEvents { get; }

        private AggregateGameStateUpdateLogEvent(IEnumerable<LogEvent> logEvents)
            => LogEvents = logEvents;

        public static bool TryCreateFromFactory(GameStateUpdateLogEventFactory factory, out NullableReference<LogEvent> result)
        {
            if (factory.QueuedGameStateUpdateLogEvents.HasValue && factory.QueuedGameStateUpdateLogEvents.Value.Any())
            {
                result = new AggregateGameStateUpdateLogEvent(factory.QueuedGameStateUpdateLogEvents.Value);
                factory.InvalidateQueuedGameStateUpdateEvents();
                return true;
            }
            result = null;
            return false;
        }
    }
}
