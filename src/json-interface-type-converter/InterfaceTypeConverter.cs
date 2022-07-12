using System.Text.Json;
using System.Text.Json.Serialization;

namespace Somerled.Json;
public class InterfaceTypeConverter<T> : JsonConverter<T> where T : class
{
    private readonly Dictionary<string, Type?> _sources = new();
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var readerClone = reader;
        if (readerClone.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException();
        }

        readerClone.Read();
        if (readerClone.TokenType != JsonTokenType.PropertyName)
        {
            throw new JsonException();
        }

        var propertyName = readerClone.GetString() ?? string.Empty;
        if (propertyName != "$concreteType")
        {
            throw new JsonException();
        }

        readerClone.Read();
        if (readerClone.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }

        var typeValue = readerClone.GetString() ?? string.Empty;
        var entityType = GetCustomType(typeValue);

        var deserialized = JsonSerializer.Deserialize(ref reader, entityType!, options);
        return deserialized as T;
    }

    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
    {
        if(value is null)
        {
            JsonSerializer.Serialize(writer, null as T, options);
            return;
        }

        var type = value.GetType();
        using var jsonDocument = JsonDocument.Parse(JsonSerializer.Serialize(value, type, options));
        writer.WriteStartObject();
        writer.WriteString("$concreteType", type.FullName);

        foreach (var property in jsonDocument.RootElement.EnumerateObject().Where(property => property.Name != "$concreteType"))
        {
            property.WriteTo(writer);
        }
        writer.WriteEndObject();
    }

    private Type? GetCustomType(string typeName)
    {
        if (_sources.ContainsKey(typeName))
        {
            return _sources[typeName];
        }

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();

        foreach (var t in assemblies.Select(assembly => assembly.GetType(typeName, false)).Where(t => t != null))
        {
            _sources.Add(typeName, t);
            return t;
        }

        throw new ArgumentException("Type " + typeName + " doesn't exist in the current app domain");
    }

}
