using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.App.MtgaOverlay.Models.Events;
using MindSculptor.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

using DataContext = MindSculptor.App.AppDataContext.AppDataContext;

namespace MindSculptor.App.MtgaOverlay.Models
{
    internal abstract class Model : INotifyPropertyChanged
    {
        private bool isModelValid = true;

        protected DataContext DataContext { get; }

        public bool IsModelValid
        {
            get => isModelValid;
            protected set => SetProperty(ref isModelValid, value, nameof(IsModelValid));
        }

        public event AsyncEventHandler<LogMessageEventArgs> LogMessageAsync = delegate { return Task.CompletedTask; };
        public event AsyncEventHandler<LogMessageEventArgs> LogErrorMessageAsync = delegate { return Task.CompletedTask; };

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        protected Model(DataContext dataContext)
            => DataContext = dataContext;

        protected void SetProperty<T>(ref T field, T value, string propertyName, IEqualityComparer<T>? comparer = null)
        {
            if ((comparer ?? EqualityComparer<T>.Default).Equals(field, value))
                return;
            field = value;
            OnPropertyChanged(propertyName);
        }

        protected async Task OnLogMessageAsync(string logMessage)
            => await LogMessageAsync.InvokeAsync(this, new LogMessageEventArgs(new LogMessage(logMessage))).ConfigureAwait(false);

        protected async Task OnLogMessageAsync(NullableReference<object> sender, LogMessageEventArgs eventArgs)
            => await LogMessageAsync.InvokeAsync(sender, eventArgs).ConfigureAwait(false);

        protected async Task OnLogErrorMessageAsync(string logMessage)
            => await LogErrorMessageAsync.InvokeAsync(this, new LogMessageEventArgs(new LogMessage(logMessage))).ConfigureAwait(false);

        protected async Task OnLogErrorMessageAsync(NullableReference<object> sender, LogMessageEventArgs eventArgs)
            => await LogErrorMessageAsync.InvokeAsync(sender, eventArgs).ConfigureAwait(false);

        protected virtual void OnPropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                throw new InvalidOperationException($"Invalid propery name '{propertyName}'.");
        }
    }
}
