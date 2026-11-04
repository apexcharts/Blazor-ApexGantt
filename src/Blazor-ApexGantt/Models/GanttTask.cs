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
    /// Task start date. Accepts a string, parsed with <see cref="GanttOptions.InputDateFormat"/>
    /// (the core default is "MM-DD-YYYY"), or a <see cref="System.DateTime"/> or
    /// <see cref="System.DateOnly"/>, which the wrapper always writes as yyyy-MM-dd (the time of
    /// day is dropped). When you pass DateTime or DateOnly values, set
    /// <see cref="GanttOptions.InputDateFormat"/> to "YYYY-MM-DD" so the edit dialog and the dates
    /// the chart writes back match them. A parent task may omit both dates: its summary bar spans
    /// its children.
    /// </summary>
    [JsonConverter(typeof(GanttDateConverter))]
    public object? StartTime { get; set; }

    /// <summary>
    /// Task end date, in the same forms as <see cref="StartTime"/>. Required for a bar. A milestone
    /// needs only <see cref="StartTime"/>: set <see cref="Type"/> to <see cref="TaskType.Milestone"/>,
    /// which is the clearest form and works on every apexgantt version. With the bundled apexgantt
    /// 3.15.0 (and up to 3.19), a task with a start, no end and no milestone type makes the core
    /// throw "Task must have an id, start, and end date", and the chart does not render. From
    /// apexgantt 3.20.0 a task with no end and no <see cref="Type"/> is drawn as a milestone; an
    /// explicit <see cref="TaskType.Task"/> with no end still throws.
    /// </summary>
    [JsonConverter(typeof(GanttDateConverter))]
    public object? EndTime { get; set; }

    /// <summary>Completion percentage (0-100).</summary>
    public double? Progress { get; set; }

    /// <summary>
    /// Visual type of the task: <see cref="TaskType.Task"/> renders a bar from
    /// <see cref="StartTime"/> to <see cref="EndTime"/>, <see cref="TaskType.Milestone"/> renders a
    /// diamond on <see cref="StartTime"/> and needs no <see cref="EndTime"/>. When omitted, a task
    /// with both dates is a bar; see <see cref="EndTime"/> for a task with no end date.
    /// </summary>
    public TaskType? Type { get; set; }

    /// <summary>Parent task id for nested/child tasks.</summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Dependency on another task. Accepts a plain task-id string (treated as Finish-to-Start
    /// with zero lag) or a <see cref="TaskDependency"/> for typed relationships with lag/lead.
    /// Dependencies are drawn as arrows and feed the critical path; they never move tasks.
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
    /// Whether a task with children draws a summary (group) bar spanning its descendants'
    /// earliest start and latest end. Defaults to true for every task that has children, so a
    /// parent gets a summary bar without setting this. While it is a summary bar, the task's own
    /// <see cref="StartTime"/> and <see cref="EndTime"/> are not used for it (nothing is logged).
    /// Set false, and give the task both dates, to draw a parent's own dates as a normal bar.
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
