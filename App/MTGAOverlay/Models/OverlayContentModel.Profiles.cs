using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ProfileLogEvents;
using MindSculptor.Tools;
using MindSculptor.Tools.Exceptions;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal partial class OverlayContentModel
    {
        private NullableReference<ProfileModel> profileModel = null;

        public ProfileModel ProfileModel
        {
            get => profileModel.HasValue ? profileModel.Value : throw new PropertyUndefinedException(nameof(ProfileModel));
            private set => SetProperty(ref profileModel, value, nameof(ProfileModel));
        }

        private async Task SetActiveProfileAsync(ProfileActiveLogEvent logEvent)
        {
            ProfileModel = await ProfileModel.LoadOrCreateAsync(DataContext, cardModelCache, logEvent.MtgaUserId, logEvent.Name, logEvent.NameId, OnLogMessageAsync).ConfigureAwait(false);
            ProfileModel.LogMessageAsync      += OnLogMessageAsync;
            ProfileModel.LogErrorMessageAsync += OnLogErrorMessageAsync;

            EventsModel = await EventsModel.LoadAsync(DataContext, ProfileModel).ConfigureAwait(false);
            EventsModel.LogMessageAsync      += OnLogMessageAsync;
            EventsModel.LogErrorMessageAsync += OnLogErrorMessageAsync;
        }

        private Task RefreshProfileInventoryAsync(ProfileInventoryInfoLogEvent logEvent)
            => ProfileModel.InventoryModel.RefreshAsync(logEvent);

        private Task RefreshProfileInventoryCardsAsync(ProfileInventoryCardsInfoLogEvent logEvent)
            => ProfileModel.InventoryModel.RefreshCardsAsync(logEvent);

        private Task UpdateProfileInventoryAsync(ProfileInventoryUpdateLogEvent logEvent)
            => ProfileModel.InventoryModel.UpdateAsync(logEvent);
    }
}
