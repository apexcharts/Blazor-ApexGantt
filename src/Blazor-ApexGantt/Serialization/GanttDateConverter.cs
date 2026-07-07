using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor_ApexGantt.Serialization;

/// <summary>
/// Serializes a task date that may be supplied as a string or a <see cref="DateTime"/>.
/// A <see cref="DateTime"/>/<see cref="DateOnly"/> is written as <c>yyyy-MM-dd</c> (the core's
/// default input date format); strings pass through unchanged. Applied per-property so it does
/// not affect other <c>object</c>-typed members.
/// </summary>
public class GanttDateConverter : JsonConverter<object>
{
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.String:
                return reader.GetString();
            case JsonTokenType.Null:
                return null;
            default:
                using (var doc = JsonDocument.ParseValue(ref reader))
                    return doc.RootElement.Clone();
        }
    }

    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case DateTime dt:
                writer.WriteStringValue(dt.ToString("yyyy-MM-dd"));
                break;
            case DateOnly d:
                writer.WriteStringValue(d.ToString("yyyy-MM-dd"));
                break;
            case string s:
                writer.WriteStringValue(s);
                break;
            default:
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
                break;
        }
    }
}
