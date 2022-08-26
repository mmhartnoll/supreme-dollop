using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.EventLogEvents;
using MindSculptor.Tools;
using MindSculptor.Tools.Exceptions;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class EventModel : Model
    {
        private readonly AsyncCachedLookup<Guid, EventEntryModel> eventEntryLookup;

        private NullableReference<EventEntryModel> activeEventEntryModel;

        protected ProfileRecord ProfileRecord { get; }
        protected EventRecord EventRecord { get; }

        public Guid Id => EventRecord.Id;
        public string MtgaEventId => EventRecord.MtgaEventId;

        public bool HasActiveEventEntry => activeEventEntryModel.HasValue;
        public EventEntryModel ActiveEventEntryModel => activeEventEntryModel.HasValue ? activeEventEntryModel.Value: throw new PropertyUndefinedException(nameof(ActiveEventEntryModel), nameof(HasActiveEventEntry));

        protected EventModel(DataContext dataContext, ProfileRecord profileRecord, EventRecord eventRecord) : base(dataContext)
        {
            ProfileRecord = profileRecord;
            EventRecord = eventRecord;

            eventEntryLookup = new AsyncCachedLookup<Guid, EventEntryModel>(LoadEventEntry);
        }

        public static async Task<EventModel> LoadOrCreateAsync(DataContext dataContext, ProfileRecord profileRecord, string mtgaEventId)
        {
            EventRecord eventRecord;
            var eventResult = await dataContext.Mtga.Events
                .QueryWhere(record => record.MtgaEventId == mtgaEventId)
                .TryGetSingleAsync()
                .ConfigureAwait(false);
            if (eventResult.Success)
            {
                eventRecord = eventResult.Value;
                var draftEventModel = await DraftEventModel.LoadOrDefaultAsync(dataContext, profileRecord, eventRecord);
                if (draftEventModel.HasValue)
                    return draftEventModel.Value;
                return new EventModel(dataContext, profileRecord, eventRecord);
            }
            else
                eventRecord = await dataContext.Mtga.Events.NewRecordAsync(mtgaEventId).ConfigureAwait(false);

            var regexMatch = Regex.Match(mtgaEventId, @"^(CompDraft|QuickDraft)_([A-Z]{3})*$");
            if (regexMatch.Success)
            {
                var rawEventType = regexMatch.Groups[1].Value;
                var setCode      = regexMatch.Groups[2].Value;
                var draftEventType = rawEventType switch
                {
                    "CompDraft"  => DraftEventType.Traditional,
                    "QuickDraft" => DraftEventType.Ranked,
                    _            => throw new Exception()
                };
                return await DraftEventModel.CreateAsync(dataContext, profileRecord, eventRecord, draftEventType, setCode).ConfigureAwait(false);
            }
            return new EventModel(dataContext, profileRecord, eventRecord);
        }

        public async Task SetActiveEventEntryAsync(Guid? id) 
        {
            if (id.HasValue)
                activeEventEntryModel = await eventEntryLookup.GetValueAsync(id.Value).ConfigureAwait(false);
            else
                activeEventEntryModel = null;
        }

        public async IAsyncEnumerable<EventEntryModel> GetEventEntriesAsync()
        {
            var eventEntryRecordIds = await DataContext.Mtga.EventEntries
                .QueryWhere(record =>
                    record.EventRecord == EventRecord &&
                    record.ProfileRecord == ProfileRecord)
                .SelectAsync(record => record.Id)
                .ToListAsync()
                .ConfigureAwait(false);
            foreach (var eventEntryRecordId in eventEntryRecordIds)
                yield return await eventEntryLookup.GetValueAsync(eventEntryRecordId).ConfigureAwait(false);
        }

        private async Task<EventEntryModel> LoadEventEntry(Guid id)
        {
            var eventEntryModel = await EventEntryModel.LoadOrCreateAsync(DataContext, ProfileRecord, EventRecord, id).ConfigureAwait(false);
            eventEntryModel.LogMessageAsync      += OnLogMessageAsync;
            eventEntryModel.LogErrorMessageAsync += OnLogErrorMessageAsync;
            return eventEntryModel;
        }
    }
}
