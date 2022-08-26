using MindSculptor.App.MtgaOverlay.LogMonitoring.LogEvents.ViewChangeLogEvents;
using MindSculptor.App.MtgaOverlay.ViewModels;
using System.Threading.Tasks;
using System.Windows;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal partial class OverlayContentModel
    {
        private ViewModel subContent = ViewModel.Empty;

        public ViewModel SubContent
        {
            get => subContent;
            private set => SetProperty(ref subContent, value, nameof(SubContent));
        }

        private async Task SetActiveViewAsync(ViewChangeLogEvent logEvent)
            => await (logEvent.TargetViewType switch
            {
                ViewType.BoosterInventory => Application.Current.Dispatcher.InvokeAsync(DisplayBoosterInventoryViewAsync).Task,
                ViewType.Home             => Application.Current.Dispatcher.InvokeAsync(DisplayHomeView).Task,
                _                         => Task.CompletedTask
            })
            .ConfigureAwait(false);

        private void DisplayHomeView()
        {
            if (SubContent is MenuFrameViewModel menuFrameViewModel)
                menuFrameViewModel.SubContent = ViewModel.Empty;
            else
                SubContent = new MenuFrameViewModel(ProfileModel, DisplayBoosterInventoryViewAsync);
        }

        private async Task DisplayBoosterInventoryViewAsync()
        {
            var boosterInventoryViewModel = await BoosterInventoryViewModel.LoadAsync(ProfileModel.InventoryModel).ConfigureAwait(false);

            if (SubContent is MenuFrameViewModel menuFrameViewModel)
                menuFrameViewModel.SubContent = boosterInventoryViewModel;
            else
            {
                menuFrameViewModel = new MenuFrameViewModel(ProfileModel, DisplayBoosterInventoryViewAsync);
                menuFrameViewModel.SubContent = boosterInventoryViewModel;
                SubContent = menuFrameViewModel;
            }
        }
    }
}
