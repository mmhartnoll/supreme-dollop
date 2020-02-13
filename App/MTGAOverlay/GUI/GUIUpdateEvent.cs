using System;

namespace MindSculptor.App.MtgaOverlay.GUI
{
    internal class GUIUpdateEvent
    {
        private readonly Action<MainWindow> action;

        public GUIUpdateEvent(Action<MainWindow> action)
            => this.action = action;

        public void Invoke(MainWindow window)
            => action.Invoke(window);
    }

    internal class LogUpdateEvent : GUIUpdateEvent
    {
        public LogUpdateEvent(string logMessage)
            : base(async mainWindow => await mainWindow.LogMessage(logMessage).ConfigureAwait(false)) { }
    }
}
