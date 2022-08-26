using MindSculptor.App.MtgaOverlay.DataTypes;

namespace MindSculptor.App.MtgaOverlay.Models.Events
{
    internal class LogMessageEventArgs
    {
        public LogMessage LogMessage { get; }

        public LogMessageEventArgs(LogMessage logMessage)
            => LogMessage = logMessage;
    }
}
