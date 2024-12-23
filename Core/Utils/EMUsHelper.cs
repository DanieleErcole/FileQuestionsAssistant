namespace Core.Utils;

public static class EMUsHelper {

    private const int EMUPerInch = 914400;
    private const int EMUPerHalfPoint = 6350;
    private const double EMUPerCentimeter = EMUPerInch * 2.54f;

    public static (double, double) FromCentimeters(double x, double y) {
        return (x * EMUPerCentimeter, y * EMUPerCentimeter);
    }

    public static (double, double) ToCentimeters(double x, double y) {
        return (x / EMUPerCentimeter, y / EMUPerCentimeter);
    }
    
    public static (double, double) FromInches(double x, double y) {
        return (x * EMUPerInch, y * EMUPerInch);
    }

    public static (double, double) ToInches(double x, double y) {
        return (x / EMUPerInch, y / EMUPerInch);
    }
    
    public static (double, double) FromHalfPoints(double x, double y) {
        return (x * EMUPerHalfPoint, y * EMUPerHalfPoint);
    }

    public static (double, double) ToHalfPoints(double x, double y) {
        return (x / EMUPerHalfPoint, y / EMUPerHalfPoint);
    }

}