namespace Core.Utils;

public static class EMUsHelper {

    private const int EMUPerInch = 914400;
    private const int EMUPerHalfPoint = 6350;
    private const double EMUPerCentimeter = EMUPerInch / 2.54f;

    public static double? FromCentimeters(double? x) => x * EMUPerCentimeter;

    public static double? ToCentimeters(double? x) => x / EMUPerCentimeter;

    public static double? FromInches(double? x) => x * EMUPerInch;

    public static double? ToInches(double? x) => x / EMUPerInch;

    public static double? FromHalfPoints(double? x) => x * EMUPerHalfPoint;

    public static double? ToHalfPoints(double? x) => x / EMUPerHalfPoint;

}