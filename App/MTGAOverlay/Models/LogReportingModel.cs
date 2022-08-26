using MindSculptor.App.MtgaOverlay.Models.Events;
using MindSculptor.Tools;
using System.Threading.Tasks;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal class LogReportingModel
    {
        public event AsyncEventHandler<LogMessageEventArgs> LogErrorMessageAsync = delegate { return Task.CompletedTask; };
        public event AsyncEventHandler<LogMessageEventArgs> LogMessageAsync      = delegate { return Task.CompletedTask; };

        public LogReportingModel(OverlayContentModel overlayContentModel)
        {
            overlayContentModel.LogErrorMessageAsync += OnLogErrorMessageAsync;
            overlayContentModel.LogMessageAsync += OnLogMessageAsync;
        }
        protected async Task OnLogMessageAsync(NullableReference<object> sender, LogMessageEventArgs eventArgs)
            => await LogMessageAsync.InvokeAsync(sender, eventArgs).ConfigureAwait(false);

        protected async Task OnLogErrorMessageAsync(NullableReference<object> sender, LogMessageEventArgs eventArgs)
            => await LogErrorMessageAsync.InvokeAsync(sender, eventArgs).ConfigureAwait(false);
    }
}
