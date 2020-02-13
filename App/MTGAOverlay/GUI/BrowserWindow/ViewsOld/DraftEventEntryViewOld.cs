using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.GUI.BrowserWindow.Views
{
    internal class DraftEventEntryViewOld : DataView
    {
        public Guid Id { get; }
        public bool IsCompleted { get; }
        public int MatchWinCount { get; }
        public int MatchLossCount { get; }

        private DraftEventEntryViewOld(Guid id, bool isCompleted, int matchWinCount, int matchLossCount)
        {
            Id = id;
            IsCompleted = isCompleted;
            MatchWinCount = matchWinCount;
            MatchLossCount = matchLossCount;
        }

        public static async IAsyncEnumerable<DraftEventEntryViewOld> GetDraftEventEntries(DraftEventViewOld eventView)
        {
            var stream = StreamDataAsync(dataContext => GetDraftEventEntries(dataContext, eventView));
            await foreach (var draftEventEntryView in stream.ConfigureAwait(false))
                yield return draftEventEntryView;
        }

        private static async IAsyncEnumerable<DraftEventEntryViewOld> GetDraftEventEntries(DataContext dataContext, DraftEventViewOld eventView)
        {
            var profileRecord = await GetProfileRecordAsync(dataContext)
                .ConfigureAwait(false);
            var draftEventRecord = await dataContext.Mtga.DraftEvents
                .QueryWhere(record => record.Id == eventView.Id)
                .SingleAsync()
                .ConfigureAwait(false);

            var draftEventEntryRecords = await dataContext.Mtga.DraftEventEntries
                .QueryWhere(record =>
                    record.ProfileRecord == profileRecord &&
                    record.DraftEventRecord == draftEventRecord)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var draftEventEntryRecord in draftEventEntryRecords)
            {
                var draftEventMatchRecords = await dataContext.Mtga.DraftEventMatches
                    .QueryWhere(record => record.DraftEventEntryRecord == draftEventEntryRecord)
                    .ToListAsync()
                    .ConfigureAwait(false);
                var draftEventMatchRecordIds = draftEventMatchRecords
                    .Select(record => record.Id);

                var draftEventMatchResultRecords = await dataContext.Mtga.DraftEventMatchResults
                    .QueryWhere(record => record.DraftEventMatchId.In(draftEventMatchRecordIds))
                    .ToListAsync()
                    .ConfigureAwait(false);

                var matchWinCount = draftEventMatchResultRecords.Count(record => record.MatchWon);
                var matchLossCount = draftEventMatchResultRecords.Count(record => !record.MatchWon);

                yield return new DraftEventEntryViewOld(draftEventEntryRecord.Id, false, matchWinCount, matchLossCount);
            }
        }
    }
}
