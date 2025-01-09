namespace Core.Utils.Converters; 

public static class NumberHelper {
    
    private const double Tolerance = 0.01;

    public static (int, int, int) HexStringToRgb(this string number) {
        var r = int.Parse(number[..2], System.Globalization.NumberStyles.HexNumber);
        var g = int.Parse(number.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        var b = int.Parse(number.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return (r, g, b);
    }

    public static bool DoubleEquals(this double? a, double? b) {
        if (a is null || b is null) return false;
        return DoubleEquals((double) a, (double) b);
    }
    
    public static bool DoubleEquals(this double a, double b) {
        return Math.Abs(a - b) < Tolerance;
    }

}