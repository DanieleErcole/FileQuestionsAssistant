namespace Core.Utils;

public enum Alignment {
    Left = 0,
    Center = 1,
    Right = 2,
}

public static class AlignmentHelper {
    public static Alignment? FromFileString(string? alignment) => alignment switch {
        "left" => Alignment.Left,
        "center" => Alignment.Center,
        "right" => Alignment.Right,
        _ => null
    };
}