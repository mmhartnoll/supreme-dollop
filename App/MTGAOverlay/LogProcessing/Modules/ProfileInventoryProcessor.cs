using MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents;
using MindSculptor.Tools.Extensions;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.Modules
{
    internal class ProfileInventoryProcessor : ProcessingModule
    {
        private ProfileInventoryProcessor() { }

        public static async Task RefreshProfileCardsAsync(UpdateProfileCardsLogEvent logEvent)
            => await ProcessDataWithTransactionAsync(dataContext => RefreshProfileCardsAsync(dataContext, logEvent))
                .ConfigureAwait(false);

        public static async Task RefreshProfileInventoryAsync(UpdateProfileInventoryLogEvent logEvent)
            => await ProcessDataWithTransactionAsync(dataContext => RefreshProfileInventoryAsync(dataContext, logEvent))
                .ConfigureAwait(false);

        private static async Task RefreshProfileCardsAsync(DataContext dataContext, UpdateProfileCardsLogEvent logEvent)
        {
            var profileRecord = await GetProfileRecordAsync(dataContext)
                .ConfigureAwait(false);

            var cardRecords = await dataContext.Mtga.Cards
                .ToListAsync()
                .ConfigureAwait(false);
            var cardLookup = cardRecords
                .ToDictionary(record => record.MtgaCardId);

            var profileHasCardRecords = await dataContext.Mtga.ProfileHasCards
                .QueryWhere(record => record.ProfileRecord == profileRecord)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var record in profileHasCardRecords)
                await record.DeleteRecordAsync()
                    .ConfigureAwait(false);

            foreach (var keyValuePair in logEvent.CardCounts)
                if (cardLookup.TryGetValue(keyValuePair.Key, out var cardRecord))
                    await dataContext.Mtga.ProfileHasCards
                        .NewRecordAsync(profileRecord, cardRecord, keyValuePair.Value)
                        .ConfigureAwait(false);
        }

        private static async Task RefreshProfileInventoryAsync(DataContext dataContext, UpdateProfileInventoryLogEvent logEvent)
        {
            var profileRecord = await GetProfileRecordAsync(dataContext)
                .ConfigureAwait(false);

            var profileInventoryResult = await dataContext.Mtga.ProfileInventories
                .QueryWhere(record => record.ProfileRecord == profileRecord)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (profileInventoryResult.Success)
            {
                var profileInventoryRecord = profileInventoryResult.Value;

                profileInventoryRecord.MythicRareWildcardCount  = logEvent.MythicRareWildcardCount;
                profileInventoryRecord.RareWildcardCount        = logEvent.RareWildcardCount;
                profileInventoryRecord.UncommonWildcardCount    = logEvent.UncommonWildcardCount;
                profileInventoryRecord.CommonWildcardCount      = logEvent.CommonWildcardCount;

                profileInventoryRecord.GoldCount                = logEvent.GoldCount;
                profileInventoryRecord.GemCount                 = logEvent.GemCount;
            }
            else
                await dataContext.Mtga.ProfileInventories
                    .NewRecordAsync(profileRecord, logEvent.MythicRareWildcardCount, logEvent.RareWildcardCount, logEvent.UncommonWildcardCount, logEvent.CommonWildcardCount, logEvent.GoldCount, logEvent.GemCount)
                    .ConfigureAwait(false);

            foreach (var keyValuePair in logEvent.BoosterCounts)
            {
                var boosterRecord = await dataContext.Mtga.Boosters
                    .QueryWhere(record => record.MtgaBoosterId == keyValuePair.Key)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var profileHasBoostersResult = await dataContext.Mtga.ProfileHasBoosters
                    .QueryWhere(record =>
                        record.ProfileRecord == profileRecord &&
                        record.BoosterRecord == boosterRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                if (profileHasBoostersResult.Success)
                {
                    var profileHasBoostersRecord = profileHasBoostersResult.Value;
                    profileHasBoostersRecord.Count = keyValuePair.Value;
                    await profileHasBoostersRecord.UpdateRecordAsync()
                        .ConfigureAwait(false);
                }
                else
                    await dataContext.Mtga.ProfileHasBoosters
                        .NewRecordAsync(profileRecord, boosterRecord, keyValuePair.Value)
                        .ConfigureAwait(false);
            }
        }
    }
}
