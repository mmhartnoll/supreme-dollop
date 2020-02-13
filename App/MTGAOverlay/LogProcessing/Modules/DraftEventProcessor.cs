using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.GUI;
using MindSculptor.App.MtgaOverlay.LogProcessing.LogEvents;
using MindSculptor.Tools.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.LogProcessing.Modules
{
    internal class DraftEventProcessor : ProcessingModule
    {
        private DraftEventProcessor() { }

        public static async Task JoinDraftEventAsync(JoinDraftEventLogEvent logEvent)
            => await ProcessDataWithTransactionAsync(dataContext => JoinDraftEventAsync(dataContext, logEvent))
                .ConfigureAwait(false);

        public static async Task RefreshDraftEventAsync(UpdateDraftEventLogEvent logEvent)
            => await ProcessDataWithTransactionAsync(dataContext => RefreshDraftEventAsync(dataContext, logEvent))
                .ConfigureAwait(false);

        public static async Task StartDraftEventMatchAsync(CreateDraftMatchLogEvent logEvent)
            => await ProcessDataWithTransactionAsync(dataContext => StartDraftEventMatchAsync(dataContext, logEvent))
                .ConfigureAwait(false);

        public static async Task RecordDraftGameAsync(DraftGameEndLogEvent logEvent)
            => await ProcessDataWithTransactionAsync(dataContext => RecordDraftGameAsync(dataContext, logEvent))
                .ConfigureAwait(false);

        public static async Task RecordDraftMatchResults(DraftMatchEndLogEvent logEvent)
            => await ProcessDataWithTransactionAsync(dataContext => RecordDraftMatchResults(dataContext, logEvent))
                .ConfigureAwait(false);

        private static async Task JoinDraftEventAsync(DataContext dataContext, JoinDraftEventLogEvent logEvent)
        {
            var profileRecord = await GetProfileRecordAsync(dataContext)
                .ConfigureAwait(false);
            var draftEventRecord = await UpsertDraftEventRecordAsync(dataContext, logEvent.DraftEventType, logEvent.SetCode)
                .ConfigureAwait(false);
            var draftEventEntryRecord = await UpsertDraftEventEntryRecordAsync(dataContext, draftEventRecord, profileRecord, logEvent.DraftEventEntryId)
                .ConfigureAwait(false);
            await SetActiveDraftEventEntry(dataContext, draftEventRecord, draftEventEntryRecord, profileRecord)
                .ConfigureAwait(false);
        }

        private static async Task RefreshDraftEventAsync(DataContext dataContext, UpdateDraftEventLogEvent logEvent)
        {
            var profileRecord = await GetProfileRecordAsync(dataContext)
                .ConfigureAwait(false);
            var draftEventRecord = await UpsertDraftEventRecordAsync(dataContext, logEvent.DraftEventType, logEvent.SetCode)
                .ConfigureAwait(false);

            if (logEvent.HasActiveDraftEventEntry)
            {
                var draftEventEntryRecord = await UpsertDraftEventEntryRecordAsync(dataContext, draftEventRecord, profileRecord, logEvent.ActiveDraftEventEntryId)
                    .ConfigureAwait(false);
                await SetActiveDraftEventEntry(dataContext, draftEventRecord, draftEventEntryRecord, profileRecord)
                    .ConfigureAwait(false);
            }
        }

        private static async Task StartDraftEventMatchAsync(DataContext dataContext, CreateDraftMatchLogEvent logEvent)
        {
            var profileRecord = await GetProfileRecordAsync(dataContext)
                .ConfigureAwait(false);

            var draftEventMatchResult = await dataContext.Mtga.DraftEventMatches
                .QueryWhere(record => record.Id == logEvent.MatchId)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (draftEventMatchResult.Success)
                return;

            var setRecord = await dataContext.Cards.Sets
                .QueryWhere(record => record.Code == logEvent.SetCode)
                .SingleAsync()
                .ConfigureAwait(false);
            var draftEventRecord = await dataContext.Mtga.DraftEvents
                .QueryWhere(record => 
                    record.SetRecord == setRecord &&
                    record.DraftType == logEvent.DraftEventType.ToString())
                .SingleAsync()
                .ConfigureAwait(false);
            var activeDraftEventEntryRecord = await dataContext.Mtga.ActiveDraftEventEntries
                .QueryWhere(record => 
                    record.DraftEventRecord == draftEventRecord &&
                    record.ProfileRecord == profileRecord)
                .SingleAsync()
                .ConfigureAwait(false);
            var draftEventEntryRecord = await dataContext.Mtga.DraftEventEntries
                .QueryWhere(record => record.Id == activeDraftEventEntryRecord.DraftEventEntryId)
                .SingleAsync()
                .ConfigureAwait(false);

            OpponentRecord opponentRecord;
            var opponentResult = await dataContext.Mtga.Opponents
                .QueryWhere(record =>
                    record.ScreenName == logEvent.OpponentScreenName &&
                    record.UserId == logEvent.OpponentUserId)
                .TryGetFirstAsync()
                .ConfigureAwait(false);
            if (opponentResult.Success)
                opponentRecord = opponentResult.Value;
            else
                opponentRecord = await dataContext.Mtga.Opponents
                    .NewRecordAsync(logEvent.OpponentScreenName, logEvent.OpponentUserId)
                    .ConfigureAwait(false);

            await dataContext.Mtga.DraftEventMatches
                .NewRecordAsync(draftEventEntryRecord, opponentRecord, logEvent.MatchId)
                .ConfigureAwait(false);
        }

        private static async Task RecordDraftGameAsync(DataContext dataContext, DraftGameEndLogEvent logEvent)
        {
            var draftEventMatchResult = await dataContext.Mtga.DraftEventMatches
                .QueryWhere(record => record.Id == logEvent.MatchId)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (!draftEventMatchResult.Success)
                return;

            var draftEventMatchRecord = draftEventMatchResult.Value;
            var draftEventGameResult = await dataContext.Mtga.DraftEventGames
                .QueryWhere(record =>
                    record.DraftEventMatchRecord == draftEventMatchRecord &&
                    record.GameNumber == logEvent.GameNumber)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (draftEventGameResult.Success)
                return;

            await dataContext.Mtga.DraftEventGames
                .NewRecordAsync(draftEventMatchRecord, logEvent.GameNumber, logEvent.PlayedFirst, logEvent.GameWon)
                .ConfigureAwait(false);
        }

        private static async Task RecordDraftMatchResults(DataContext dataContext, DraftMatchEndLogEvent logEvent)
        {
            var draftEventMatchResult = await dataContext.Mtga.DraftEventMatches
                .QueryWhere(record => record.Id == logEvent.MatchId)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (!draftEventMatchResult.Success)
                return;

            var draftEventMatchRecord = draftEventMatchResult.Value;
            var draftEventMatchResultResult = await dataContext.Mtga.DraftEventMatchResults
                .QueryWhere(record => record.DraftEventMatchRecord == draftEventMatchRecord)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (draftEventMatchResultResult.Success)
                return;

            var draftEventGameRecords = await dataContext.Mtga.DraftEventGames
                .QueryWhere(record => record.DraftEventMatchRecord == draftEventMatchRecord)
                .ToListAsync()
                .ConfigureAwait(false);
            if (!draftEventGameRecords.Any())
                return;

            var draftEventGameLookup = draftEventGameRecords
                .ToDictionary(record => record.GameNumber);

            var testRecord = draftEventGameRecords.First();
            var winningPlayerId = logEvent.WinningPlayerNumberForEachGame[testRecord.GameNumber - 1];
            int playerId = winningPlayerId == 1 ? (testRecord.GameWon ? 1 : 2) : (testRecord.GameWon ? 2 : 1);

            bool matchWon = playerId == logEvent.WinningPlayerNumber;

            await dataContext.Mtga.DraftEventMatchResults
                .NewRecordAsync(draftEventMatchRecord, matchWon)
                .ConfigureAwait(false);
        }

        private static async Task<DraftEventRecord> UpsertDraftEventRecordAsync(DataContext dataContext, DraftEventType draftEventType, string setCode)
        {
            var profileRecord = await GetProfileRecordAsync(dataContext)
                .ConfigureAwait(false);

            var setRecord = await dataContext.Cards.Sets
                .QueryWhere(record => record.Code == setCode)
                .SingleAsync()
                .ConfigureAwait(false);

            var draftEventResult = await dataContext.Mtga.DraftEvents
                .QueryWhere(record =>
                    record.SetRecord == setRecord &&
                    record.DraftType == draftEventType.ToString())
                .TryGetSingleAsync()
                .ConfigureAwait(false);

            if (draftEventResult.Success)
                return draftEventResult.Value;

            return await dataContext.Mtga.DraftEvents
                .NewRecordAsync(setRecord, draftEventType.ToString())
                .ConfigureAwait(false);
        }

        private static async Task<DraftEventEntryRecord> UpsertDraftEventEntryRecordAsync(DataContext dataContext, DraftEventRecord draftEventRecord, ProfileRecord profileRecord, Guid id)
        {
            var draftEventEntryResult = await dataContext.Mtga.DraftEventEntries
                .QueryWhere(record => record.Id == id)
                .TryGetSingleAsync()
                .ConfigureAwait(false);

            if (draftEventEntryResult.Success)
                return draftEventEntryResult.Value;

            return await dataContext.Mtga.DraftEventEntries
                .NewRecordAsync(draftEventRecord, profileRecord, id)
                .ConfigureAwait(false);
        }

        private static async Task SetActiveDraftEventEntry(DataContext dataContext, DraftEventRecord draftEventRecord, DraftEventEntryRecord draftEventEntryRecord, ProfileRecord profileRecord)
        {
            var activeDraftEventEntryResult = await dataContext.Mtga.ActiveDraftEventEntries
                .QueryWhere(record =>
                    record.DraftEventRecord == draftEventRecord &&
                    record.ProfileRecord == profileRecord)
                .TryGetSingleAsync()
                .ConfigureAwait(false);

            if (activeDraftEventEntryResult.Success)
            {
                var activeDraftEventEntryRecord = activeDraftEventEntryResult.Value;
                if (activeDraftEventEntryRecord.DraftEventEntryId == draftEventEntryRecord.Id)
                    return;
                await activeDraftEventEntryRecord.DeleteRecordAsync()
                    .ConfigureAwait(false);
            }

            await dataContext.Mtga.ActiveDraftEventEntries
                .NewRecordAsync(draftEventEntryRecord)
                .ConfigureAwait(false);
        }
    }
}
