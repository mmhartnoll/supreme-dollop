using MindSculptor.App.AppDataContext.Schemas.Mtga.Tables.Records;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.EventLogEvents;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class EventsModel : Model
    {
        private readonly ProfileRecord profileRecord;

        public IReadOnlyDictionary<string, EventModel> AvailableEventModels { get; private set; } = new ReadOnlyDictionary<string, EventModel>(new Dictionary<string, EventModel>());

        private EventsModel(DataContext dataContext, ProfileRecord profileRecord) : base(dataContext)
            => this.profileRecord = profileRecord;

        public static async Task<EventsModel> LoadAsync(DataContext dataContext, ProfileModel profileModel)
        {
            var profileRecord = await dataContext.Mtga.Profiles
                .QueryWhere(record => record.Id == profileModel.Id)
                .SingleAsync()
                .ConfigureAwait(false);
            return new EventsModel(dataContext, profileRecord);
        }

        public async Task RefreshAvailableEventsAsync(EventInfoLogEvent logEvent)
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
                var refreshedEventModels = AvailableEventModels.ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Value);

                foreach (var eventModelKey in AvailableEventModels.Keys.Where(key => !logEvent.EventInfo.Select(eventInfo => eventInfo.MtgaEventId).Contains(key)))
                    refreshedEventModels.Remove(eventModelKey);
                foreach (var eventInfo in logEvent.EventInfo.Where(info => !AvailableEventModels.Keys.Contains(info.MtgaEventId)).ToList())
                {
                    var eventModel = await EventModel.LoadOrCreateAsync(DataContext, profileRecord, eventInfo.MtgaEventId).ConfigureAwait(false);
                    eventModel.LogMessageAsync += OnLogMessageAsync;
                    eventModel.LogErrorMessageAsync += OnLogErrorMessageAsync;
                    refreshedEventModels.Add(eventInfo.MtgaEventId, eventModel);
                }
                foreach (var eventInfo in logEvent.EventInfo)
                {
                    var activeDraftEventEntryId = eventInfo.HasActiveDraftEventEntry ? eventInfo.ActiveDraftEventEntryId : (Guid?)null;
                    await refreshedEventModels[eventInfo.MtgaEventId].SetActiveEventEntryAsync(activeDraftEventEntryId).ConfigureAwait(false);
                }

                AvailableEventModels = new ReadOnlyDictionary<string, EventModel>(refreshedEventModels);
                await OnLogMessageAsync("Available events data refreshed.").ConfigureAwait(false);
            }
        }
    }
}
