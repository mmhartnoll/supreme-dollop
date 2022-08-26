using MindSculptor.App.MtgaOverlay.Models;
using MindSculptor.Tools;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.ViewModels
{
    internal class OverlayContentViewModel : ViewModel
    {
        private bool isModelLoaded = false;

        public bool IsModelLoaded
        {
            get => isModelLoaded;
            private set => SetProperty(ref isModelLoaded, value, nameof(IsModelLoaded));
        }
        public OverlayContentModel Model { get; }

        public OverlayContentViewModel(OverlayContentModel model)
        {
            Model = model;
            Model.ModelLoadedAsync += OnModelLoaded;
        }

        private Task OnModelLoaded(NullableReference<object> sender, EventArgs eventArgs)
        {
            IsModelLoaded = true;
            return Task.CompletedTask;
        }
    }
}
