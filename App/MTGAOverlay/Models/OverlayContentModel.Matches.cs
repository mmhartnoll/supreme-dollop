using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents;
using MindSculptor.App.MtgaOverlay.ViewModels;
using MindSculptor.Tools;
using MindSculptor.Tools.Exceptions;
using System;
using System.Threading.Tasks;
using System.Windows;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal partial class OverlayContentModel
    {
        private NullableReference<MatchOverlayModel> matchOverlayModel = null;

        public MatchOverlayModel MatchOverlayModel
        {
            get => matchOverlayModel.HasValue ? matchOverlayModel.Value : throw new PropertyUndefinedException(nameof(MatchOverlayModel));
            private set => SetProperty(ref matchOverlayModel, value, nameof(MatchOverlayModel));
        }

        private async Task CreateNewMatchOverlayAsync(MatchCreationLogEvent logEvent)
        {
            var eventModel = EventsModel.AvailableEventModels[logEvent.MtgaEventId];
            await eventModel.SetActiveEventEntryAsync(logEvent.EventEntryId).ConfigureAwait(false);
            var eventEntryModel = eventModel.ActiveEventEntryModel;
            MatchOverlayModel = await MatchOverlayModel.LoadAsync(DataContext, cardModelCache, ProfileModel, eventModel, eventEntryModel, logEvent).ConfigureAwait(false);
            MatchOverlayModel.LogMessageAsync += OnLogMessageAsync;
            MatchOverlayModel.LogErrorMessageAsync += OnLogErrorMessageAsync;
            MatchOverlayModel.MatchCompletedAsync += OnMatchCompletedAsync;
            SubContent = new MatchOverlayViewModel(MatchOverlayModel);
        }

        private async Task AggregateGameStateFeedbackAsync(AggregateGameStateUpdateLogEvent logEvent)
        {
            foreach (var subLogEvent in logEvent.LogEvents)
                await ProcessLogEventAsync(subLogEvent).ConfigureAwait(false);
        }

        private Task UpdateMatchDeckConfigurationAsync(MatchDeckConfigurationUpdateLogEvent logEvent)
            => MatchOverlayModel.UpdateDeckConfigurationAsync(logEvent);

        private Task UpdateMatchGameState(GameStateUpdateLogEvent logEvent)
            => MatchOverlayModel.UpdateGameStateAsync(logEvent);

        private Task OnMatchCompletedAsync(NullableReference<object> sender, EventArgs eventArgs)
            => Application.Current.Dispatcher.InvokeAsync(DisplayHomeView).Task;
    }
}
