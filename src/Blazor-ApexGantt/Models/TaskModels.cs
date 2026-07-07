namespace Blazor_ApexGantt.Models;

/// <summary>
/// A typed dependency on another task, with optional lag/lead and relationship type.
/// A task's <see cref="GanttTask.Dependency"/> can be a plain id string or one of these.
/// </summary>
public class TaskDependency
{
    /// <summary>Id of the predecessor task this task depends on.</summary>
    public string TaskId { get; set; } = string.Empty;

    /// <summary>Relationship type. Defaults to Finish-to-Start when omitted.</summary>
    public DependencyType? Type { get; set; }

    /// <summary>Lag (positive) or lead (negative) applied to the relationship.</summary>
    public int? Lag { get; set; }

    /// <summary>Unit for <see cref="Lag"/>: working days (calendar-aware) or raw calendar days.</summary>
    public LagUnit? LagUnit { get; set; }
}

/// <summary>Baseline (planned) start/end dates for comparison against actual dates.</summary>
public class BaselineInput
{
    /// <summary>Planned start date string (parsed with the chart's input date format).</summary>
    public string Start { get; set; } = string.Empty;

    /// <summary>Planned end date string (parsed with the chart's input date format).</summary>
    public string End { get; set; } = string.Empty;
}

/// <summary>A person assigned to a task, rendered as an avatar in the assignees column.</summary>
public class Assignee
{
    /// <summary>Display name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Optional avatar image URL.</summary>
    public string? AvatarUrl { get; set; }

    /// <summary>Optional initials shown when no avatar image is provided.</summary>
    public string? Initials { get; set; }

    /// <summary>Optional avatar background color.</summary>
    public string? Color { get; set; }
}

/// <summary>A working segment of a split task, rendered as a separate bar piece.</summary>
public class TaskSegment
{
    /// <summary>Segment start date string.</summary>
    public string Start { get; set; } = string.Empty;

    /// <summary>Segment end date string.</summary>
    public string End { get; set; } = string.Empty;
}
