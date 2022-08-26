using System;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class MatchGame
    {
        public Guid Id { get; }
        public int GameNumber { get; }
        public bool PlayedFirst { get; }
        public bool GameWon { get; }

        public MatchGame(Guid id, int gameNumber, bool playedFirst, bool gameWon)
        {
            Id          = id;
            GameNumber  = gameNumber;
            PlayedFirst = playedFirst;
            GameWon     = gameWon;
        }
    }
}
