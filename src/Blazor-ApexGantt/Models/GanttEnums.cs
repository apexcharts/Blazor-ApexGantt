using System.Text.Json.Serialization;

namespace Blazor_ApexGantt.Models;

// The chart serializer registers a JsonStringEnumConverter with a camelCase naming policy,
// so single-word members serialize to their lowercase form (Light -> "light"). DependencyType
// is the exception: its values are upper-case codes, so it carries its own default-naming
// converter that overrides the global policy.

/// <summary>Color theme preset.</summary>
public enum ThemeMode
{
    /// <summary>The light theme (sent as "light").</summary>
    Light,
    /// <summary>The dark theme (sent as "dark").</summary>
    Dark
}

/// <summary>Visual type of a task: a bar or a milestone diamond.</summary>
public enum TaskType
{
    /// <summary>A bar drawn from the task's start to its end.</summary>
    Task,
    /// <summary>A diamond drawn at the task's start; needs no end.</summary>
    Milestone
}

/// <summary>
/// Dependency relationship type: Finish-to-Start, Finish-to-Finish, Start-to-Finish,
/// Start-to-Start. Serialized as the upper-case code the core expects (FS, FF, SF, SS).
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<DependencyType>))]
public enum DependencyType
{
    /// <summary>Finish-to-Finish: the successor finishes after the predecessor finishes.</summary>
    FF,
    /// <summary>Finish-to-Start: the successor starts after the predecessor finishes.</summary>
    FS,
    /// <summary>Start-to-Finish: the successor finishes after the predecessor starts.</summary>
    SF,
    /// <summary>Start-to-Start: the successor starts after the predecessor starts.</summary>
    SS
}

/// <summary>Sort direction for a column.</summary>
public enum SortDirection
{
    /// <summary>Ascending order (sent as "asc").</summary>
    Asc,
    /// <summary>Descending order (sent as "desc").</summary>
    Desc
}

/// <summary>Export output format: SVG (vector), or PNG and PDF, which are raster images.</summary>
public enum GanttExportFormat
{
    /// <summary>An SVG file (vector).</summary>
    Svg,
    /// <summary>A PNG image (raster).</summary>
    Png,
    /// <summary>A single-page PDF that embeds a raster image of the chart.</summary>
    Pdf
}

/// <summary>Placement of a bar label relative to the task bar.</summary>
public enum BarLabelPosition
{
    /// <summary>Centred inside the bar.</summary>
    Inside,
    /// <summary>Before the bar; the timeline reserves leading space so the label stays visible.</summary>
    Left,
    /// <summary>After the bar (the core's default).</summary>
    Right,
    /// <summary>Inside or right of the bar, chosen from the bar's width.</summary>
    Auto
}

/// <summary>Text and layout direction.</summary>
public enum TextDirection
{
    /// <summary>Left to right (the core's default).</summary>
    Ltr,
    /// <summary>Right to left: the layout flows right to left and the timeline axis is mirrored.</summary>
    Rtl,
    /// <summary>Defers to the direction of the document or the containing element.</summary>
    Auto
}

/// <summary>
/// Orientation of an annotation's label text. It turns only the label: the line or region
/// itself still runs down the task rows.
/// </summary>
public enum Orientation
{
    /// <summary>The label reads left to right (the core's default).</summary>
    Horizontal,
    /// <summary>The label runs up the annotation's edge.</summary>
    Vertical
}

/// <summary>Unit used when snapping drag/resize commits.</summary>
public enum SnapUnit
{
    /// <summary>Snap to whole days (the core's default).</summary>
    Day,
    /// <summary>Snap to hours, for tasks scheduled with an input date format that has time tokens.</summary>
    Hour,
    /// <summary>Snap to minutes, for tasks scheduled with an input date format that has time tokens.</summary>
    Minute
}

/// <summary>
/// Values for <see cref="TaskDependency.LagUnit"/>, which has no effect: the core has never read
/// it, and apexgantt 3.20.0 deprecates it. Lag counts working days when a working calendar is
/// active and calendar days otherwise.
/// </summary>
[Obsolete("LagUnit has no effect: the core has never read it, and apexgantt 3.20.0 deprecates it. Lag counts working days when a working calendar is active and calendar days otherwise.")]
public enum LagUnit
{
    /// <summary>Working days. Has no effect; see <see cref="LagUnit"/>.</summary>
    Working,
    /// <summary>Calendar days. Has no effect; see <see cref="LagUnit"/>.</summary>
    Calendar
}

/// <summary>What happens when a drag/resize commit lands on a non-working day.</summary>
public enum DragSnapMode
{
    /// <summary>Snap forward to the next working day (the core's default).</summary>
    Next,
    /// <summary>Snap back to the previous working day.</summary>
    Previous,
    /// <summary>Allow the task to start or end on a non-working day.</summary>
    Allow
}
