using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.DraftLogEvents;
using MindSculptor.App.MtgaOverlay.ViewModels;
using MindSculptor.Tools;
using MindSculptor.Tools.Exceptions;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal partial class OverlayContentModel
    {
        private NullableReference<DraftOverlayModel> draftOverlayModel = null;

        public DraftOverlayModel DraftOverlayModel
        {
            get => draftOverlayModel.HasValue ? draftOverlayModel.Value : throw new PropertyUndefinedException(nameof(DraftOverlayModel));
            private set => SetProperty(ref draftOverlayModel, value, nameof(DraftOverlayModel));
        }

        private async Task SetDraftPickOptionsAsync(DraftPickOptionsLogEvent logEvent)
        {
            if (draftOverlayModel.HasValue && DraftOverlayModel.MtgaEventId == logEvent.MtgaEventId)
                await DraftOverlayModel.SetDraftPickOptions(logEvent).ConfigureAwait(false);
            else
            {
                var eventModel = EventsModel.AvailableEventModels[logEvent.MtgaEventId];
                DraftOverlayModel = new DraftOverlayModel(DataContext, cardModelCache, eventModel, ProfileModel.InventoryModel);
                DraftOverlayModel.LogMessageAsync      += OnLogMessageAsync;
                DraftOverlayModel.LogErrorMessageAsync += OnLogErrorMessageAsync;
                await DraftOverlayModel.SetDraftPickOptions(logEvent).ConfigureAwait(false);
                var draftOverlayViewModel = new DraftOverlayViewModel(DraftOverlayModel);

                if (SubContent is MenuFrameViewModel menuFrameViewModel)
                    menuFrameViewModel.SubContent = draftOverlayViewModel;
                else
                {
                    menuFrameViewModel = new MenuFrameViewModel(ProfileModel, DisplayBoosterInventoryViewAsync);
                    menuFrameViewModel.SubContent = draftOverlayViewModel;
                    SubContent = menuFrameViewModel;
                }
            }
        }

        private Task SetDraftPickSelectionAsync(DraftPickSelectionLogEvent logEvent)
            => DraftOverlayModel.SetDraftPickSelection(logEvent);
    }
}
