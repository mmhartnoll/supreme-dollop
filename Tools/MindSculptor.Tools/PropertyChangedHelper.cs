using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace MindSculptor.Tools
{
    public class PropertyChangedHelper
    {
        private readonly INotifyPropertyChanged propertyOwner;
        private readonly PropertyChangedEventHandler eventHandler;

        public PropertyChangedHelper(INotifyPropertyChanged propertyOwner, PropertyChangedEventHandler eventHandler)
        {
            this.propertyOwner = propertyOwner;
            this.eventHandler = eventHandler;
        }

        public void SetProperty<T>(ref T field, T value, string propertyName, IEqualityComparer<T>? comparer = null)
        {
            if ((comparer ?? EqualityComparer<T>.Default).Equals(field, value))
                return;
            field = value;
            RaisePropertyChanged(propertyName);
        }

        private void RaisePropertyChanged(string propertyName)
        {
            VerifyPropertyName(propertyName);
            eventHandler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [Conditional("DEBUG")]
        private void VerifyPropertyName(string propertyName)
        {
            if (TypeDescriptor.GetProperties(propertyOwner)[propertyName] == null)
                throw new InvalidOperationException($"Invalid propery name '{propertyName}'.");
        }
    }
}
