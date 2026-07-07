using System.Text.Json.Serialization;

namespace Blazor_ApexGantt.Models;

// The chart serializer registers a JsonStringEnumConverter with a camelCase naming policy,
// so single-word members serialize to their lowercase form (Light -> "light"). DependencyType
// is the exception: its values are upper-case codes, so it carries its own default-naming
// converter that overrides the global policy.

/// <summary>Color theme preset.</summary>
public enum ThemeMode
{
    Light,
    Dark
}

/// <summary>Visual type of a task: a bar or a milestone diamond.</summary>
public enum TaskType
{
    Task,
    Milestone
}

/// <summary>
/// Dependency relationship type: Finish-to-Start, Finish-to-Finish, Start-to-Finish,
/// Start-to-Start. Serialized as the upper-case code the core expects (FS, FF, SF, SS).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<DependencyType>))]
public enum DependencyType
{
    FF,
    FS,
    SF,
    SS
}

/// <summary>Sort direction for a column.</summary>
public enum SortDirection
{
    Asc,
    Desc
}

/// <summary>Export output format.</summary>
public enum GanttExportFormat
{
    Svg,
    Png,
    Pdf
}

/// <summary>Placement of a bar label relative to the task bar.</summary>
public enum BarLabelPosition
{
    Inside,
    Left,
    Right,
    Auto
}

/// <summary>Text and layout direction.</summary>
public enum TextDirection
{
    Ltr,
    Rtl,
    Auto
}

/// <summary>Orientation of an annotation line/region.</summary>
public enum Orientation
{
    Horizontal,
    Vertical
}

/// <summary>Unit used when snapping drag/resize commits.</summary>
public enum SnapUnit
{
    Day,
    Hour,
    Minute
}

/// <summary>Unit for dependency lag: working days (calendar-aware) or raw calendar days.</summary>
public enum LagUnit
{
    Working,
    Calendar
}

/// <summary>What happens when a drag/resize commit lands on a non-working day.</summary>
public enum DragSnapMode
{
    Next,
    Previous,
    Allow
}
