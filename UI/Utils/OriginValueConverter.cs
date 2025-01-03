using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Core.Utils;

namespace UI.Utils;

public class OriginValueConverter : IValueConverter {
    
    public static readonly OriginValueConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is Origin origin)
            return origin.ToFriendlyString();
        throw new InvalidOperationException("Invalid input type for converter");
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is string str) {
            if (str == Lang.Lang.OriginCenterText)
                return Origin.SlideCenter;
            if (str == Lang.Lang.OriginTopLeftCornerText)
                return Origin.TopLeftCorner;
        }
        throw new InvalidOperationException("Invalid input type for converter");
    }
    
}