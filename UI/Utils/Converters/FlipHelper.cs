namespace UI.Utils.Converters;

public static class FlipHelper {

    public static string ToFriendlyString(this bool value) => value ? Lang.Lang.YesText : Lang.Lang.NoText;
    
}