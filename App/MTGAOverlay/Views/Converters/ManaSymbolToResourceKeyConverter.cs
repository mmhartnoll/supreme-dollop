using MindSculptor.App.MtgaOverlay.DataTypes;
using System;
using System.Globalization;
using System.Windows.Data;

namespace MindSculptor.App.MtgaOverlay.Views.Converters
{
    internal class ManaSymbolToResourceKeyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var formatString = (parameter as string)!;
            var manaSymbolObject = (value as ManaCost)!;

            if (manaSymbolObject.Code == "1")
                return string.Format(formatString, manaSymbolObject.Count);
            return string.Format(formatString, manaSymbolObject.Code);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
