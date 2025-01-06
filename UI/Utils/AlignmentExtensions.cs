using Core.Utils;

namespace UI.Utils;

public static class AlignmentExtensions {
    public static string? ToFriendlyString(this Alignment? alignment) => alignment switch {
        Alignment.Left => Lang.Lang.AlignmentLeft,
        Alignment.Center => Lang.Lang.AlignmentCenter,
        Alignment.Right => Lang.Lang.AlignmentRight,
        _ => null
    };
    
    public static string ToFriendlyString(this Alignment alignment) => alignment switch {
        Alignment.Left => Lang.Lang.AlignmentLeft,
        Alignment.Center => Lang.Lang.AlignmentCenter,
        Alignment.Right => Lang.Lang.AlignmentRight,
        _ => string.Empty
    };
}