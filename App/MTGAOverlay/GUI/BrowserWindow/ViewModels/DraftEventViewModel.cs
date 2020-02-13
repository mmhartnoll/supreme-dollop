using System;

namespace MindSculptor.App.MtgaOverlay.GUI.BrowserWindow.ViewModels
{
    internal class DraftEventViewModel : ViewModel
    {
        public Guid Id { get; }
        public string EventType { get; }
        public string SetName { get; }
        public int Ordinal { get; }

        private DraftEventViewModel(Guid id, string eventType, string setName, int ordinal)
        {
            Id = id;
            EventType = eventType;
            SetName = setName;
            Ordinal = ordinal;
        }
    }
}
