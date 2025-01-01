namespace Core.Utils; 

public static class NumberHelper {
    
    private const double Tolerance = 0.001;

    public static (int, int, int) HexStringToRgb(this string number) {
        var r = int.Parse(number[..2], System.Globalization.NumberStyles.HexNumber);
        var g = int.Parse(number.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        var b = int.Parse(number.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return (r, g, b);
    }

    public static bool DoubleEquals(this double? a, double? b) {
        if (a is null || b is null) return false;
        return Math.Abs(Math.Round((double) a, 2) - Math.Round((double) b, 2)) < Tolerance;
    }

}