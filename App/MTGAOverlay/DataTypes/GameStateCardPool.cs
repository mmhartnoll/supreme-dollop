using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class GameStateCardPool
    {
        private readonly Lazy<IEnumerable<GameStateCard>> cardStateLoader;
        private readonly IEnumerable<MatchCardView> libraryConfiguration;
        private readonly IEnumerable<MatchCardView> sideboardConfiguration;

        public int GameStateId { get; private set; }
        public IEnumerable<GameStateCard> CardStates => cardStateLoader.Value;

        public GameStateCardPool(IEnumerable<MatchCardView> libraryConfiguration, IEnumerable<MatchCardView> sideboardConfiguration)
        {
            this.libraryConfiguration = libraryConfiguration;
            this.sideboardConfiguration = sideboardConfiguration;
            cardStateLoader = new Lazy<IEnumerable<GameStateCard>>(LoadCardStates);
        }

        public void SetNextGameState()
            => GameStateId++;

        public void SetDefaultClassifications()
        {
            foreach (var cardState in CardStates)
                cardState.Classification = GameStateCardClassification.NotTracked;
            foreach (var card in libraryConfiguration)
            {
                var cardState = CardStates.First(cardState => cardState.Card.MtgaCardId == card.MtgaCardId && cardState.Classification == GameStateCardClassification.NotTracked);
                cardState.Classification = GameStateCardClassification.TrackedLibrary;
            }
            foreach (var card in sideboardConfiguration)
            {
                var cardState = CardStates.First(cardState => cardState.Card.MtgaCardId == card.MtgaCardId && cardState.Classification == GameStateCardClassification.NotTracked);
                cardState.Classification = GameStateCardClassification.TrackedSideboard;
            }
        }

        public void RevertGameState(int targetGameStateId)
        {
            if (targetGameStateId > 0)
            {
                foreach (var cardState in CardStates)
                    cardState.RevertObjectState(targetGameStateId);
                GameStateId = targetGameStateId;
            }
            else
                throw new InvalidOperationException("Target game state id is not in range.");
        }

        private IEnumerable<GameStateCard> LoadCardStates()
        {
            var cardStates = new List<GameStateCard>();
            foreach (var card in libraryConfiguration)
                cardStates.Add(new GameStateCard(this, card, GameStateCardClassification.TrackedLibrary));
            foreach (var card in sideboardConfiguration)
                cardStates.Add(new GameStateCard(this, card, GameStateCardClassification.TrackedSideboard));
            return cardStates.Enumerate();
        }
    }
}
