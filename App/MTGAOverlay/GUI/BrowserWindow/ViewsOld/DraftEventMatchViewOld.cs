using System;

namespace MindSculptor.App.MtgaOverlay.GUI.BrowserWindow.Views
{
    internal class DraftEventMatchViewOld
    {
        public Guid Id { get; }
        public DateTime MatchCreatedTime { get; }
        public string OpponentScreenName { get; }
        public int OpponentUserId { get; }
        public bool IsCompleted { get; }
        public bool GameWinCount { get; }
        public bool GameLossCount { get; }
    }
}
