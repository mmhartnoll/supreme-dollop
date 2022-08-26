using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.EventLogEvents;
using MindSculptor.Tools;
using MindSculptor.Tools.Exceptions;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal partial class OverlayContentModel
    {
        private NullableReference<EventsModel> eventsModel = null;

        public EventsModel EventsModel
        {
            get => eventsModel.HasValue ? eventsModel.Value : throw new PropertyUndefinedException(nameof(EventsModel));
            private set => SetProperty(ref eventsModel, value, nameof(EventsModel));
        }

        private Task RefreshAvailableEventsAsync(EventInfoLogEvent logEvent)
            => EventsModel.RefreshAvailableEventsAsync(logEvent);

        private async Task CreateNewEventEntryAsync(EventJoinLogEvent logEvent) 
        {
            var eventModel = EventsModel.AvailableEventModels[logEvent.MtgaEventId];
            await eventModel.SetActiveEventEntryAsync(logEvent.EventEntryId).ConfigureAwait(false);
        }
    }
}
