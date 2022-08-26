using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ProfileLogEvents;
using MindSculptor.Tools;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class ProfileInventoryModel : Model
    {
        private readonly CardModelCache         cardModelCache;
        private readonly ProfileRecord          profileRecord;
        private readonly ProfileInventoryRecord profileInventoryRecord;

        private readonly IDictionary<int, ProfileHasBoostersRecord> profileHasBoostersRecordsLookup;

        private int     gemCount;
        private int     goldCount;
        private int     commonWildcardCount;
        private int     uncommonWildcardCount;
        private int     rareWildcardCount;
        private int     mythicRareWildcardCount;
        private decimal vaultProgress;

        private IEnumerable<CardInventoryModel>          cardInventory = Enumerable.Empty<CardInventoryModel>();
        private IEnumerable<ItemCount<BoosterInfoModel>> boosterCounts = Enumerable.Empty<ItemCount<BoosterInfoModel>>();

        public int GemCount
        {
            get => gemCount;
            private set => SetProperty(ref gemCount, value, nameof(GemCount));
        }
        public int GoldCount
        {
            get => goldCount;
            private set => SetProperty(ref goldCount, value, nameof(GoldCount));
        }
        public int CommonWildcardCount
        {
            get => commonWildcardCount;
            private set => SetProperty(ref commonWildcardCount, value, nameof(CommonWildcardCount));
        }
        public int UncommonWildcardCount
        {
            get => uncommonWildcardCount;
            private set => SetProperty(ref uncommonWildcardCount, value, nameof(UncommonWildcardCount));
        }
        public int RareWildcardCount
        {
            get => rareWildcardCount;
            private set => SetProperty(ref rareWildcardCount, value, nameof(RareWildcardCount));
        }
        public int MythicRareWildcardCount
        {
            get => mythicRareWildcardCount;
            private set => SetProperty(ref mythicRareWildcardCount, value, nameof(MythicRareWildcardCount));
        }
        public decimal VaultProgress
        {
            get => vaultProgress;
            private set => SetProperty(ref vaultProgress, value, nameof(VaultProgress));
        }

        public IEnumerable<CardInventoryModel> CardInventory
        {
            get => cardInventory;
            private set => SetProperty(ref cardInventory, value, nameof(CardInventory));
        }
        public IEnumerable<ItemCount<BoosterInfoModel>> BoosterCounts
        {
            get => boosterCounts;
            private set => SetProperty(ref boosterCounts, value, nameof(BoosterCounts));
        }

        public event AsyncEventHandler InventoryUpdatedAsync = delegate { return Task.CompletedTask; };

        private ProfileInventoryModel(DataContext dataContext, CardModelCache cardModelCache, ProfileRecord profileRecord, ProfileInventoryRecord profileInventoryRecord, IDictionary<int, ProfileHasBoostersRecord> profileHasBoostersRecordsLookup, 
            IEnumerable<CardInventoryModel> cardInventory, IEnumerable<ItemCount<BoosterInfoModel>> boosterCounts) : base(dataContext)
        {
            this.cardModelCache         = cardModelCache;
            this.profileRecord          = profileRecord;
            this.profileInventoryRecord = profileInventoryRecord;

            this.profileHasBoostersRecordsLookup = profileHasBoostersRecordsLookup;

            GemCount                = profileInventoryRecord.GemCount;
            GoldCount               = profileInventoryRecord.GoldCount;
            CommonWildcardCount     = profileInventoryRecord.CommonWildcardCount;
            UncommonWildcardCount   = profileInventoryRecord.UncommonWildcardCount;
            RareWildcardCount       = profileInventoryRecord.RareWildcardCount;
            MythicRareWildcardCount = profileInventoryRecord.MythicRareWildcardCount;
            VaultProgress           = profileInventoryRecord.VaultProgress;

            CardInventory           = cardInventory;
            BoosterCounts           = boosterCounts;
        }

        public static async Task<ProfileInventoryModel> LoadOrCreateAsync(DataContext dataContext, CardModelCache cardModelCache, ProfileRecord profileRecord, Func<string, Task> onLogMessage)
        {
            var profileInventoryResult = await dataContext.Mtga.ProfileInventories
                .QueryWhere(record => record.ProfileRecord == profileRecord)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (profileInventoryResult.Success)
            {
                var cardInventory = await CardInventoryModel.LoadAllAsync(dataContext, cardModelCache, profileRecord).ConfigureAwait(false);
                
                var boosterCounts = new List<ItemCount<BoosterInfoModel>>();
                var profileHasBoostersRecordsLookup = new Dictionary<int, ProfileHasBoostersRecord>();
                var profileHasBoostersRecords = await dataContext.Mtga.ProfileHasBoosters
                    .QueryWhere(record => record.ProfileRecord == profileRecord)
                    .ToListAsync()
                    .ConfigureAwait(false);
                foreach (var profileHasBoostersRecord in profileHasBoostersRecords)
                {
                    var boosterInfoModel = await BoosterInfoModel.LoadByIdAsync(dataContext, profileHasBoostersRecord.BoosterId).ConfigureAwait(false);
                    boosterCounts.Add(new ItemCount<BoosterInfoModel>(boosterInfoModel, profileHasBoostersRecord.Count));
                    profileHasBoostersRecordsLookup.Add(boosterInfoModel.MtgaBoosterId, profileHasBoostersRecord);
                }
                await onLogMessage($"Profile inventory records loaded.").ConfigureAwait(false);

                return new ProfileInventoryModel(dataContext, cardModelCache, profileRecord, profileInventoryResult.Value, profileHasBoostersRecordsLookup, cardInventory, boosterCounts.Enumerate());
            }
            else
            {
                var profileInventoryRecord = await dataContext.Mtga.ProfileInventories
                    .NewRecordAsync(profileRecord, 0, 0, 0, 0, 0, 0, 0m)
                    .ConfigureAwait(false);
                await onLogMessage($"Empty profile inventory records created.").ConfigureAwait(false);

                return new ProfileInventoryModel(dataContext, cardModelCache, profileRecord, profileInventoryRecord, new Dictionary<int, ProfileHasBoostersRecord>(), 
                    Enumerable.Empty<CardInventoryModel>(), Enumerable.Empty<ItemCount<BoosterInfoModel>>());
            }
        }

        public async Task RefreshAsync(ProfileInventoryInfoLogEvent logEvent)
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

            async Task TransactionScope()
            {
                profileInventoryRecord.GemCount = GemCount = logEvent.GemCount;
                profileInventoryRecord.GoldCount = GoldCount = logEvent.GoldCount;
                profileInventoryRecord.CommonWildcardCount = CommonWildcardCount = logEvent.CommonWildcardCount;
                profileInventoryRecord.UncommonWildcardCount = UncommonWildcardCount = logEvent.UncommonWildcardCount;
                profileInventoryRecord.RareWildcardCount = RareWildcardCount = logEvent.RareWildcardCount;
                profileInventoryRecord.MythicRareWildcardCount = MythicRareWildcardCount = logEvent.MythicRareWildcardCount;
                profileInventoryRecord.VaultProgress = VaultProgress = logEvent.VaultProgress;

                await profileInventoryRecord.UpdateRecordAsync().ConfigureAwait(false);

                var refreshedBoosterCounts = BoosterCounts.ToList();
                foreach (var boosterCount in BoosterCounts.Where(boosterCount => !logEvent.BoosterCounts.ContainsKey(boosterCount.Item.MtgaBoosterId)))
                {
                    var record = profileHasBoostersRecordsLookup[boosterCount.Item.MtgaBoosterId];
                    profileHasBoostersRecordsLookup.Remove(boosterCount.Item.MtgaBoosterId);
                    await record.DeleteRecordAsync().ConfigureAwait(false);
                    refreshedBoosterCounts.Remove(boosterCount);
                }
                foreach (var boosterCount in refreshedBoosterCounts.Where(boosterCount => logEvent.BoosterCounts.ContainsKey(boosterCount.Item.MtgaBoosterId)))
                {
                    var record = profileHasBoostersRecordsLookup[boosterCount.Item.MtgaBoosterId];
                    record.Count = boosterCount.Count = logEvent.BoosterCounts[boosterCount.Item.MtgaBoosterId];
                    await record.UpdateRecordAsync().ConfigureAwait(false);
                }
                foreach (var boosterCount in logEvent.BoosterCounts.Where(boosterCount => !BoosterCounts.Select(x => x.Item.MtgaBoosterId).Contains(boosterCount.Key)))
                {
                    var boosterInfoModel = await BoosterInfoModel.LoadByMtgaBoosterIdAsync(DataContext, boosterCount.Key).ConfigureAwait(false);
                    var profileHasBoostersRecord = await DataContext.Mtga.ProfileHasBoosters
                        .NewRecordAsync(profileRecord.Id, boosterInfoModel.Id, boosterCount.Value)
                        .ConfigureAwait(false);
                    profileHasBoostersRecordsLookup.Add(boosterCount.Key, profileHasBoostersRecord);
                    refreshedBoosterCounts.Add(new ItemCount<BoosterInfoModel>(boosterInfoModel, boosterCount.Value));
                }
                BoosterCounts = refreshedBoosterCounts.Enumerate();

                await OnLogMessageAsync("Profile inventory refreshed.").ConfigureAwait(false);
                await OnInventoryUpdated().ConfigureAwait(false);
            }
        }

        public async Task RefreshCardsAsync(ProfileInventoryCardsInfoLogEvent logEvent)
        {
            using var transaction = await DataContext.BeginTransactionAsync().ConfigureAwait(false);
            await using (transaction.ConfigureAwait(false))
                try
                {
                    await transaction.ExecuteAsync(TransactionScope).ConfigureAwait(false);
                    await transaction.CommitAsync().ConfigureAwait(false);
                }
                catch
                {
                    await transaction.RollbackAsync().ConfigureAwait(false);
                    throw;
                }

            async Task TransactionScope()
            {
                var refreshedCardInventory = CardInventory.ToList();
                foreach (var cardInventoryModel in CardInventory.Where(cardInventoryModel => !logEvent.CardCounts.ContainsKey(cardInventoryModel.CardModel.MtgaCardId)))
                {
                    refreshedCardInventory.Remove(cardInventoryModel);
                    await cardInventoryModel.UpdateCountAsync(0).ConfigureAwait(false);
                }

                foreach (var cardInventoryModel in refreshedCardInventory.Where(cardInventoryModel => logEvent.CardCounts.ContainsKey(cardInventoryModel.CardModel.MtgaCardId)))
                    await cardInventoryModel.UpdateCountAsync(logEvent.CardCounts[cardInventoryModel.CardModel.MtgaCardId]).ConfigureAwait(false);

                foreach (var cardCount in logEvent.CardCounts.Where(cardCount => !CardInventory.Select(cardInventoryModel => cardInventoryModel.CardModel.MtgaCardId).Contains(cardCount.Key)).ToList())
                {
                    var cardResult = await DataContext.Mtga.DigitalCards.QueryWhere(record => record.MtgaCardId == cardCount.Key).TryGetSingleAsync().ConfigureAwait(false);
                    if (cardResult.Success)
                    {
                        var cardInventoryModel = await CardInventoryModel.CreateNewAsync(DataContext, cardModelCache, profileRecord, cardCount.Key, cardCount.Value).ConfigureAwait(false);
                        refreshedCardInventory.Add(cardInventoryModel);
                    }
                }
                CardInventory = refreshedCardInventory.Enumerate();

                await OnLogMessageAsync("Profile card inventory refreshed.").ConfigureAwait(false);
                await OnInventoryUpdated().ConfigureAwait(false);
            }
        }

        public async Task UpdateAsync(ProfileInventoryUpdateLogEvent logEvent)
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

            async Task TransactionScope()
            {
                profileInventoryRecord.GemCount = GemCount += logEvent.GemsDelta;
                profileInventoryRecord.GoldCount = GoldCount += logEvent.GoldDelta;
                profileInventoryRecord.CommonWildcardCount = CommonWildcardCount += logEvent.CommonWildcardDelta;
                profileInventoryRecord.UncommonWildcardCount = UncommonWildcardCount += logEvent.UncommonWildcardDelta;
                profileInventoryRecord.RareWildcardCount = RareWildcardCount += logEvent.RareWildcardDelta;
                profileInventoryRecord.MythicRareWildcardCount = MythicRareWildcardCount += logEvent.MythicRareWildcardDelta;
                profileInventoryRecord.VaultProgress = VaultProgress += logEvent.VaultProgressDelta;

                await profileInventoryRecord.UpdateRecordAsync().ConfigureAwait(false);

                var refreshedCardInventory = CardInventory.ToList();
                foreach (var addedCardIdGroup in logEvent.CardIdsAdded.GroupBy(x => x))
                {
                    var cardInventoryModel = refreshedCardInventory.SingleOrDefault(cardInventory => cardInventory.CardModel.MtgaCardId == addedCardIdGroup.Key);
                    if (cardInventoryModel != null)
                        await cardInventoryModel.UpdateCountAsync(cardInventoryModel.Count + addedCardIdGroup.Count()).ConfigureAwait(false);
                    else
                    {
                        cardInventoryModel = await CardInventoryModel.CreateNewAsync(DataContext, cardModelCache, profileRecord, addedCardIdGroup.Key, addedCardIdGroup.Count()).ConfigureAwait(false);
                        refreshedCardInventory.Add(cardInventoryModel);
                    }
                }

                var refreshedBoosterCounts = BoosterCounts.ToList();
                var requiresBoosterCountRefresh = false;
                foreach (var boosterDelta in logEvent.BoostersDelta)
                {
                    var boosterCount = refreshedBoosterCounts.SingleOrDefault(boosterCount => boosterCount.Item.MtgaBoosterId == boosterDelta.Key);
                    if (boosterCount != null)
                    {
                        var record = profileHasBoostersRecordsLookup[boosterDelta.Key];
                        record.Count = boosterCount.Count += boosterDelta.Value;
                        requiresBoosterCountRefresh |= record.IsModified;
                        if (boosterCount.Count == 0)
                        {
                            profileHasBoostersRecordsLookup.Remove(boosterDelta.Key);
                            await record.DeleteRecordAsync().ConfigureAwait(false);
                            refreshedBoosterCounts.Remove(boosterCount);
                        }
                        else
                            await record.UpdateRecordAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        var boosterInfoModel = await BoosterInfoModel.LoadByMtgaBoosterIdAsync(DataContext, boosterDelta.Key).ConfigureAwait(false);
                        var profileHasBoostersRecord = await DataContext.Mtga.ProfileHasBoosters.NewRecordAsync(profileRecord.Id, boosterInfoModel.Id, boosterDelta.Value).ConfigureAwait(false);
                        profileHasBoostersRecordsLookup.Add(boosterDelta.Key, profileHasBoostersRecord);
                        refreshedBoosterCounts.Add(new ItemCount<BoosterInfoModel>(boosterInfoModel, boosterDelta.Value));
                        requiresBoosterCountRefresh |= true;
                    }
                }
                CardInventory = refreshedCardInventory.Enumerate();
                BoosterCounts = refreshedBoosterCounts.Enumerate();
                await OnInventoryUpdated().ConfigureAwait(false);
            }
        }

        public async Task AddDraftPick(int mtgaCardId)
        {
            var existingCardInventoryModel = CardInventory.FirstOrDefault(cardInventory => cardInventory.CardModel.MtgaCardId == mtgaCardId);
            if (existingCardInventoryModel != null)
                await existingCardInventoryModel.UpdateCountAsync(existingCardInventoryModel.Count + 1).ConfigureAwait(false);
            else
            {
                var refreshedCardInventory = CardInventory.ToList();
                var cardInventoryModel = await CardInventoryModel.CreateNewAsync(DataContext, cardModelCache, profileRecord, mtgaCardId, 1).ConfigureAwait(false);
                refreshedCardInventory.Add(cardInventoryModel);
                CardInventory = refreshedCardInventory;
            }
        }

        private Task OnInventoryUpdated()
            => InventoryUpdatedAsync.InvokeAsync(this, EventArgs.Empty);
    }
}
