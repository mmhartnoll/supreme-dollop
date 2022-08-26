using MindSculptor.App.MtgaOverlay.Models;
using System;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.ViewModels
{
    internal class MenuFrameViewModel : ViewModel
    {
        private ViewModel subContent = ViewModel.Empty;

        public ProfileModel ProfileModel { get; }

        public ViewModel SubContent 
        {
            get => subContent;
            set => SetProperty(ref subContent, value, nameof(SubContent));
        }

        public MenuFrameViewModel(ProfileModel profileModel, Func<Task> displayBoosterChamberViewModel)
            => ProfileModel = profileModel;
    }
}
