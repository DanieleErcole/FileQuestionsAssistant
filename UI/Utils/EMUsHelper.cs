namespace UI.Utils;

public static class EMUsHelper {

    private const int EMUPerInch = 914400;
    private const int EMUPerHalfPoint = 6350;
    private const double EMUPerCentimeter = EMUPerInch * 2.54f;

    public static (double, double) FromCentimeters(double x, double y) {
        return (x * EMUPerCentimeter, y * EMUPerCentimeter);
    }

    public static (double, double) ToCentimeters(int x, int y) {
        return (x / EMUPerCentimeter, y / EMUPerCentimeter);
    }
    
    public static (int, int) FromInches(int x, int y) {
        return (x * EMUPerInch, y * EMUPerInch);
    }

    public static (int, int) ToInches(int x, int y) {
        return (x / EMUPerInch, y / EMUPerInch);
    }
    
    public static (int, int) FromHalfPoints(int x, int y) {
        return (x * EMUPerHalfPoint, y * EMUPerHalfPoint);
    }

    public static (int, int) ToHalfPoints(int x, int y) {
        return (x / EMUPerHalfPoint, y / EMUPerHalfPoint);
    }

}