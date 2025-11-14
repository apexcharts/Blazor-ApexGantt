namespace Blazor_ApexGantt.Models;

/// <summary>
/// configuration options for the gantt chart
/// </summary>
public class GanttOptions
{
    /// <summary>
    /// chart title
    /// </summary>
    public string? Title { get; set; }

    /// <summary>
    /// chart height
    /// </summary>
    public object? Height { get; set; }

    /// <summary>
    /// chart width
    /// </summary>
    public object? Width { get; set; }

    /// <summary>
    /// width of the tasks container area
    /// </summary>
    public int? TasksContainerWidth { get; set; }

    /// <summary>
    /// view mode (day, week, month)
    /// </summary>
    public string? ViewMode { get; set; }

    /// <summary>
    /// list of tasks to display
    /// </summary>
    public List<GanttTask>? Tasks { get; set; }

    /// <summary>
    /// enable export functionality
    /// </summary>
    public bool? EnableExport { get; set; }

    /// <summary>
    /// enable resize functionality
    /// </summary>
    public bool? EnableResize { get; set; }

    /// <summary>
    /// additional custom options
    /// </summary>
    public Dictionary<string, object>? CustomOptions { get; set; }
}