namespace Core.Utils;

public static class EMUsHelper {

    private const int EMUPerInch = 914400;
    private const double EMUPerCentimeter = EMUPerInch / 2.54f;

    public static double? FromCentimeters(double? x) => x * EMUPerCentimeter;

    public static double? ToCentimeters(double? x) {
        if (x is not { } number) return null;
        return Math.Round(number / EMUPerCentimeter, 2);
    }

}