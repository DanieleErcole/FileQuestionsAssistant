namespace Core.Utils.Converters;

public static class DegreesHelper {
    
    private const int FileUnitToDegrees = 60000;

    public static int? ToDegrees(int? fileValue) {
        if (fileValue is not { } number) return null;
        return number / FileUnitToDegrees;
    }

    public static string? ToDegreeString(int? value) => value is null ? null : value + " °";

}