using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Utils;

public static class ColorExtensions {
    public static string ToHexString(this Color color) {
        return ((Color?) color).ToHexString();
    }
    public static string ToHexString(this Color? color) =>
        color is {} c ? "#" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2") : string.Empty;
}

public class ColorConverter : JsonConverter<Color> {
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
        => ColorTranslator.FromHtml(reader.GetString() ?? string.Empty);
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) 
        => writer.WriteStringValue(value.ToHexString());
}