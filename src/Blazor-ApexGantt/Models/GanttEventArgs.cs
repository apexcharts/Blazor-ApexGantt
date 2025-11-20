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