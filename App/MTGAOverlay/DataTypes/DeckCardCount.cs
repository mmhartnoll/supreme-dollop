using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace MindSculptor.App.MtgaOverlay.DataTypes
{
    internal class DeckCardCount : INotifyPropertyChanged
    {
        private int count;

        public MatchCardView Card { get; }
        public int Count
        {
            get => count;
            set => SetProperty(ref count, value, nameof(Count));
        }
        public int TotalCount { get; }

        public DeckCardCount(MatchCardView card, int totalCount)
        {
            Card = card;
            Count = TotalCount = totalCount;
        }


        public event PropertyChangedEventHandler? PropertyChanged = delegate { };

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
    }
}
