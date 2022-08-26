using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents;
using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class MatchOverlayModel : Model
    {
        private readonly CardModelCache  cardModelCache;
        private readonly ProfileModel    profileModel;
        private readonly EventModel      eventModel;
        private readonly EventEntryModel eventEntryModel;
        private readonly EventMatchModel eventMatchModel;
        private readonly IDictionary<int, ResultType> gameResults = new Dictionary<int, ResultType>();

        private IEnumerable<MatchCardView>          libraryConfiguration   = new List<MatchCardView>();
        private IEnumerable<MatchCardView>          sideboardConfiguration = new List<MatchCardView>();
        private GameState                           gameState;
        private GameStateCardPool                   gameStateCardPool;
        private ObservableCollection<DeckCardCount> deckCardCounts         = new ObservableCollection<DeckCardCount>();
        private int                                 currentDeckCount;
        private bool                                isContentVisible       = false;
        private bool                                hasErrorOccurred       = true;

        public string MtgaEventId => eventModel.MtgaEventId; 
        public Guid EventEntryId => eventEntryModel.Id;
        public Guid MatchId => eventMatchModel.Id;
        public int GameNumber { get; private set; }
        public Player PlayerOne { get; }
        public Player PlayerTwo { get; }
        public string DeckName { get; }
        public ObservableCollection<DeckCardCount> DeckCardCounts
        {
            get => deckCardCounts;
            private set => SetProperty(ref deckCardCounts, value, nameof(DeckCardCounts));
        }
        public int CurrentDeckCount
        {
            get => currentDeckCount;
            private set => SetProperty(ref currentDeckCount, value, nameof(CurrentDeckCount));
        }
        public bool IsContentVisible
        {
            get => isContentVisible;
            private set => SetProperty(ref isContentVisible, value, nameof(IsContentVisible));
        }
        public bool HasErrorOccurred
        {
            get => hasErrorOccurred;
            private set => SetProperty(ref hasErrorOccurred, value, nameof(HasErrorOccurred));
        }

        public event AsyncEventHandler MatchCompletedAsync = delegate { return Task.CompletedTask; };

        public MatchOverlayModel(DataContext dataContext, CardModelCache cardModelCache, ProfileModel profileModel, EventModel eventModel, EventEntryModel eventEntryModel, EventMatchModel eventMatchModel, Player playerOne, Player playerTwo,
            string deckName, IEnumerable<MatchCardView> deckConfiguration, IEnumerable<MatchCardView> sideboardConfiguration) : base(dataContext)
        {
            this.cardModelCache         = cardModelCache;
            this.profileModel           = profileModel;
            this.eventModel             = eventModel;
            this.eventEntryModel        = eventEntryModel;
            this.eventMatchModel        = eventMatchModel;
            PlayerOne                   = playerOne;
            PlayerTwo                   = playerTwo;
            DeckName                    = deckName;
            this.libraryConfiguration      = deckConfiguration;
            this.sideboardConfiguration = sideboardConfiguration;

            var deckCardCounts = new List<DeckCardCount>();
            foreach (var deckCard in deckConfiguration.Distinct(new CardViewNameComparer()).OrderBy(x => x))
            {
                var totalCount = deckConfiguration.Where(card => card.Name == deckCard.Name).Count();
                deckCardCounts.Add(new DeckCardCount(deckCard, totalCount));
            }

            DeckCardCounts = new ObservableCollection<DeckCardCount>(deckCardCounts);
        }

        public async static Task<MatchOverlayModel> LoadAsync(DataContext dataContext, CardModelCache cardModelCache, ProfileModel profileModel, EventModel eventModel, EventEntryModel eventEntryModel, MatchCreationLogEvent logEvent)
        {
            var transactionScope = await dataContext.BeginTransactionAsync().ConfigureAwait(false);
            await using (transactionScope.ConfigureAwait(false))
                try
                {
                    var result = await transactionScope.ExecuteAsync(TransactionScope).ConfigureAwait(false);
                    await transactionScope.CommitAsync().ConfigureAwait(false);
                    return result;
                }
                catch
                {
                    await transactionScope.RollbackAsync().ConfigureAwait(false);
                    throw;
                }

            async Task<MatchOverlayModel> TransactionScope()
            {
                var playerOne = await Player.LoadOrCreateAsync(dataContext, logEvent.PlayerOneMtgaUserId, logEvent.PlayerOneName).ConfigureAwait(false);
                var playerTwo = await Player.LoadOrCreateAsync(dataContext, logEvent.PlayerTwoMtgaUserId, logEvent.PlayerTwoName).ConfigureAwait(false);
                var opponent = profileModel.MtgaUserId == playerOne.MtgaPlayerId ? playerOne : playerTwo;
                var eventMatchModel = await eventEntryModel.LoadOrCreateMatchAsync(logEvent.MatchId, opponent).ConfigureAwait(false);

                var deckConfiguration = await logEvent.DeckCardCounts
                    .SelectMany(cardCount => Enumerable.Repeat(cardCount.Key, cardCount.Value))
                    .SelectAsync(mtgaCardId => cardModelCache.GetCardByMtgaCardId(mtgaCardId))
                    .SelectAsync(cardModel => MatchCardView.LoadAsync(cardModel))
                    .ToListAsync()
                    .ConfigureAwait(false);
                var sideboardConfiguration = await logEvent.SideboardCardCounts
                    .SelectMany(cardCount => Enumerable.Repeat(cardCount.Key, cardCount.Value))
                    .SelectAsync(mtgaCardId => cardModelCache.GetCardByMtgaCardId(mtgaCardId))
                    .SelectAsync(cardModel => MatchCardView.LoadAsync(cardModel))
                    .ToListAsync()
                    .ConfigureAwait(false);

                var model = new MatchOverlayModel(dataContext, cardModelCache, profileModel, eventModel, eventEntryModel, eventMatchModel, playerOne, playerTwo, logEvent.DeckName, deckConfiguration, sideboardConfiguration);
                eventMatchModel.LogMessageAsync += model.OnLogMessageAsync;
                eventMatchModel.LogErrorMessageAsync += model.OnLogErrorMessageAsync;

                return model;
            }
        }

        public async Task UpdateDeckConfigurationAsync(MatchDeckConfigurationUpdateLogEvent logEvent)
        {
            libraryConfiguration = await logEvent.DeckCardIds
                .SelectAsync(mtgaCardId => cardModelCache.GetCardByMtgaCardId(mtgaCardId))
                .SelectAsync(cardModel => MatchCardView.LoadAsync(cardModel))
                .ToListAsync()
                .ConfigureAwait(false);
            sideboardConfiguration = await logEvent.SideboardCardIds
                .SelectAsync(mtgaCardId => cardModelCache.GetCardByMtgaCardId(mtgaCardId))
                .SelectAsync(cardModel => MatchCardView.LoadAsync(cardModel))
                .ToListAsync()
                .ConfigureAwait(false);

            var deckCardCounts = new List<DeckCardCount>();
            foreach (var deckCard in libraryConfiguration.Distinct(new CardViewNameComparer()).OrderBy(x => x))
            {
                var totalCount = libraryConfiguration.Where(card => card.Name == deckCard.Name).Count();
                deckCardCounts.Add(new DeckCardCount(deckCard, totalCount));
            }

            DeckCardCounts = new ObservableCollection<DeckCardCount>(deckCardCounts);
            CurrentDeckCount = DeckCardCounts.Aggregate(0, (accum, cardCount) => accum + cardCount.Count);

            await OnLogMessageAsync("Match deck configuration updated.").ConfigureAwait(false);
        }

        public async Task UpdateGameStateAsync(GameStateUpdateLogEvent logEvent)
        {
            if (MatchId != logEvent.MatchId)
            {
                await OnLogErrorMessageAsync("MatchId does not match that of GameStateUpdateLogEvent.").ConfigureAwait(false);
                IsModelValid = false;
            }
            if (!IsModelValid)
                return;

            if (logEvent.GameState.GameStateId == 1)
            {
                gameState = new GameState();
                gameStateCardPool = new GameStateCardPool(libraryConfiguration, sideboardConfiguration);
            }
            if (logEvent.GameState.PreviousGameStateId != gameState.GameStateId)
            {
                gameState.RevertGameState(logEvent.GameState.PreviousGameStateId);
                gameStateCardPool.RevertGameState(logEvent.GameState.PreviousGameStateId);
            }

            await (logEvent.GameStage switch
            {
                GameStage.Start    => ProcessGameStageStartAsync(),
                GameStage.Play     => ProcessGameStagePlayAsync(),
                GameStage.Complete => ProcessGameStageCompleteAsync(),
                _                  => throw new InvalidOperationException()
            })
            .ConfigureAwait(false);

            Task ProcessGameStageStartAsync()
            {
                gameState.SetNextGameState();
                UpdateLocalGameState(gameState, logEvent.GameState);

                gameStateCardPool.SetNextGameState();
                gameStateCardPool.SetDefaultClassifications();

                var handInstances = gameState.ObjectInstances.Values.Where(instance => 
                    !instance.IsDeleted && 
                    instance.Zone == logEvent.PlayerHandZone);
                foreach (var instance in handInstances)
                {
                    var card = gameStateCardPool.CardStates.FirstOrDefault(state =>
                        state.Classification == GameStateCardClassification.TrackedLibrary &&
                        state.Card.MtgaCardId == instance.MtgaCardId);
                    if (card == null)
                        throw new Exception("Failed to find card with matching Mtga card id in tracked library zone.");
                    card.Classification = GameStateCardClassification.NotTracked;
                }
                return Task.CompletedTask;
            }

            Task ProcessGameStagePlayAsync()
            {
                gameState.SetNextGameState();
                UpdateLocalGameState(gameState, logEvent.GameState);

                gameStateCardPool.SetNextGameState();
                UpdateGameStateCardPool();

                return Task.CompletedTask;
            }

            async Task ProcessGameStageCompleteAsync()
            {

            }

            static void UpdateLocalGameState(GameState localGameState, GameState referenceGameState)
            {
                var referenceInstancesToUpdate = referenceGameState.ObjectInstances.Values
                    .Where(instance => instance.IsNew || instance.IsUpdated)
                    .OrderBy(instance => instance.InstanceId);
                foreach (var referenceInstance in referenceInstancesToUpdate)
                {
                    if (!localGameState.ObjectInstances.TryGetValue(referenceInstance.InstanceId, out var localInstance))
                        localInstance = localGameState.CreateNewInstance(referenceInstance.InstanceId, referenceInstance.OwnerId, referenceInstance.ControllerId, referenceInstance.Zone);
                    UpdateLocalInstance(localInstance, referenceInstance);
                }

                void UpdateLocalInstance(GameStateObjectInstance localInstance, GameStateObjectInstance referenceInstance)
                {
                    localInstance.ControllerId = referenceInstance.ControllerId;
                    if (localInstance.Zone != referenceInstance.Zone)
                    {
                        var originalZone = localInstance.Zone;
                        localInstance.Zone = referenceInstance.Zone;
                    }
                    if (referenceInstance.IsSubZoneSpecified)
                    {
                        localInstance.SubZone = referenceInstance.SubZone;
                        localInstance.SubZoneOrdinal = referenceInstance.SubZoneOrdinal;
                        if (referenceInstance.IsSubZoneInternallyOrdered)
                            localInstance.SubZoneInternalOrdinal = referenceInstance.SubZoneInternalOrdinal;
                    }
                    if (referenceInstance.IsObjectKnown && !localInstance.IsObjectKnown)
                        localInstance.MtgaCardId = referenceInstance.MtgaCardId;
                    if (referenceInstance.HasPreviousInstance && !localInstance.HasPreviousInstance)
                    {
                        if (!localGameState.ObjectInstances.TryGetValue(referenceInstance.PreviousInstance.InstanceId, out var localPreviousInstance))
                            throw new Exception("Failed to find local previous instance.");
                        localInstance.PreviousInstance = localPreviousInstance;
                    }
                }
            }

            void UpdateGameStateCardPool()
            {
                var updatedInstances = gameState.ObjectInstances.Values
                    .Where(instance => instance.OwnerId == logEvent.PlayerSeatId && (instance.IsNew || instance.IsUpdated))
                    .OrderBy(instance => instance.InstanceId);

                foreach (var instance in updatedInstances)
                    if (instance.HasPreviousState)
                        ProcessObjectStateChange(instance.PreviousState, instance.AsReadOnlyObjectState());
                    else if (instance.HasPreviousInstance)
                        ProcessObjectStateChange(instance.PreviousInstance.AsReadOnlyObjectState(), instance.AsReadOnlyObjectState());
                    else
                        ProcessNewObjectState(instance.AsReadOnlyObjectState());

                void ProcessNewObjectState(IReadOnlyObjectState state)
                {
                    // Nothing should need to be handled in this case.
                }

                void ProcessObjectStateChange(IReadOnlyObjectState previousState, IReadOnlyObjectState currentState)
                {
                    if (currentState.IsDeleted)
                    {
                        // The instance was deleted. We only need to act if the instance was known and was untracked.
                        if (previousState.IsObjectKnown && !previousState.Zone.In(logEvent.PlayerLibraryZone, logEvent.PlayerSideboardZone))
                        {
                            // Instance was known and not in a tracked zone. We must track the instance.
                            // Cards cannot move into the sideboard from any other zone, so we track it in the library.
                            var card = FindCardInCardPool(previousState.MtgaCardId, GameStateCardClassification.NotTracked);
                            card.Classification = GameStateCardClassification.TrackedLibrary;
                        }
                    }
                    else if (previousState.IsObjectKnown != currentState.IsObjectKnown)
                    {
                        // The instance has been revealed or hidden. We only need to act on this if the instance was untracked.
                        if (!currentState.Zone.In(logEvent.PlayerLibraryZone, logEvent.PlayerSideboardZone))
                            if (currentState.IsObjectKnown)
                            {
                                // The instance has been revealed. We no longer need to track it.
                                var previousClassification = previousState.Zone == logEvent.PlayerSideboardZone ? GameStateCardClassification.TrackedSideboard : GameStateCardClassification.TrackedLibrary;
                                var card = FindCardInCardPool(currentState.MtgaCardId, previousClassification);
                                card.Classification = GameStateCardClassification.NotTracked;
                            }
                            else
                            {
                                // The instance has been hidden. We need to track it.
                                // Cards cannot move into the sideboard from any other zone, so we track it in the library.
                                var card = FindCardInCardPool(previousState.MtgaCardId, GameStateCardClassification.NotTracked);
                                card.Classification = GameStateCardClassification.TrackedLibrary;
                            }
                    }
                    else if (currentState.IsObjectKnown)
                    {
                        // Instance remains known. We only need to act if the instance has changed zones to or from the library or sideboard.
                        if (previousState.Zone != currentState.Zone)
                            if (previousState.Zone == logEvent.PlayerSideboardZone)
                            {
                                var card = FindCardInCardPool(currentState.MtgaCardId, GameStateCardClassification.TrackedSideboard);
                                card.Classification = currentState.Zone == logEvent.PlayerLibraryZone ? GameStateCardClassification.TrackedLibrary : GameStateCardClassification.NotTracked;
                            }
                            else if (previousState.Zone == logEvent.PlayerLibraryZone)
                            {
                                var card = FindCardInCardPool(currentState.MtgaCardId, GameStateCardClassification.TrackedLibrary);
                                card.Classification = GameStateCardClassification.NotTracked;
                            }
                            else if (currentState.Zone == logEvent.PlayerLibraryZone)
                            {
                                var card = FindCardInCardPool(currentState.MtgaCardId, GameStateCardClassification.NotTracked);
                                card.Classification = GameStateCardClassification.TrackedLibrary;
                            }
                    }
                }
            }

            GameStateCard FindCardInCardPool(int mtgaCardId, GameStateCardClassification classification)
            {
                var potentials = gameStateCardPool.CardStates.Where(cardState => cardState.Card.MtgaCardId == mtgaCardId).ToList();

                var card = gameStateCardPool.CardStates.FirstOrDefault(cardState => cardState.Card.MtgaCardId == mtgaCardId && cardState.Classification == classification);
                return card ?? throw new Exception("Failed to find a card state of the specified classification matching the specified card id.");
            }

            #region ObsoleteNew

            //if (logEvent.GameStateType == GameStateType.Full)
            //{
            //    revealedCardLookup.Clear();
            //    deckCards = new List<MatchCardView>(deckConfiguration);
            //    sideboardCards = new List<MatchCardView>(sideboardConfiguration);
            //    deckInstanceIds = logEvent.PlayerDeckInstanceIds.ToList();
            //    IsModelValid = true;

            //    foreach (var instanceIdMtgaCardMapping in logEvent.InstanceIdMtgaCardIdMap)
            //    {
            //        var card = deckCards.FirstOrDefault(card => card.MtgaCardId == instanceIdMtgaCardMapping.Value);
            //        if (card != null)
            //        {
            //            deckCards.Remove(card);
            //            revealedCardLookup.Add(instanceIdMtgaCardMapping.Key, card);
            //        }
            //        else
            //        {
            //            IsModelValid = false;
            //            await OnLogErrorMessageAsync("Failed to find a value for instance id in mapping.").ConfigureAwait(false);
            //            return;
            //        }
            //    }
            //    if (!deckInstanceIds.OrderBy(x => x).SequenceEqual(logEvent.PlayerDeckInstanceIds.OrderBy(x => x)))
            //    {
            //        IsModelValid = false;
            //        await OnLogErrorMessageAsync("Deck instance ids are inconsistent with those specified in log event.");
            //        return;
            //    }

            //    foreach (var cardCount in DeckCardCounts)
            //        cardCount.Count = deckCards.Where(card => card.Name == cardCount.Card.Name).Count();
            //    CurrentDeckCount = DeckCardCounts.Aggregate(0, (accum, cardCount) => accum + cardCount.Count);
            //    IsContentVisible = true;
            //}
            //else if (logEvent.GameStateType == GameStateType.Difference && IsModelValid)
            //{
            //    foreach (var instanceIdChange in logEvent.InstanceIdChanges)
            //        if (deckInstanceIds.Contains(instanceIdChange.Key))
            //        {
            //            deckInstanceIds.Remove(instanceIdChange.Key);
            //            deckInstanceIds.Add(instanceIdChange.Value);
            //        }
            //        else if (revealedCardLookup.TryGetValue(instanceIdChange.Key, out var card))
            //        {
            //            revealedCardLookup.Remove(instanceIdChange.Key);
            //            revealedCardLookup.Add(instanceIdChange.Value, card);
            //        }

            //    foreach (var instanceId in logEvent.PlayerDeckInstanceIds.Except(deckInstanceIds).ToList())
            //        if (revealedCardLookup.TryGetValue(instanceId, out var card))
            //        {
            //            revealedCardLookup.Remove(instanceId);
            //            deckCards.Add(card);
            //            deckInstanceIds.Add(instanceId);
            //        }
            //        else
            //        {
            //            IsModelValid = false;
            //            await OnLogErrorMessageAsync("Failed to find a revealed card matching instance id.").ConfigureAwait(false);
            //            return;
            //        }

            //    foreach (var instanceId in deckInstanceIds.Except(logEvent.PlayerDeckInstanceIds).ToList())
            //        if (logEvent.InstanceIdMtgaCardIdMap.TryGetValue(instanceId, out var mtgaCardId))
            //        {
            //            var card = deckCards.FirstOrDefault(card => card.MtgaCardId == mtgaCardId);
            //            if (card != null)
            //            {
            //                deckCards.Remove(card);
            //                deckInstanceIds.Remove(instanceId);
            //                revealedCardLookup.Add(instanceId, card);
            //            }
            //            else
            //            {
            //                IsModelValid = false;
            //                await OnLogErrorMessageAsync("Failed to find card matching id in current deck.").ConfigureAwait(false);
            //                return;
            //            }
            //        }
            //        else
            //        {
            //            IsModelValid = false;
            //            await OnLogErrorMessageAsync("Failed to find a value for instance id in mapping.").ConfigureAwait(false);
            //            return;
            //        }

            //    if (!deckInstanceIds.OrderBy(x => x).SequenceEqual(logEvent.PlayerDeckInstanceIds.OrderBy(x => x)))
            //    {
            //        IsModelValid = false;
            //        await OnLogErrorMessageAsync("Deck instance ids are inconsistent with those specified in log event.");
            //        return;
            //    }

            //    foreach (var cardCount in DeckCardCounts)
            //        cardCount.Count = deckCards.Where(card => card.Name == cardCount.Card.Name).Count();
            //    CurrentDeckCount = DeckCardCounts.Aggregate(0, (accum, cardCount) => accum + cardCount.Count);

            //    IsContentVisible = true;
            //}
            //if (logEvent.GameStage == GameStage.Complete)
            //{
            //    IsContentVisible = false;

            //    foreach (var gameNumber in logEvent.GameResults.Keys)
            //    {
            //        var result = logEvent.GameResults[gameNumber];
            //        if (gameResults.ContainsKey(gameNumber) && gameResults[gameNumber] != result)
            //        {
            //            IsModelValid = false;
            //            await OnLogErrorMessageAsync("Logged game result conflicts with previously logged game result.").ConfigureAwait(false);
            //            return;
            //        }
            //        else if (!gameResults.ContainsKey(gameNumber))
            //        {
            //            var gameWon = result == ResultType.Win;
            //            await eventMatchModel.SetGameResult(gameNumber, true, gameWon).ConfigureAwait(false);
            //            gameResults[gameNumber] = result;
            //        }
            //    }
            //    if (logEvent.HasMatchResult)
            //    {
            //        var matchWon = logEvent.MatchResult == ResultType.Win;
            //        await eventMatchModel.SetMatchResult(matchWon).ConfigureAwait(false);
            //        await MatchCompletedAsync.InvokeAsync(this, EventArgs.Empty).ConfigureAwait(false);
            //    }
            //}

            #endregion

            #region Obsolete

            //async Task ProcessGameStageStartAsync()
            //{
            //    revealedCardLookup.Clear();
            //    deckCards = new List<MatchCardView>(deckConfiguration);
            //    sideboardCards = new List<MatchCardView>(sideboardConfiguration);
            //    deckInstanceIds = logEvent.PlayerDeckInstanceIds.ToList();
            //    IsModelValid = true;

            //    foreach (var instanceId in logEvent.PlayerHandInstanceIds)
            //    {
            //        var mtgaCardId = logEvent.InstanceIdMtgaCardIdMap[instanceId];
            //        var card = deckCards.FirstOrDefault(card => card.MtgaCardId == mtgaCardId);
            //        if (card != null)
            //        {
            //            deckCards.Remove(card);
            //            revealedCardLookup.Add(instanceId, card);
            //        }
            //        else
            //        {
            //            IsModelValid = false;
            //            await OnLogErrorMessageAsync("Failed to find a value for instance id in mapping.").ConfigureAwait(false);
            //            return;
            //        }
            //    }
            //    if (!deckInstanceIds.OrderBy(x => x).SequenceEqual(logEvent.PlayerDeckInstanceIds.OrderBy(x => x)))
            //    {
            //        IsModelValid = false;
            //        await OnLogErrorMessageAsync("Deck instance ids are inconsistent with those specified in log event.");
            //        return;
            //    }

            //    foreach (var cardCount in DeckCardCounts)
            //        cardCount.Count = deckCards.Where(card => card.Name == cardCount.Card.Name).Count();
            //    CurrentDeckCount = DeckCardCounts.Aggregate(0, (accum, cardCount) => accum + cardCount.Count);
            //    IsContentVisible = true;
            //}

            //async Task ProcessGameStagePlayAsync()
            //{
            //    if (IsModelValid)
            //    {
            //        foreach (var instanceIdChange in logEvent.InstanceIdChanges)
            //            if (deckInstanceIds.Contains(instanceIdChange.Key))
            //            {
            //                deckInstanceIds.Remove(instanceIdChange.Key);
            //                deckInstanceIds.Add(instanceIdChange.Value);
            //            }
            //            else if (revealedCardLookup.TryGetValue(instanceIdChange.Key, out var card))
            //            {
            //                revealedCardLookup.Remove(instanceIdChange.Key);
            //                revealedCardLookup.Add(instanceIdChange.Value, card);
            //            }

            //        foreach (var instanceId in logEvent.PlayerDeckInstanceIds.Except(deckInstanceIds).ToList())
            //            if (revealedCardLookup.TryGetValue(instanceId, out var card))
            //            {
            //                revealedCardLookup.Remove(instanceId);
            //                deckCards.Add(card);
            //                deckInstanceIds.Add(instanceId);
            //            }
            //            else
            //            {
            //                IsModelValid = false;
            //                await OnLogErrorMessageAsync("Failed to find a revealed card matching instance id.").ConfigureAwait(false);
            //                return;
            //            }

            //        foreach (var instanceId in deckInstanceIds.Except(logEvent.PlayerDeckInstanceIds).ToList())
            //            if (logEvent.InstanceIdMtgaCardIdMap.TryGetValue(instanceId, out var mtgaCardId))
            //            {
            //                var card = deckCards.FirstOrDefault(card => card.MtgaCardId == mtgaCardId);
            //                if (card != null)
            //                {
            //                    deckCards.Remove(card);
            //                    deckInstanceIds.Remove(instanceId);
            //                    revealedCardLookup.Add(instanceId, card);
            //                }
            //                else
            //                {
            //                    IsModelValid = false;
            //                    await OnLogErrorMessageAsync("Failed to find card matching id in current deck.").ConfigureAwait(false);
            //                    return;
            //                }
            //            }
            //            else
            //            {
            //                IsModelValid = false;
            //                await OnLogErrorMessageAsync("Failed to find a value for instance id in mapping.").ConfigureAwait(false);
            //                return;
            //            }

            //        if (!deckInstanceIds.OrderBy(x => x).SequenceEqual(logEvent.PlayerDeckInstanceIds.OrderBy(x => x)))
            //        {
            //            IsModelValid = false;
            //            await OnLogErrorMessageAsync("Deck instance ids are inconsistent with those specified in log event.");
            //            return;
            //        }

            //        foreach (var cardCount in DeckCardCounts)
            //            cardCount.Count = deckCards.Where(card => card.Name == cardCount.Card.Name).Count();
            //        CurrentDeckCount = DeckCardCounts.Aggregate(0, (accum, cardCount) => accum + cardCount.Count);

            //        IsContentVisible = true;
            //    }
            //}

            //async Task ProccessGameStageCompleteAsync()
            //{
            //    IsContentVisible = false;

            //    foreach (var gameNumber in logEvent.GameResults.Keys)
            //    {
            //        var result = logEvent.GameResults[gameNumber];
            //        if (gameResults.ContainsKey(gameNumber) && gameResults[gameNumber] != result)
            //        {
            //            IsModelValid = false;
            //            await OnLogErrorMessageAsync("Logged game result conflicts with previously logged game result.").ConfigureAwait(false);
            //            return;
            //        }
            //        else if (!gameResults.ContainsKey(gameNumber))
            //        {
            //            var gameWon = result == ResultType.Win;
            //            await eventMatchModel.SetGameResult(gameNumber, true, gameWon).ConfigureAwait(false);
            //            gameResults[gameNumber] = result;
            //        }
            //    }
            //    if (logEvent.HasMatchResult)
            //    {
            //        var matchWon = logEvent.MatchResult == ResultType.Win;
            //        await eventMatchModel.SetMatchResult(matchWon).ConfigureAwait(false);
            //        await MatchCompletedAsync.InvokeAsync(this, EventArgs.Empty).ConfigureAwait(false);
            //    }
            //}

            #endregion
        }

        private void UpdateLocalInstance(GameStateObjectInstance localInstance, GameStateObjectInstance updatedInstance)
        {
            throw new NotImplementedException();
        }

        private class CardViewNameComparer : IEqualityComparer<MatchCardView>
        {
            public bool Equals([AllowNull] MatchCardView x, [AllowNull] MatchCardView y)
            {
                if (x == null && y == null)
                    return true;
                if (x == null || y == null)
                    return false;
                return x.Name == y.Name;
            }

            public int GetHashCode(MatchCardView obj)
                => obj.Name.GetHashCode();
        }
    }
}
