using System;

namespace MindSculptor.App.MtgaOverlay.GUI.BrowserWindow.ViewModels
{
    internal class DraftEventEntryViewModel : ViewModel
    {
        public Guid Id { get; }
        public bool IsCompleted { get; }
        public int MatchWinCount { get; }
        public int MatchLossCount { get; }
        public int Ordinal { get; }

        private DraftEventEntryViewModel(Guid id, bool isCompleted, int matchWinCount, int matchLossCount, int ordinal)
        {
            Id = id;
            IsCompleted = isCompleted;
            MatchWinCount = matchWinCount;
            MatchLossCount = matchLossCount;
            Ordinal = ordinal;
        }
    }
}
