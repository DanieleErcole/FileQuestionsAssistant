namespace Core.Utils; 

public static class NumberHelper {

    public static (int, int, int) HexStringToRgb(this string number) {
        var r = int.Parse(number[..2], System.Globalization.NumberStyles.HexNumber);
        var g = int.Parse(number.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        var b = int.Parse(number.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        return (r, g, b);
    }

}