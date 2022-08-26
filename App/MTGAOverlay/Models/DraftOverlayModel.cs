using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.DraftLogEvents;
using MindSculptor.Tools.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class DraftOverlayModel : Model
    {
        private readonly CardModelCache        cardModelCache;
        private readonly EventModel            eventModel;
        private readonly ProfileInventoryModel profileInventoryModel;

        private int packNumber;
        private int pickNumber;
        private IEnumerable<DraftPickOption> draftPickOptions = Enumerable.Empty<DraftPickOption>();

        public string MtgaEventId => eventModel.MtgaEventId;
        public int PackNumber 
        { 
            get => packNumber;
            private set => SetProperty(ref packNumber, value, nameof(PackNumber));
        }
        public int PickNumber 
        { 
            get => pickNumber;
            private set => SetProperty(ref pickNumber, value, nameof(PickNumber));
        }
        public IEnumerable<DraftPickOption> DraftPickOptions
        {
            get => draftPickOptions;
            private set => SetProperty(ref draftPickOptions, value, nameof(DraftPickOptions));
        }

        public DraftOverlayModel(DataContext dataContext, CardModelCache cardModelCache, EventModel eventModel, ProfileInventoryModel profileInventoryModel) 
            : base(dataContext)
        {
            this.cardModelCache        = cardModelCache;
            this.eventModel            = eventModel;
            this.profileInventoryModel = profileInventoryModel;
        }

        public async Task SetDraftPickOptions(DraftPickOptionsLogEvent logEvent)
        {
            if (MtgaEventId != logEvent.MtgaEventId)
            {
                IsModelValid = false;
                await OnLogErrorMessageAsync("Error: MtgaEventId does not match for draft pick options.");
            }
            if (IsModelValid)
            {
                var draftPickOptions = new List<DraftPickOption>();
                foreach (var draftPackCardId in logEvent.DraftPackCardIds)
                {
                    var cardInventoryModel = profileInventoryModel.CardInventory.FirstOrDefault(cardInventory => cardInventory.CardModel.MtgaCardId == draftPackCardId);
                    if (cardInventoryModel != null)
                    {
                        var name = await cardInventoryModel.CardModel.GetName().ConfigureAwait(false);
                        var rarity = await cardInventoryModel.CardModel.GetRarityAsync().ConfigureAwait(false);
                        var logicalOrdinal = await cardInventoryModel.CardModel.GetLogicalOrdinalAsync().ConfigureAwait(false);
                        draftPickOptions.Add(new DraftPickOption(cardInventoryModel.CardModel, name, rarity, logicalOrdinal, cardInventoryModel.Count));
                    }
                    else
                    {
                        var cardModel = await cardModelCache.GetCardByMtgaCardId(draftPackCardId).ConfigureAwait(false);
                        var name = await cardModel.GetName().ConfigureAwait(false);
                        var rarity = await cardModel.GetRarityAsync().ConfigureAwait(false);
                        var logicalOrdinal = await cardModel.GetLogicalOrdinalAsync().ConfigureAwait(false);
                        draftPickOptions.Add(new DraftPickOption(cardModel, name, rarity, logicalOrdinal, 0));
                    }
                }
                PackNumber = logEvent.PackNumber;
                PickNumber = logEvent.PickNumber;
                DraftPickOptions = draftPickOptions.OrderBy(option => option.Rarity).ThenBy(option => option.LogicalOrdinal);
                await OnLogMessageAsync($"Displaying draft picks for pack {PackNumber}, pick {PickNumber}.").ConfigureAwait(false);
            }
        }

        public async Task SetDraftPickSelection(DraftPickSelectionLogEvent logEvent)
        {
            if (MtgaEventId != logEvent.MtgaEventId)
            {
                IsModelValid = false;
                await OnLogErrorMessageAsync("Error: MtgaEventId does not match for draft pick selection.");
            }
            if (PackNumber != logEvent.PackNumber || PickNumber != logEvent.PickNumber)
            {
                IsModelValid = false;
                await OnLogErrorMessageAsync("Error: Pack number or pick number does not match for draft pick selection.").ConfigureAwait(false);
            }
            if (IsModelValid)
            {
                var transactionScope = await DataContext.BeginTransactionAsync().ConfigureAwait(false);
                await using (transactionScope.ConfigureAwait(false))
                    try
                    {
                        await transactionScope.ExecuteAsync(TransactionScope).ConfigureAwait(false);
                        await transactionScope.CommitAsync().ConfigureAwait(false);
                    }
                    catch
                    {
                        await transactionScope.RollbackAsync().ConfigureAwait(false);
                        throw;
                    }
            }

            async Task TransactionScope()
            {
                var existingRecord = await DataContext.Mtga.DraftPicks
                    .QueryWhere(record =>
                        record.EventEntryId == eventModel.ActiveEventEntryModel.Id &&
                        record.PackNumber == logEvent.PackNumber &&
                        record.PickNumber == logEvent.PickNumber)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (!existingRecord.Success)
                {
                    var ordinal = 0;
                    foreach (var draftPickOption in DraftPickOptions)
                    {
                        var optionRecord = await DataContext.Mtga.DraftPickOptions
                            .NewRecordAsync(eventModel.Id, eventModel.ActiveEventEntryModel.Id, draftPickOption.CardModel.Id, logEvent.PackNumber, logEvent.PickNumber, ordinal++)
                            .ConfigureAwait(false);
                        if (draftPickOption.CardModel.MtgaCardId == logEvent.SelectedCardId)
                        {
                            var isFifthCopy = draftPickOption.InventoryCount == 4;
                            await DataContext.Mtga.DraftPicks.NewRecordAsync(eventModel.ActiveEventEntryModel.Id, logEvent.PackNumber, logEvent.PickNumber, isFifthCopy).ConfigureAwait(false);
                            if (!isFifthCopy)
                                await profileInventoryModel.AddDraftPick(logEvent.SelectedCardId).ConfigureAwait(false);
                        }
                    }
                    await OnLogMessageAsync($"Saving draft pick for pack {PackNumber}, pick {PickNumber}.").ConfigureAwait(false);
                }
            }
        }
    }
}
