using System;
using System.Globalization;
using System.Windows.Data;

namespace MindSculptor.App.MtgaOverlay.Views.Converters
{
    internal class DrawPercentageConverter : IMultiValueConverter
    {

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is int && values[1] is int)
            {
                var currentCardCount = (int)values[0];
                var currentDeckCount = (int)values[1];

                return (decimal)currentCardCount / currentDeckCount;
            }
            return 0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
