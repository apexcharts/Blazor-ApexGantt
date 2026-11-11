using System.Collections.Generic;
using System.Text.Json;

namespace Blazor_ApexGantt.Models;

// Event payloads are deserialized by Blazor's JSInterop, whose default options do not apply a
// string-enum converter. To stay deserialization-safe these read models use primitive/string
// fields (never the input-side C# enums) and a JsonExtensionData bag for anything unmapped.

/// <summary>Read-only view of a task as it appears in event payloads (the core's computed Task).</summary>
public class GanttTaskInfo
{
    /// <summary>Unique identifier of the task.</summary>
    public string? Id { get; set; }
    /// <summary>Display name shown in the task list and on the bar.</summary>
    public string? Name { get; set; }
    /// <summary>Start date in the chart's input date format. Empty for a summary row given no dates.</summary>
    public string? StartTime { get; set; }
    /// <summary>End date in the chart's input date format; a milestone's equals its start.</summary>
    public string? EndTime { get; set; }
    /// <summary>Completion percentage, 0 to 100.</summary>
    public double? Progress { get; set; }
    /// <summary>"task" or "milestone".</summary>
    public string? Type { get; set; }
    /// <summary>Id of the parent task; null for a top-level task.</summary>
    public string? ParentId { get; set; }
    /// <summary>Bar fill color, when the task sets its own.</summary>
    public string? BarBackgroundColor { get; set; }
    /// <summary>Row background color, when the task sets its own.</summary>
    public string? RowBackgroundColor { get; set; }
    /// <summary>Color of the label drawn inside this task's bar, when the task sets its own.</summary>
    public string? BarTextColor { get; set; }
    /// <summary>Nesting depth, counted from 1: a top-level task is 1, its children 2.</summary>
    public int? Level { get; set; }
    /// <summary>Work-breakdown-structure code (e.g. "1.2.1").</summary>
    public string? Wbs { get; set; }
    /// <summary>Whether the task draws as a summary bar spanning its children.</summary>
    public bool? ShowSummaryBar { get; set; }
    /// <summary>Whether the task's children are collapsed (hidden) in the task list.</summary>
    public bool? Collapsed { get; set; }
    /// <summary>
    /// Any fields not modeled above, keyed by their camelCase core name: for example
    /// <c>dependency</c>, <c>baseline</c>, <c>segments</c>, <c>assignees</c>,
    /// <c>predecessors</c>, <c>successors</c>, group metadata and your own custom fields.
    /// </summary>
    [System.Text.Json.Serialization.JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>A validation error for a specific field.</summary>
public class ValidationError
{
    /// <summary>Name of the field that failed validation (for example <c>startTime</c>).</summary>
    public string Field { get; set; } = string.Empty;
    /// <summary>Human-readable reason the value was rejected.</summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>One child task shifted as a side effect of dragging its parent.</summary>
public class AffectedChildTask
{
    /// <summary>Id of the child task that moved with its parent.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>The child's new start date, in the chart's input date format.</summary>
    public string NewStartTime { get; set; } = string.Empty;
    /// <summary>The child's new end date, in the chart's input date format.</summary>
    public string NewEndTime { get; set; } = string.Empty;
}

/// <summary>
/// One dependency link as <c>GetDependenciesAsync</c> returns it: the links the tasks were loaded
/// with and the ones added with <c>AddDependencyAsync</c> or drawn in the chart, less removed ones.
/// </summary>
public class DependencyLinkInfo
{
    /// <summary>Id of the predecessor task, where the arrow starts.</summary>
    public string FromId { get; set; } = string.Empty;
    /// <summary>Id of the successor task, where the arrow ends.</summary>
    public string ToId { get; set; } = string.Empty;
    /// <summary>"FS", "FF", "SF" or "SS".</summary>
    public string Type { get; set; } = string.Empty;
    /// <summary>Lag (positive) or lead (negative) in days.</summary>
    public double Lag { get; set; }
}

/// <summary>
/// The edit paths the core names in <see cref="TaskUpdateEventArgs.Source"/> and
/// <see cref="TaskUpdateSuccessEventArgs.Source"/>. The source is a plain string, so compare it
/// with these constants; a value added by a later core still arrives intact.
/// </summary>
public static class TaskUpdateSource
{
    /// <summary><c>UpdateTaskAsync</c> (and the core's <c>splitTask</c>, which calls it).</summary>
    public const string Api = "api";
    /// <summary>The edit dialog opened by double-clicking a bar.</summary>
    public const string Dialog = "dialog";
    /// <summary>Dragging a bar, including the children moved with a parent.</summary>
    public const string Drag = "drag";
    /// <summary>An inline cell edit in the task list.</summary>
    public const string Inline = "inline";
    /// <summary>Moving or resizing a focused bar with the arrow keys.</summary>
    public const string Keyboard = "keyboard";
    /// <summary>Dragging the progress handle inside a bar.</summary>
    public const string Progress = "progress";
    /// <summary>Dragging a bar's resize handle.</summary>
    public const string Resize = "resize";
}

/// <summary>A sort criterion as reported by the sortChange event.</summary>
public class SortCriterionInfo
{
    /// <summary>Key of the sorted column.</summary>
    public string Key { get; set; } = string.Empty;
    /// <summary>"asc" or "desc".</summary>
    public string Direction { get; set; } = string.Empty;
}

/// <summary>
/// Fired once per committed task edit, just before it is applied, on every edit path: drag,
/// resize, progress drag, edit dialog, inline edit, keyboard and <c>UpdateTaskAsync</c>.
/// <see cref="TaskUpdateSuccessEventArgs"/> follows once it is stored. Not raised for undo or
/// redo, or for <c>UpdateTaskAsync</c> called with <c>silent: true</c>. <see cref="Source"/> names
/// the path.
/// </summary>
public class TaskUpdateEventArgs
{
    /// <summary>Id of the task being updated.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>The partial fields being applied.</summary>
    public GanttTaskInfo? Updates { get; set; }
    /// <summary>The task as it will read once the edit is applied.</summary>
    public GanttTaskInfo? UpdatedTask { get; set; }
    /// <summary>
    /// The edit path that produced the update: one of the <see cref="TaskUpdateSource"/>
    /// constants ("api", "dialog", "drag", "inline", "keyboard", "progress" or "resize").
    /// </summary>
    public string? Source { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// Fired after a task edit is stored, on the same paths as <see cref="TaskUpdateEventArgs"/>. A
/// drag raises both this and OnTaskDragged (a resize OnTaskResized, a progress drag
/// OnTaskProgressChanged), so an app that saves on both saves the edit twice: save on this one
/// and use <see cref="Source"/> when the path matters.
/// </summary>
public class TaskUpdateSuccessEventArgs
{
    /// <summary>Id of the updated task.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>The task with the edit applied.</summary>
    public GanttTaskInfo? UpdatedTask { get; set; }
    /// <summary>
    /// The edit path that produced the update: one of the <see cref="TaskUpdateSource"/>
    /// constants ("api", "dialog", "drag", "inline", "keyboard", "progress" or "resize").
    /// </summary>
    public string? Source { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task update fails, on every edit path.</summary>
public class TaskUpdateErrorEventArgs
{
    /// <summary>Id of the task whose update failed. The task keeps its previous values.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>Message of the error the core threw ("Unknown error" when it had none).</summary>
    public string ErrorMessage { get; set; } = string.Empty;
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when the edit dialog rejects its input.</summary>
public class TaskValidationErrorEventArgs
{
    /// <summary>Id of the task being edited.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>One entry per field that failed validation.</summary>
    public List<ValidationError> Errors { get; set; } = new();
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// Fired when a task bar is dragged along the timeline to new dates. The date strings use the
/// chart's input date format.
/// </summary>
public class TaskDraggedEventArgs
{
    /// <summary>Id of the dragged task.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>Start date before the drag.</summary>
    public string OldStartTime { get; set; } = string.Empty;
    /// <summary>End date before the drag.</summary>
    public string OldEndTime { get; set; } = string.Empty;
    /// <summary>Start date after the drag.</summary>
    public string NewStartTime { get; set; } = string.Empty;
    /// <summary>End date after the drag.</summary>
    public string NewEndTime { get; set; } = string.Empty;
    /// <summary>Days moved; fractional when snapping by hour/minute.</summary>
    public double DaysMoved { get; set; }
    /// <summary>Child tasks whose dates shifted with the dragged task (empty for a task with no children).</summary>
    public List<AffectedChildTask> AffectedChildTasks { get; set; } = new();
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task bar is resized.</summary>
public class TaskResizedEventArgs
{
    /// <summary>Id of the resized task.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>"left" or "right".</summary>
    public string ResizeHandle { get; set; } = string.Empty;
    /// <summary>Start date before the resize, in the chart's input date format.</summary>
    public string OldStartTime { get; set; } = string.Empty;
    /// <summary>End date before the resize, in the chart's input date format.</summary>
    public string OldEndTime { get; set; } = string.Empty;
    /// <summary>Start date after the resize, in the chart's input date format.</summary>
    public string NewStartTime { get; set; } = string.Empty;
    /// <summary>End date after the resize, in the chart's input date format.</summary>
    public string NewEndTime { get; set; } = string.Empty;
    /// <summary>Change in duration (days); fractional when snapping by hour/minute.</summary>
    public double DurationChange { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// Fired after a task is added, by <c>AddTaskAsync</c> or by the chart's own add controls
/// (toolbar, add-task row, context menu, drawing a task).
/// </summary>
public class TaskAddedEventArgs
{
    /// <summary>Id of the added task.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>The task as the chart stored it.</summary>
    public GanttTaskInfo? Task { get; set; }
    /// <summary>Id of the parent the task was added under; null for a top-level task.</summary>
    public string? ParentId { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired after a task (and its descendants) is deleted.</summary>
public class TaskDeletedEventArgs
{
    /// <summary>Id of the deleted task.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>The task as it read when it was removed.</summary>
    public GanttTaskInfo? Task { get; set; }
    /// <summary>Ids of the descendants removed in the same delete (empty for a task with no children or a delete that did not cascade).</summary>
    public List<string> RemovedDescendantIds { get; set; } = new();
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// Fired when a task is re-parented, by <c>MoveTaskAsync</c> or by indenting or outdenting in the
/// chart (keyboard shortcuts or context menu).
/// </summary>
public class TaskMovedEventArgs
{
    /// <summary>Id of the moved task.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>Id of the previous parent; null when the task was top-level.</summary>
    public string? OldParentId { get; set; }
    /// <summary>Id of the new parent; null when the task is now top-level.</summary>
    public string? NewParentId { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task's progress changes.</summary>
public class TaskProgressChangedEventArgs
{
    /// <summary>Id of the task whose progress changed.</summary>
    public string TaskId { get; set; } = string.Empty;
    /// <summary>Progress before the change, an integer percentage from 0 to 100.</summary>
    public double OldProgress { get; set; }
    /// <summary>Progress after the change, an integer percentage from 0 to 100.</summary>
    public double NewProgress { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when a dependency is added or removed.</summary>
public class DependencyChangeEventArgs
{
    /// <summary>Id of the predecessor task, where the arrow starts.</summary>
    public string FromId { get; set; } = string.Empty;
    /// <summary>Id of the successor task, where the arrow ends.</summary>
    public string ToId { get; set; } = string.Empty;
    /// <summary>"FS", "FF", "SF" or "SS".</summary>
    public string Type { get; set; } = string.Empty;
    /// <summary>Lag (positive) or lead (negative) in days. For a removal, the value the link had before it was removed.</summary>
    public double Lag { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when a dependency arrow's geometry is recomputed.</summary>
public class DependencyArrowUpdateEventArgs
{
    /// <summary>Id of the predecessor task, where the arrow starts.</summary>
    public string FromId { get; set; } = string.Empty;
    /// <summary>Id of the successor task, where the arrow ends.</summary>
    public string ToId { get; set; } = string.Empty;
    /// <summary>"FS", "FF", "SF" or "SS".</summary>
    public string Type { get; set; } = string.Empty;
    /// <summary>Lag (positive) or lead (negative) in days, when the link has one.</summary>
    public double? Lag { get; set; }
    /// <summary>Id of the chart instance that owns the arrow.</summary>
    public string? ChartInstanceId { get; set; }
    /// <summary>Id of the chart's arrow layer that drew the arrow.</summary>
    public string? ArrowLinkInstanceId { get; set; }
}

/// <summary>Fired when the selected-task set changes.</summary>
public class SelectionChangeEventArgs
{
    /// <summary>The selected tasks after the change.</summary>
    public List<GanttTaskInfo> SelectedTasks { get; set; } = new();
    /// <summary>Ids of the selected tasks after the change.</summary>
    public List<string> SelectedIds { get; set; } = new();
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when the undo/redo history changes.</summary>
public class HistoryChangeEventArgs
{
    /// <summary>"record", "undo", "redo" or "clear".</summary>
    public string Kind { get; set; } = string.Empty;
    /// <summary>Whether there is a step to undo.</summary>
    public bool CanUndo { get; set; }
    /// <summary>Whether there is a step to redo.</summary>
    public bool CanRedo { get; set; }
    /// <summary>Number of steps on the undo stack.</summary>
    public int UndoSize { get; set; }
    /// <summary>Number of steps on the redo stack.</summary>
    public int RedoSize { get; set; }
    /// <summary>Label of the step at the top of the undo stack, if any.</summary>
    public string? TopUndoLabel { get; set; }
    /// <summary>Label of the step at the top of the redo stack, if any.</summary>
    public string? TopRedoLabel { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when the sort criteria change.</summary>
public class SortChangeEventArgs
{
    /// <summary>The active sort criteria after the change; empty for natural (input) order.</summary>
    public List<SortCriterionInfo> Criteria { get; set; } = new();
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when the active filter changes.</summary>
public class FilterChangeEventArgs
{
    /// <summary>Whether a filter is active after the change.</summary>
    public bool Active { get; set; }
    /// <summary>Number of tasks visible under the active filter.</summary>
    public int VisibleCount { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when grouping changes.</summary>
public class GroupChangeEventArgs
{
    /// <summary>Whether grouping is active after the change.</summary>
    public bool Active { get; set; }
    /// <summary>The field being grouped by; null when grouping was cleared.</summary>
    public string? Field { get; set; }
    /// <summary>Number of groups the active grouping produced (0 when cleared).</summary>
    public int GroupCount { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task-list column is resized.</summary>
public class ColumnResizeEventArgs
{
    /// <summary>Key of the column whose width changed.</summary>
    public string Key { get; set; } = string.Empty;
    /// <summary>The column's new width in pixels; null when it was reset to its automatic width.</summary>
    public int? Width { get; set; }
    /// <summary>Every manual column width in force after the change, keyed by column key, in pixels.</summary>
    public Dictionary<string, int> Widths { get; set; } = new();
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}

/// <summary>Fired when task-list columns are reordered.</summary>
public class ColumnReorderEventArgs
{
    /// <summary>Keys of the visible columns in their new left-to-right order.</summary>
    public List<string> Order { get; set; } = new();
    /// <summary>Key of the dragged column; null when the whole order was set at once.</summary>
    public string? MovedKey { get; set; }
    /// <summary>When the core raised the event, in milliseconds since the Unix epoch (JavaScript <c>Date.now()</c>).</summary>
    public long Timestamp { get; set; }
}
