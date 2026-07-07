using System.Collections.Generic;
using System.Text.Json;

namespace Blazor_ApexGantt.Models;

// Event payloads are deserialized by Blazor's JSInterop, whose default options do not apply a
// string-enum converter. To stay deserialization-safe these read models use primitive/string
// fields (never the input-side C# enums) and a JsonExtensionData bag for anything unmapped.

/// <summary>Read-only view of a task as it appears in event payloads (the core's computed Task).</summary>
public class GanttTaskInfo
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? StartTime { get; set; }
    public string? EndTime { get; set; }
    public double? Progress { get; set; }
    /// <summary>"task" or "milestone".</summary>
    public string? Type { get; set; }
    public string? ParentId { get; set; }
    public string? BarBackgroundColor { get; set; }
    public string? RowBackgroundColor { get; set; }
    /// <summary>Nesting depth (0 = root).</summary>
    public int? Level { get; set; }
    /// <summary>Work-breakdown-structure code (e.g. "1.2.1").</summary>
    public string? Wbs { get; set; }
    public bool? ShowSummaryBar { get; set; }
    public bool? Collapsed { get; set; }
    /// <summary>Any fields not modeled above (e.g. predecessors, successors, group metadata).</summary>
    [System.Text.Json.Serialization.JsonExtensionData]
    public Dictionary<string, JsonElement>? AdditionalData { get; set; }
}

/// <summary>A validation error for a specific field.</summary>
public class ValidationError
{
    public string Field { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

/// <summary>One child task shifted as a side effect of dragging its parent.</summary>
public class AffectedChildTask
{
    public string TaskId { get; set; } = string.Empty;
    public string NewStartTime { get; set; } = string.Empty;
    public string NewEndTime { get; set; } = string.Empty;
}

/// <summary>A sort criterion as reported by the sortChange event.</summary>
public class SortCriterionInfo
{
    public string Key { get; set; } = string.Empty;
    /// <summary>"asc" or "desc".</summary>
    public string Direction { get; set; } = string.Empty;
}

/// <summary>Fired when a task update is initiated.</summary>
public class TaskUpdateEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    /// <summary>The partial fields being applied.</summary>
    public GanttTaskInfo? Updates { get; set; }
    public GanttTaskInfo? UpdatedTask { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task update succeeds.</summary>
public class TaskUpdateSuccessEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    public GanttTaskInfo? UpdatedTask { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task update fails.</summary>
public class TaskUpdateErrorEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
    public long Timestamp { get; set; }
}

/// <summary>Fired when task validation fails.</summary>
public class TaskValidationErrorEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    public List<ValidationError> Errors { get; set; } = new();
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task bar is dragged along the timeline.</summary>
public class TaskDraggedEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    public string OldStartTime { get; set; } = string.Empty;
    public string OldEndTime { get; set; } = string.Empty;
    public string NewStartTime { get; set; } = string.Empty;
    public string NewEndTime { get; set; } = string.Empty;
    /// <summary>Days moved; fractional when snapping by hour/minute.</summary>
    public double DaysMoved { get; set; }
    public List<AffectedChildTask> AffectedChildTasks { get; set; } = new();
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task bar is resized.</summary>
public class TaskResizedEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    /// <summary>"left" or "right".</summary>
    public string ResizeHandle { get; set; } = string.Empty;
    public string OldStartTime { get; set; } = string.Empty;
    public string OldEndTime { get; set; } = string.Empty;
    public string NewStartTime { get; set; } = string.Empty;
    public string NewEndTime { get; set; } = string.Empty;
    /// <summary>Change in duration (days); fractional when snapping by hour/minute.</summary>
    public double DurationChange { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired after a task is inserted via AddTask.</summary>
public class TaskAddedEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    public GanttTaskInfo? Task { get; set; }
    public string? ParentId { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired after a task (and its descendants) is deleted.</summary>
public class TaskDeletedEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    public GanttTaskInfo? Task { get; set; }
    public List<string> RemovedDescendantIds { get; set; } = new();
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task is re-parented via MoveTask.</summary>
public class TaskMovedEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    public string? OldParentId { get; set; }
    public string? NewParentId { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task's progress changes.</summary>
public class TaskProgressChangedEventArgs
{
    public string TaskId { get; set; } = string.Empty;
    public double OldProgress { get; set; }
    public double NewProgress { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired when a dependency is added or removed.</summary>
public class DependencyChangeEventArgs
{
    public string FromId { get; set; } = string.Empty;
    public string ToId { get; set; } = string.Empty;
    /// <summary>"FS", "FF", "SF" or "SS".</summary>
    public string Type { get; set; } = string.Empty;
    public double Lag { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired when a dependency arrow's geometry is recomputed.</summary>
public class DependencyArrowUpdateEventArgs
{
    public string FromId { get; set; } = string.Empty;
    public string ToId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public double? Lag { get; set; }
    public string? ChartInstanceId { get; set; }
    public string? ArrowLinkInstanceId { get; set; }
}

/// <summary>Fired when the selected-task set changes.</summary>
public class SelectionChangeEventArgs
{
    public List<GanttTaskInfo> SelectedTasks { get; set; } = new();
    public List<string> SelectedIds { get; set; } = new();
    public long Timestamp { get; set; }
}

/// <summary>Fired when the undo/redo history changes.</summary>
public class HistoryChangeEventArgs
{
    /// <summary>"record", "undo", "redo" or "clear".</summary>
    public string Kind { get; set; } = string.Empty;
    public bool CanUndo { get; set; }
    public bool CanRedo { get; set; }
    public int UndoSize { get; set; }
    public int RedoSize { get; set; }
    public string? TopUndoLabel { get; set; }
    public string? TopRedoLabel { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired when the sort criteria change.</summary>
public class SortChangeEventArgs
{
    public List<SortCriterionInfo> Criteria { get; set; } = new();
    public long Timestamp { get; set; }
}

/// <summary>Fired when the active filter changes.</summary>
public class FilterChangeEventArgs
{
    public bool Active { get; set; }
    public int VisibleCount { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired when grouping changes.</summary>
public class GroupChangeEventArgs
{
    public bool Active { get; set; }
    public string? Field { get; set; }
    public int GroupCount { get; set; }
    public long Timestamp { get; set; }
}

/// <summary>Fired when a task-list column is resized.</summary>
public class ColumnResizeEventArgs
{
    public string Key { get; set; } = string.Empty;
    public int? Width { get; set; }
    public Dictionary<string, int> Widths { get; set; } = new();
    public long Timestamp { get; set; }
}

/// <summary>Fired when task-list columns are reordered.</summary>
public class ColumnReorderEventArgs
{
    public List<string> Order { get; set; } = new();
    public string? MovedKey { get; set; }
    public long Timestamp { get; set; }
}
