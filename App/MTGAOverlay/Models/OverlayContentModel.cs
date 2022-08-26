using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.App.MtgaOverlay.LogMonitoring;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.DraftLogEvents;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.EventLogEvents;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.MatchLogEvents;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ProfileLogEvents;
using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ViewChangeLogEvents;
using MindSculptor.Tools;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal partial class OverlayContentModel : Model
    {
        private readonly LogParser logParser;
        private readonly CardModelCache cardModelCache;

        public event AsyncEventHandler ModelLoadedAsync = delegate { return Task.CompletedTask; };

        public OverlayContentModel(DataContext dataContext, LogParser logParser)
            : base(dataContext)
        {
            this.logParser          = logParser;
            cardModelCache          = new CardModelCache(dataContext);
            logParser.PollingAsync += OnLogParserPollingAsync;

            Task.Run(async () => await ProcessLogEventsAsync().ConfigureAwait(false));
        }

        private async Task OnLogParserPollingAsync(NullableReference<object> sender, EventArgs eventArgs)
        {
            if (profileModel.HasValue)
            {
                logParser.PollingAsync -= OnLogParserPollingAsync;
                await ModelLoadedAsync.InvokeAsync(this, eventArgs).ConfigureAwait(false);
                await Application.Current.Dispatcher.InvokeAsync(DisplayHomeView).Task.ConfigureAwait(false);
            }
        }

        private async Task ProcessLogEventsAsync(CancellationToken cancellationToken = default)
        {
            await foreach (var logEvent in logParser.StreamLogEventsAsync(cancellationToken).ConfigureAwait(false))
                await ProcessLogEventAsync(logEvent).ConfigureAwait(false);
        }

        private async Task ProcessLogEventAsync(LogEvent logEvent)
            => await (logEvent switch
            {
                AggregateGameStateUpdateLogEvent      aggregateGameStateUpdateLogEvent      => AggregateGameStateFeedbackAsync   (aggregateGameStateUpdateLogEvent),
                DraftPickOptionsLogEvent              draftPickOptionsLogEvent              => SetDraftPickOptionsAsync          (draftPickOptionsLogEvent),
                DraftPickSelectionLogEvent            draftPickSelectionLogEvent            => SetDraftPickSelectionAsync        (draftPickSelectionLogEvent),
                EventInfoLogEvent                     eventInfoLogEvent                     => RefreshAvailableEventsAsync       (eventInfoLogEvent),
                EventJoinLogEvent                     eventJoinLogEvent                     => CreateNewEventEntryAsync          (eventJoinLogEvent),
                GameStateUpdateLogEvent               gameStateUpdateLogEvent               => UpdateMatchGameState              (gameStateUpdateLogEvent),
                MatchCreationLogEvent                 matchCreationLogEvent                 => CreateNewMatchOverlayAsync        (matchCreationLogEvent),
                MatchDeckConfigurationUpdateLogEvent  matchDeckConfigurationUpdateLogEvent  => UpdateMatchDeckConfigurationAsync (matchDeckConfigurationUpdateLogEvent),
                ProfileActiveLogEvent                 profileActiveLogEvent                 => SetActiveProfileAsync             (profileActiveLogEvent),
                ProfileInventoryCardsInfoLogEvent     profileInventoryCardsInfoLogEvent     => RefreshProfileInventoryCardsAsync (profileInventoryCardsInfoLogEvent),
                ProfileInventoryInfoLogEvent          profileInventoryInfoLogEvent          => RefreshProfileInventoryAsync      (profileInventoryInfoLogEvent),
                ProfileInventoryUpdateLogEvent        profileInventoryUpdateLogEvent        => UpdateProfileInventoryAsync       (profileInventoryUpdateLogEvent),
                ViewChangeLogEvent                    viewChangeLogEvent                    => SetActiveViewAsync                (viewChangeLogEvent),

                _                                                                           => Task.CompletedTask
            })
            .ConfigureAwait(false);
    }
}
