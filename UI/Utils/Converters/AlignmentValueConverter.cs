using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Core.Utils;

namespace UI.Utils.Converters;

public class AlignmentValueConverter : IValueConverter {
    
    public static readonly AlignmentValueConverter Instance = new();
    
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) => (value as Alignment?).ToFriendlyString();

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is not string str) 
            return null;
        
        if (str == Lang.Lang.AlignmentLeft)
            return Alignment.Left;
        if (str == Lang.Lang.AlignmentCenter)
            return Alignment.Center;
        if (str == Lang.Lang.AlignmentRight)
            return Alignment.Right;
        throw new InvalidOperationException("Invalid input type for converter");
    }
    
}