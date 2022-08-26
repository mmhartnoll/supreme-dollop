using System;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class LogMessage
    {
        public string Timestamp { get; }
        public string Message { get; }

        public LogMessage(string message)
        {
            Timestamp = DateTime.Now.ToLongTimeString();
            Message = message;
        }
    }
}
