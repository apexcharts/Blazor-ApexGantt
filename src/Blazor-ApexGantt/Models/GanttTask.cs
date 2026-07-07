using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor_ApexGantt.Serialization;

namespace Blazor_ApexGantt.Models;

/// <summary>
/// Represents a task in the gantt chart. Mirrors the core <c>TaskInput</c> contract (apexgantt 3.15).
/// </summary>
public class GanttTask
{
    /// <summary>Unique identifier for the task.</summary>
    public string? Id { get; set; }

    /// <summary>Task name/title.</summary>
    public string? Name { get; set; }

    /// <summary>
    /// Task start date. Accepts a string (parsed with the chart's input date format, default
    /// YYYY-MM-DD) or a <see cref="System.DateTime"/> (serialized as yyyy-MM-dd).
    /// </summary>
    [JsonConverter(typeof(GanttDateConverter))]
    public object? StartTime { get; set; }

    /// <summary>
    /// Task end date. When omitted, the task renders as a milestone on <see cref="StartTime"/>.
    /// Accepts a string or a <see cref="System.DateTime"/>.
    /// </summary>
    [JsonConverter(typeof(GanttDateConverter))]
    public object? EndTime { get; set; }

    /// <summary>Completion percentage (0-100).</summary>
    public double? Progress { get; set; }

    /// <summary>Visual type of the task (bar or milestone).</summary>
    public TaskType? Type { get; set; }

    /// <summary>Parent task id for nested/child tasks.</summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Dependency on another task. Accepts a plain task-id string (treated as Finish-to-Start
    /// with zero lag) or a <see cref="TaskDependency"/> for typed relationships with lag/lead.
    /// </summary>
    public object? Dependency { get; set; }

    /// <summary>Background color for this task's bar.</summary>
    public string? BarBackgroundColor { get; set; }

    /// <summary>Background color for this task's row.</summary>
    public string? RowBackgroundColor { get; set; }

    /// <summary>Baseline (planned) dates rendered below the actual bar when baselines are enabled.</summary>
    public BaselineInput? Baseline { get; set; }

    /// <summary>People assigned to this task, shown as avatars in the assignees column.</summary>
    public List<Assignee>? Assignees { get; set; }

    /// <summary>Working segments of a split task, rendered as separate bar pieces.</summary>
    public List<TaskSegment>? Segments { get; set; }

    /// <summary>
    /// When true, this task renders as a summary (group) bar whose span is computed from its
    /// descendants. The task's own start/end are ignored.
    /// </summary>
    public bool? ShowSummaryBar { get; set; }

    /// <summary>Whether this task's children are collapsed (hidden) in the task list.</summary>
    public bool? Collapsed { get; set; }

    /// <summary>
    /// Any additional task fields not modeled above are serialized inline (the core accepts
    /// arbitrary extra fields on a task). Also captures unmapped fields when tasks are
    /// deserialized from event payloads.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; set; }
}
