using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Core.Utils;

public class ColorConverter : JsonConverter<Color> {
    public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) 
        => ColorTranslator.FromHtml(reader.GetString() ?? string.Empty);
    public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options) 
        => writer.WriteStringValue("#" + value.R.ToString("X2") + value.G.ToString("X2") + value.B.ToString("X2").ToLower());
}