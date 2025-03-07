using Avalonia;
using Avalonia.Data.Converters;
using ActivityDirectorGames.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace ActivityDirectorGames.Converters;
public class BoundsConverter : IMultiValueConverter
{
    public object Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        if (values == null || values.Count <= 0)
            return AvaloniaProperty.UnsetValue;
        if (!(values[0] is MainViewModel vm))
            return AvaloniaProperty.UnsetValue;
        if (!(values[1] is double width))
            return AvaloniaProperty.UnsetValue;
        if (!(values[2] is double height))
            return AvaloniaProperty.UnsetValue;

        vm.SizeChanged(width, height);

        return new object();
    }
}