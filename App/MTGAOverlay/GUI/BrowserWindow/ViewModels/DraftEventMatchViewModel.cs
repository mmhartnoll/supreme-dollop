using System;

namespace MindSculptor.App.MtgaOverlay.GUI.BrowserWindow.ViewModels
{
    internal class DraftEventMatchViewModel : ViewModel
    {
        public Guid Id { get; }
        public string OpponentScreenName { get; }
        public int OpponentUserId { get; }
        public bool IsCompleted { get; }
        public int GameWinCount { get; }
        public int GameLossCount { get; }

        private DraftEventMatchViewModel(Guid id, string opponentScreenName, int opponentUserId, bool isCompleted, int gameWinCount, int gameLossCount)
        {
            Id = id;
            OpponentScreenName = opponentScreenName;
            OpponentUserId = opponentUserId;
            IsCompleted = isCompleted;
            GameWinCount = gameWinCount;
            GameLossCount = gameLossCount;
        }
    }
}
