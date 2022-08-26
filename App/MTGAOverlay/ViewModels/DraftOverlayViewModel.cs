using MindSculptor.App.MtgaOverlay.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace MindSculptor.App.MtgaOverlay.ViewModels
{
    internal class DraftOverlayViewModel : ViewModel
    {
        public DraftOverlayModel Model { get; }

        public DraftOverlayViewModel(DraftOverlayModel model)
        {
            Model = model;
        }
    }
}
