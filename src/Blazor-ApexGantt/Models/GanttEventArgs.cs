namespace Blazor_ApexGantt.Models;

/// <summary>
/// event data for task update event
/// </summary>
public class TaskUpdateEventArgs
{
    /// <summary>
    /// id of the task being updated
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// updated task data
    /// </summary>
    public GanttTask? UpdatedTask { get; set; }

    /// <summary>
    /// timestamp when the update occurred
    /// </summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// event data for task update success event
/// </summary>
public class TaskUpdateSuccessEventArgs
{
    /// <summary>
    /// id of the task that was updated
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// the successfully updated task
    /// </summary>
    public GanttTask? UpdatedTask { get; set; }

    /// <summary>
    /// timestamp when the update completed
    /// </summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// event data for task update error event
/// </summary>
public class TaskUpdateErrorEventArgs
{
    /// <summary>
    /// id of the task that failed to update
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// error message describing what went wrong
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// timestamp when the error occurred
    /// </summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// validation error for a specific field
/// </summary>
public class ValidationError
{
    /// <summary>
    /// name of the field with validation error
    /// </summary>
    public string Field { get; set; } = string.Empty;

    /// <summary>
    /// validation error message
    /// </summary>
    public string Message { get; set; } = string.Empty;
}

/// <summary>
/// event data for task validation error event
/// </summary>
public class TaskValidationErrorEventArgs
{
    /// <summary>
    /// id of the task with validation errors
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// list of validation errors
    /// </summary>
    public List<ValidationError> Errors { get; set; } = new();

    /// <summary>
    /// timestamp when validation failed
    /// </summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// event data for task dragged event
/// </summary>
public class TaskDraggedEventArgs
{
    /// <summary>
    /// id of the task that was dragged
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// original start time before drag
    /// </summary>
    public string OldStartTime { get; set; } = string.Empty;

    /// <summary>
    /// original end time before drag
    /// </summary>
    public string OldEndTime { get; set; } = string.Empty;

    /// <summary>
    /// new start time after drag
    /// </summary>
    public string NewStartTime { get; set; } = string.Empty;

    /// <summary>
    /// new end time after drag
    /// </summary>
    public string NewEndTime { get; set; } = string.Empty;

    /// <summary>
    /// number of days the task was moved
    /// </summary>
    public int DaysMoved { get; set; }

    /// <summary>
    /// list of child task ids affected by the drag
    /// </summary>
    public List<string> AffectedChildTasks { get; set; } = new();

    /// <summary>
    /// timestamp when the drag occurred
    /// </summary>
    public long Timestamp { get; set; }
}

/// <summary>
/// event data for task resized event
/// </summary>
public class TaskResizedEventArgs
{
    /// <summary>
    /// id of the task that was resized
    /// </summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>
    /// which handle was used for resize ('left' or 'right')
    /// </summary>
    public string ResizeHandle { get; set; } = string.Empty;

    /// <summary>
    /// original start time before resize
    /// </summary>
    public string OldStartTime { get; set; } = string.Empty;

    /// <summary>
    /// original end time before resize
    /// </summary>
    public string OldEndTime { get; set; } = string.Empty;

    /// <summary>
    /// new start time after resize
    /// </summary>
    public string NewStartTime { get; set; } = string.Empty;

    /// <summary>
    /// new end time after resize
    /// </summary>
    public string NewEndTime { get; set; } = string.Empty;

    /// <summary>
    /// change in duration (in days)
    /// </summary>
    public int DurationChange { get; set; }

    /// <summary>
    /// timestamp when the resize occurred
    /// </summary>
    public long Timestamp { get; set; }
}