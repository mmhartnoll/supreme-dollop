using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.Modules
{
    internal class DraftPickProcessor : ProcessingModule
    {
        private static DraftPickOptionsLogEvent? savedOptionsLogEvent = null;

        private DraftPickProcessor() { }


        public static async IAsyncEnumerable<DraftPickOption> StreamDraftPickOptionsAndSaveState(DraftPickOptionsLogEvent logEvent) 
        {
            var stream = StreamDataAsync(dataContext => StreamDraftPickOptionsAndSaveState(dataContext, logEvent));
            await foreach (var draftPickOption in stream.ConfigureAwait(false))
                yield return draftPickOption;
        }

        public static async Task SaveDraftPickSelection(DraftPickSelectionLogEvent logEvent)
            => await ProcessDataWithTransactionAsync(dataContext => SaveDraftPickSelection(dataContext, logEvent))
                .ConfigureAwait(false);

        private static async IAsyncEnumerable<DraftPickOption> StreamDraftPickOptionsAndSaveState(DataContext dataContext, DraftPickOptionsLogEvent logEvent)
        {
            var profileRecord = await GetProfileRecordAsync(dataContext)
                .ConfigureAwait(false);

            var existingDraftPicksStream = StreamDataAsync(dataContext => 
                StreamExistingDraftPickRecordsAsync(dataContext, profileRecord, logEvent.DraftEventType, logEvent.SetCode, logEvent.PackNumber, logEvent.PickNumber));

            if (await existingDraftPicksStream.AnyAsync().ConfigureAwait(false))
                yield break;

            savedOptionsLogEvent = logEvent;

            var draftPickOptionsStream = StreamDataAsync(dataContext => StreamDraftPickOptions(dataContext, profileRecord, logEvent.DraftPackCardIds));
            await foreach (var draftPickOption in draftPickOptionsStream.ConfigureAwait(false))
                yield return draftPickOption;
        }

        private static async Task SaveDraftPickSelection(DataContext dataContext, DraftPickSelectionLogEvent logEvent)
        {
            if (savedOptionsLogEvent == null)
                return;
            if (!savedOptionsLogEvent.MatchesSelectionLogEvent(logEvent))
                throw new Exception($"{nameof(DraftPickProcessor)}: Saved values in {nameof(DraftPickOptionsLogEvent)} do not match those of {nameof(DraftPickSelectionLogEvent)}.");

            var profileRecord = await GetProfileRecordAsync(dataContext)
                .ConfigureAwait(false);
            var draftEventEntryRecord = await GetDraftEventEntryRecord(dataContext, profileRecord, savedOptionsLogEvent.DraftEventType, savedOptionsLogEvent.SetCode)
                .ConfigureAwait(false);

            await InsertDraftPickRecordsAsync(dataContext, profileRecord, draftEventEntryRecord, logEvent.PackNumber, logEvent.PickNumber, savedOptionsLogEvent.DraftPackCardIds, logEvent.SelectedCardId)
                .ConfigureAwait(false);

            savedOptionsLogEvent = null;
        }

        private static async IAsyncEnumerable<DraftPickRecord> StreamExistingDraftPickRecordsAsync(DataContext dataContext, ProfileRecord profileRecord, DraftEventType draftEventType, string setCode, int packNumber, int pickNumber)
        {
            var draftEventEntryRecord = await GetDraftEventEntryRecord(dataContext, profileRecord, draftEventType, setCode)
                .ConfigureAwait(false);

            var draftPickRecords = dataContext.Mtga.DraftPicks
                .QueryWhere(record =>
                    record.DraftEventEntryRecord == draftEventEntryRecord &&
                    record.PackNumber == packNumber &&
                    record.PickNumber == pickNumber);
            await foreach (var record in draftPickRecords.ConfigureAwait(false))
                yield return record;
        }

        private static async Task<DraftEventEntryRecord> GetDraftEventEntryRecord(DataContext dataContext, ProfileRecord profileRecord, DraftEventType draftEventType, string setCode)
        {
            var setRecord = await dataContext.Cards.Sets
                .QueryWhere(record => record.Code == setCode)
                .SingleAsync()
                .ConfigureAwait(false);

            var draftEventRecord = await dataContext.Mtga.DraftEvents
                .QueryWhere(record =>
                    record.SetRecord == setRecord &&
                    record.DraftType == draftEventType.ToString())
                .SingleAsync()
                .ConfigureAwait(false);

            var activeDraftEventEntryRecord = await dataContext.Mtga.ActiveDraftEventEntries
                .QueryWhere(record =>
                    record.DraftEventRecord == draftEventRecord &&
                    record.ProfileRecord == profileRecord)
                .SingleAsync()
                .ConfigureAwait(false);

            return await dataContext.Mtga.DraftEventEntries
                .QueryWhere(record => record.Id == activeDraftEventEntryRecord.DraftEventEntryId)
                .SingleAsync()
                .ConfigureAwait(false);
        }

        private static async IAsyncEnumerable<DraftPickOption> StreamDraftPickOptions(DataContext dataContext, ProfileRecord profileRecord, IEnumerable<int> draftPackCardIds)
        {
            var list = new List<(DraftPickOption Option, int LogicalOrdinal)>();
            foreach (var cardId in draftPackCardIds)
            {
                var cardRecord = await dataContext.Mtga.Cards
                    .QueryWhere(record => record.MtgaCardId == cardId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var basePrintingRecord = await dataContext.Cards.BasePrintings
                    .QueryWhere(record => record.Id == cardRecord.Id)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var setInclusionRecord = await dataContext.Cards.SetInclusions
                    .QueryWhere(record => record.Id == basePrintingRecord.SetInclusionId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var rarityRecord = await dataContext.Cards.RarityTypes
                    .QueryWhere(record => record.Id == setInclusionRecord.RarityTypeId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var baseRecord = await dataContext.Cards.Bases
                    .QueryWhere(record => record.Id == setInclusionRecord.BaseId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var faceRecord = await dataContext.Cards.Faces
                    .QueryWhere(record => 
                        record.BaseRecord == baseRecord &&
                        record.IsPrimaryFace)
                    .SingleAsync()
                    .ConfigureAwait(false);

                var profileHasCardResult = await dataContext.Mtga.ProfileHasCards
                    .QueryWhere(record =>
                        record.ProfileRecord == profileRecord &&
                        record.CardRecord == cardRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);

                var rarity = (Rarity)Enum.Parse(typeof(Rarity), rarityRecord.Value.Replace(" ", null));
                var inventoryCount = profileHasCardResult.Success ?
                    profileHasCardResult.Value.Count : 0;

                list.Add((DraftPickOption.Create(faceRecord.Name, rarity, inventoryCount), setInclusionRecord.LogicalOrdinal));
            }

            foreach (var item in list.OrderBy(item => item.Option.Rarity).ThenBy(item => item.LogicalOrdinal))
                yield return item.Option;
        }

        private static async Task InsertDraftPickRecordsAsync(DataContext dataContext, ProfileRecord profileRecord, DraftEventEntryRecord draftEventEntryRecord, int packNumber, int pickNumber, IEnumerable<int> draftPackCardIds, int selectedCardId)
        {
            var list = new List<(CardRecord CardRecord, Rarity Rarity, int LogicalOrdinal)>();
            foreach (var cardId in draftPackCardIds)
            {
                var cardRecord = await dataContext.Mtga.Cards
                    .QueryWhere(record => record.MtgaCardId == cardId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var basePrintingRecord = await dataContext.Cards.BasePrintings
                    .QueryWhere(record => record.Id == cardRecord.Id)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var setInclusionRecord = await dataContext.Cards.SetInclusions
                    .QueryWhere(record => record.Id == basePrintingRecord.SetInclusionId)
                    .SingleAsync()
                    .ConfigureAwait(false);
                var rarityRecord = await dataContext.Cards.RarityTypes
                    .QueryWhere(record => record.Id == setInclusionRecord.RarityTypeId)
                    .SingleAsync()
                    .ConfigureAwait(false);

                var rarity = (Rarity)Enum.Parse(typeof(Rarity), rarityRecord.Value.Replace(" ", null));

                list.Add((cardRecord, rarity, setInclusionRecord.LogicalOrdinal));
            }

            var ordinal = 0;
            var orderedList = list.OrderBy(item => item.Rarity).ThenBy(item => item.LogicalOrdinal);
            foreach (var item in orderedList)
            {
                var isSelectedCard = item.CardRecord.MtgaCardId == selectedCardId;

                await dataContext.Mtga.DraftPicks
                    .NewRecordAsync(draftEventEntryRecord, item.CardRecord, packNumber, pickNumber, ordinal++, isSelectedCard)
                    .ConfigureAwait(false);

                if (isSelectedCard)
                {
                    var profileHasCardResult = await dataContext.Mtga.ProfileHasCards
                    .QueryWhere(record =>
                        record.ProfileRecord == profileRecord &&
                        record.CardRecord == item.CardRecord)
                    .TryGetSingleAsync()
                    .ConfigureAwait(false);
                    if (profileHasCardResult.Success)
                    {
                        var profileHasCardRecord = profileHasCardResult.Value;
                        profileHasCardRecord.Count = Math.Min(4, profileHasCardRecord.Count + 1);
                        await profileHasCardRecord.UpdateRecordAsync()
                            .ConfigureAwait(false);
                    }
                    else
                        await dataContext.Mtga.ProfileHasCards
                            .NewRecordAsync(profileRecord, item.CardRecord, 1);
                }
            }
        }
    }
}
