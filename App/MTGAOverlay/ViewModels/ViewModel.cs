﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace MindSculptor.App.MtgaOverlay.ViewModels
{
    internal abstract class ViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public static ViewModel Empty => new EmptyViewModel();

        protected void SetProperty<T>(ref T field, T value, string propertyName, IEqualityComparer<T>? comparer = null)
        {
            if ((comparer ?? EqualityComparer<T>.Default).Equals(field, value))
                return;
            field = value;
            OnPropertyChanged(propertyName);
        }

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

        private class EmptyViewModel : ViewModel { }
    }
}
