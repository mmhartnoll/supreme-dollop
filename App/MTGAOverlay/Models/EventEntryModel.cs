using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.Tools;
using MindSculptor.Tools.Exceptions;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class EventEntryModel : Model
    {
        private readonly AsyncCachedLookup<Guid, EventMatchModel> eventMatchLookup;

        private readonly EventEntryRecord eventEntryRecord;

        public Guid Id => eventEntryRecord.Id;

        private EventEntryModel(DataContext dataContext, EventEntryRecord eventEntryRecord) 
            : base(dataContext)
        {
            this.eventEntryRecord = eventEntryRecord;

            eventMatchLookup = new AsyncCachedLookup<Guid, EventMatchModel>(TryLoadMatchModel);
        }

        public static async Task<EventEntryModel> LoadOrCreateAsync(DataContext dataContext, ProfileRecord profileRecord, EventRecord eventRecord, Guid id)
        {
            EventEntryRecord eventEntryRecord;
            var eventEntryResult = await dataContext.Mtga.EventEntries
                .QueryWhere(record => record.Id == id)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (eventEntryResult.Success)
                eventEntryRecord = eventEntryResult.Value;
            else
                eventEntryRecord = await dataContext.Mtga.EventEntries.NewRecordAsync(eventRecord, profileRecord, id).ConfigureAwait(false);
            return new EventEntryModel(dataContext, eventEntryRecord);
        }

        public async Task<EventMatchModel> LoadOrCreateMatchAsync(Guid id, Player opponent)
        {
            var matchModelResult = await eventMatchLookup.TryGetValueAsync(id).ConfigureAwait(false);
            if (matchModelResult.Success)
                return matchModelResult.Value;
            else
            {
                var matchModel = await EventMatchModel.CreateAsync(DataContext, eventEntryRecord, id, opponent).ConfigureAwait(false);
                matchModel.LogMessageAsync      += OnLogMessageAsync;
                matchModel.LogErrorMessageAsync += OnLogErrorMessageAsync;

                await eventMatchLookup.AddValueAsync(id, matchModel).ConfigureAwait(false);
                return matchModel;
            }
        }

        public async IAsyncEnumerable<EventMatchModel> GetMatchesAsync()
        {
            var eventMatchIds = await DataContext.Mtga.EventMatches
                .QueryWhere(record => record.EventEntryRecord == eventEntryRecord)
                .SelectAsync(record => record.Id)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var eventMatchId in eventMatchIds)
                yield return await eventMatchLookup.GetValueAsync(eventMatchId).ConfigureAwait(false);
        }

        private Task<VerifiedResult<EventMatchModel>> TryLoadMatchModel(Guid id)
            => EventMatchModel.TryLoadAsync(DataContext, id);
    }
}
