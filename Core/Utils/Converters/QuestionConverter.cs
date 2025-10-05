using System.Text.Json;
using System.Text.Json.Serialization;
using Core.Questions;

namespace Core.Utils.Converters;

public class QuestionConverter : JsonConverter<AbstractQuestion> {
    
    private static readonly Dictionary<string, Type> TypeMap;

    static QuestionConverter() {
        TypeMap = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(AbstractQuestion).IsAssignableFrom(t) && !t.IsAbstract)
            .ToDictionary(t => t.Name, t => t);
    }

    public override AbstractQuestion Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        if (!root.TryGetProperty("Type", out var typeProp))
            throw new JsonException("Missing Type property");

        var typeName = typeProp.GetString();
        if (typeName == null || !TypeMap.TryGetValue(typeName, out var type))
            throw new JsonException($"Unknown question type: {typeName}");

        return (AbstractQuestion) JsonSerializer.Deserialize(root.GetRawText(), type, options)!;
    }

    public override void Write(Utf8JsonWriter writer, AbstractQuestion value, JsonSerializerOptions options) {
        var type = value.GetType();
        var typeName = type.Name;
        var json = JsonSerializer.SerializeToElement(value, type, options);

        using var doc = JsonDocument.Parse(json.GetRawText());
        writer.WriteStartObject();
        writer.WriteString("Type", typeName);
        foreach (var prop in doc.RootElement.EnumerateObject())
            prop.WriteTo(writer);
        writer.WriteEndObject();
    }
    
}