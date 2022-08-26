using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.App.MtgaOverlay.Models;
using MindSculptor.App.MtgaOverlay.Models.Events;
using MindSculptor.App.MtgaOverlay.ViewModels.Commands;
using MindSculptor.Tools;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MindSculptor.App.MtgaOverlay.ViewModels
{
    internal class LogReportingViewModel : ViewModel
    {
        public ObservableCollection<LogMessage> LogMessages { get; } = new ObservableCollection<LogMessage>();
        public ObservableCollection<LogMessage> LogErrorMessages { get; } = new ObservableCollection<LogMessage>();

        public LogReportingModel Model;

        public ViewModel SubContent { get; }

        public ICommand RemoveLogErrorMessageCommand { get; }

        public LogReportingViewModel(LogReportingModel model, ViewModel subContent)
        {
            Model                         = model;
            SubContent                    = subContent;
            Model.LogErrorMessageAsync   += OnLogErrorMessageAsync;
            Model.LogMessageAsync        += OnLogMessageAsync;
            RemoveLogErrorMessageCommand  = new RelayCommand<LogMessage>(RemoveLogErrorMessage);
        }

        private async Task OnLogErrorMessageAsync(NullableReference<object> sender, LogMessageEventArgs eventArgs)
            => await Application.Current.Dispatcher.InvokeAsync(() => { LogErrorMessages.Add(eventArgs.LogMessage); }).Task.ConfigureAwait(false);

        private async Task OnLogMessageAsync(NullableReference<object> sender, LogMessageEventArgs eventArgs)
        {
            await Application.Current.Dispatcher.InvokeAsync(DisplayLogMessage).Task.ConfigureAwait(false);
            _ = Application.Current.Dispatcher.InvokeAsync(RemoveLogMessageAsync);

            void DisplayLogMessage()
            {
                if (LogMessages.Count == LogMessageMaximumDisplayCount)
                    LogMessages.RemoveAt(0);
                LogMessages.Add(eventArgs.LogMessage);
            }
            async Task RemoveLogMessageAsync()
            {
                await Task.Delay(LogMessageDisplayTimeInMilliseconds);
                LogMessages.Remove(eventArgs.LogMessage);
            }
        }

        private void RemoveLogErrorMessage(LogMessage logMessage)
            => Application.Current.Dispatcher.InvokeAsync(() => { LogErrorMessages.Remove(logMessage); });

        private const int LogMessageDisplayTimeInMilliseconds = 4000;
        private const int LogMessageMaximumDisplayCount = 8;
    }
}
