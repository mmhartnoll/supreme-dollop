using MindSculptor.App.MtgaOverlay.LogMonitoring;
using MindSculptor.App.MtgaOverlay.Models;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.ViewModels
{
    internal class OverlayWindowViewModel : ViewModel
    {
        private ViewModel content = ViewModel.Empty;

        public ViewModel Content
        {
            get => content;
            private set => SetProperty(ref content, value, nameof(Content));
        }

        public OverlayWindowViewModel()
        {
            var dataContext             = DataContext.Create(DBConnectionString);
            var logParser               = new LogParser(LogDirectory, LogFileName);

            var overlayContentModel     = new OverlayContentModel(dataContext, logParser);
            var overlayContentViewModel = new OverlayContentViewModel(overlayContentModel);

            var logReportingModel       = new LogReportingModel(overlayContentModel);
            var logReportingViewModel   = new LogReportingViewModel(logReportingModel, overlayContentViewModel);

            Content                     = logReportingViewModel;
        }

        private const string DBConnectionString = @"Server=localhost\SQLEXPRESS;Database=MindSculptorApp;Trusted_Connection=True;";
        private const string LogDirectory       = @"C:\Users\mmhar\AppData\LocalLow\Wizards Of The Coast\MTGA";
        private const string LogFileName        = @"Player.log";

        //private const string LogDirectory = @"C:\Users\mmhar\OneDrive\Desktop";
        //private const string LogFileName  = @"Player.log";
    }
}
