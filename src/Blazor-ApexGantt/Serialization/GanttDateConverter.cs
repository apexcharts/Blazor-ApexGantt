using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Blazor_ApexGantt.Serialization;

/// <summary>
/// Serializes a task date that may be supplied as a string or a <see cref="DateTime"/>.
/// A <see cref="DateTime"/>/<see cref="DateOnly"/> is written as <c>yyyy-MM-dd</c> (the core's
/// default input date format) in the Gregorian calendar of the invariant culture, so a Thai
/// Buddhist, Hijri or other non-Gregorian current culture cannot shift the year; strings pass
/// through unchanged. Applied per-property so it does not affect other <c>object</c>-typed
/// members.
/// </summary>
public class GanttDateConverter : JsonConverter<object>
{
    /// <summary>
    /// Reads a string as itself, null as null, and any other JSON value as a cloned
    /// <see cref="JsonElement"/>.
    /// </summary>
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

    /// <summary>
    /// Writes a <see cref="DateTime"/> or <see cref="DateOnly"/> as an invariant-culture
    /// <c>yyyy-MM-dd</c> string, a string unchanged, and any other value with the default
    /// serializer for its runtime type.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case DateTime dt:
                writer.WriteStringValue(dt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
                break;
            case DateOnly d:
                writer.WriteStringValue(d.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
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
