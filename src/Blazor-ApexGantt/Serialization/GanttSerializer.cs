using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor_ApexGantt.Serialization;

/// <summary>
/// Central System.Text.Json configuration for serializing gantt options, tasks, and method
/// arguments to the JSON shape the apexgantt core expects (camelCase keys, enums as camelCase
/// strings, nulls omitted).
/// </summary>
public static class GanttSerializer
{
    /// <summary>Shared serializer options used for every payload sent to the core.</summary>
    public static readonly JsonSerializerOptions Default = Build();

    private static JsonSerializerOptions Build()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };
        // Enums serialize as camelCase strings by default; DependencyType overrides this with its
        // own converter attribute because its values are upper-case codes (FS, FF, ...).
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        return options;
    }

    /// <summary>Serializes a value to JSON using <see cref="Default"/>.</summary>
    public static string Serialize<T>(T value) => JsonSerializer.Serialize(value, Default);
}
