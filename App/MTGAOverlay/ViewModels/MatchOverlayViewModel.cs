using MindSculptor.App.MtgaOverlay.Models;
using MindSculptor.App.MtgaOverlay.ViewModels.Commands;
using System.Windows.Input;

namespace MindSculptor.App.MtgaOverlay.ViewModels
{
    internal class MatchOverlayViewModel : ViewModel
    {
        private bool isDeckListVisible = true;

        public MatchOverlayModel Model { get; }

        public bool IsDeckListVisible
        {
            get => isDeckListVisible;
            set => SetProperty(ref isDeckListVisible, value, nameof(IsDeckListVisible));
        }

        public ICommand ToggleDeckListVisibilityCommand { get; }

        public MatchOverlayViewModel(MatchOverlayModel model)
        {
            Model = model;
            ToggleDeckListVisibilityCommand = new RelayCommand(ToggleDeckListVisibility);
        }

        private void ToggleDeckListVisibility()
            => IsDeckListVisible = !IsDeckListVisible;
    }
}
