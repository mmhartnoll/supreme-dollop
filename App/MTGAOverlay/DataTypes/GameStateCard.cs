using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class GameStateCard
    {
        private readonly GameStateCardPool gameStateCardPool;
        private readonly Stack<CardState> cardStates = new Stack<CardState>();

        private CardState CurrentState => cardStates.Peek();

        public MatchCardView Card { get; }

        public GameStateCardClassification Classification
        {
            get => CurrentState.Classification;
            set
            {
                if (CurrentState.Classification != value)
                {
                    if (CurrentState.GameStateId != gameStateCardPool.GameStateId)
                        cardStates.Push(CurrentState.CreateNextState(gameStateCardPool.GameStateId));
                    CurrentState.Classification = value;
                }
            }
        }

        public GameStateCard(GameStateCardPool gameStateCardPool, MatchCardView card, GameStateCardClassification classification)
        {
            this.gameStateCardPool = gameStateCardPool;
            Card = card;
            cardStates.Push(CardState.CreateInitialState(gameStateCardPool.GameStateId, classification));
        }

        public void RevertObjectState(int targetGameStateId)
        {
            while (cardStates.Any() && CurrentState.GameStateId > targetGameStateId)
                cardStates.Pop();
        }

        private class CardState
        {
            public int GameStateId { get; }
            public GameStateCardClassification Classification { get; set; }

            private CardState(int gameStateId, GameStateCardClassification classification)
            {
                GameStateId = gameStateId;
                Classification = classification;
            }

            public static CardState CreateInitialState(int currentGameStateId, GameStateCardClassification classification)
                => new CardState(currentGameStateId, classification);

            public CardState CreateNextState(int currentGameStateId)
                => new CardState(currentGameStateId, Classification);
        }
    }
}
