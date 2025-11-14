namespace Blazor_ApexGantt.Models;

/// <summary>
/// represents a task in the gantt chart
/// </summary>
public class GanttTask
{
    /// <summary>
    /// unique identifier for the task
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// task name/title
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// task start time (MM-DD-YYYY format or DateTime)
    /// </summary>
    public object? StartTime { get; set; }

    /// <summary>
    /// task end time (MM-DD-YYYY format or DateTime)
    /// </summary>
    public object? EndTime { get; set; }

    /// <summary>
    /// parent task id for subtasks
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// task progress (0-100)
    /// </summary>
    public double? Progress { get; set; }

    /// <summary>
    /// custom data for the task
    /// </summary>
    public Dictionary<string, object>? CustomData { get; set; }
}