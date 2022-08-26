using MindSculptor.App.MtgaOverlay.DataTypes;
using MindSculptor.Tools.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace MindSculptor.App.MtgaOverlay.Views.Converters
{
    internal class ManaSymbolSelectManyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var manaSymbols = (value as IEnumerable<ManaCost>)!;

            var expandedSymbols = new List<ManaCost>();
            foreach (var manaSymbol in manaSymbols)
                if (manaSymbol.Code == "1")
                    expandedSymbols.Add(manaSymbol);
                else
                    for (int i = 0; i < manaSymbol.Count; i++)
                        expandedSymbols.Add(manaSymbol);

            return expandedSymbols.Enumerate();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
