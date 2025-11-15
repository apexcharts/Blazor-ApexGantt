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
    /// view mode (day, week, month, quarter, year)
    /// </summary>
    public string? ViewMode { get; set; }

    /// <summary>
    /// list of tasks to display
    /// </summary>
    public List<GanttTask>? Tasks { get; set; }

    // Visual/Appearance Options

    /// <summary>
    /// custom CSS styles for the canvas
    /// </summary>
    public string? CanvasStyle { get; set; }

    /// <summary>
    /// background color for timeline bars
    /// </summary>
    public string? BarBackgroundColor { get; set; }

    /// <summary>
    /// border radius for timeline bars
    /// </summary>
    public int? BarBorderRadius { get; set; }

    /// <summary>
    /// top and bottom margin for timeline bars
    /// </summary>
    public int? BarMargin { get; set; }

    /// <summary>
    /// text color for timeline bars
    /// </summary>
    public string? BarTextColor { get; set; }

    /// <summary>
    /// array of alternate row background colors
    /// </summary>
    public List<string>? RowBackgroundColors { get; set; }

    // Layout Options

    /// <summary>
    /// height of each task row
    /// </summary>
    public int? RowHeight { get; set; }

    // Interactivity Options

    /// <summary>
    /// enable or disable the toolbar
    /// </summary>
    public bool? EnableToolbar { get; set; }

    /// <summary>
    /// enable task drag functionality
    /// </summary>
    public bool? EnableTaskDrag { get; set; }

    /// <summary>
    /// enable task editing functionality
    /// </summary>
    public bool? EnableTaskEdit { get; set; }

    /// <summary>
    /// enable task resize functionality
    /// </summary>
    public bool? EnableTaskResize { get; set; }

    /// <summary>
    /// enable export functionality
    /// </summary>
    public bool? EnableExport { get; set; }

    /// <summary>
    /// enable resize functionality
    /// </summary>
    public bool? EnableResize { get; set; }

    // Dependency/Arrow Options

    /// <summary>
    /// color for dependency arrows between tasks
    /// </summary>
    public string? ArrowColor { get; set; }

    /// <summary>
    /// additional custom options
    /// </summary>
    public Dictionary<string, object>? CustomOptions { get; set; }
}