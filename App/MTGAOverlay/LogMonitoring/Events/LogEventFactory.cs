using System;
using System.Collections.Generic;
using System.Text;

namespace MindSculptor.App.MtgaOverlay.LogMonitoring.Events
{
    internal class LogEventFactory
    {
        private readonly Queue<LogEvent> logEventQueue = new Queue<LogEvent>();

        public void ReadLogMessage(string logMessage)
        {

        }

        public IEnumerable<LogEvent> GetQueuedEvents()
        {
            while (logEventQueue.TryDequeue(out var logEvent))
                yield return logEvent;
        }
    }
}
