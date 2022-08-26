using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class ItemCount<T> : INotifyPropertyChanged
    {
        private int count;

        public T Item { get; }
        public int Count 
        {
            get => count;
            set => SetProperty(ref count, value, nameof(Count));
        }

        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

        public ItemCount(T item, int count)
        {
            Item  = item;
            Count = count;
        }

        protected void SetProperty<TProperty>(ref TProperty field, TProperty value, string propertyName, IEqualityComparer<TProperty>? comparer = null)
        {
            if ((comparer ?? EqualityComparer<TProperty>.Default).Equals(field, value))
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
    }
}
