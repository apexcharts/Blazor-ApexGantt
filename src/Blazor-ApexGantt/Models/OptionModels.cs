using System.Collections.Generic;

namespace Blazor_ApexGantt.Models;

/// <summary>Localization and text-direction options.</summary>
public class LocaleOptions
{
    /// <summary>Text/layout direction. 'rtl' mirrors the timeline horizontally.</summary>
    public TextDirection? Direction { get; set; }

    /// <summary>Overrides for individual user-facing strings (partial GanttMessages).</summary>
    public Dictionary<string, string>? Messages { get; set; }
}

/// <summary>Baseline rendering options (planned-vs-actual bars).</summary>
public class BaselineOptions
{
    /// <summary>Render baseline bars below the actual bars.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Baseline color. Defaults to the bar's darkened progress color.</summary>
    public string? Color { get; set; }

    /// <summary>Fill the baseline with diagonal stripes instead of a flat fill.</summary>
    public bool? Striped { get; set; }

    /// <summary>Color of the gaps between stripes.</summary>
    public string? StripeColor { get; set; }

    /// <summary>Width (px) of each stripe band.</summary>
    public int? StripeWidth { get; set; }

    /// <summary>Angle (deg) of the stripes.</summary>
    public int? StripeAngle { get; set; }
}

/// <summary>Working-calendar options: working days, holidays, and non-working-day snapping.</summary>
public class CalendarOptions
{
    /// <summary>Weekdays that count as working days (0 = Sunday .. 6 = Saturday).</summary>
    public List<int>? WorkingWeekdays { get; set; }

    /// <summary>
    /// Non-working dates regardless of weekday. Each entry may be a date string or an
    /// object such as <c>{ date, label }</c> for tooltipped entries.
    /// </summary>
    public List<object>? Holidays { get; set; }

    /// <summary>Render hatched/tinted bands over non-working columns.</summary>
    public bool? ShowNonWorkingStripes { get; set; }

    /// <summary>What happens when a drag/resize commit lands on a non-working day.</summary>
    public DragSnapMode? DragSnapMode { get; set; }
}

/// <summary>Dependency-arrow appearance and interaction options.</summary>
public class DependencyOptions
{
    /// <summary>Pixel radius for rounded joints on arrows (0 = sharp corners).</summary>
    public int? CornerRadius { get; set; }

    /// <summary>Invisible hit-area thickness (px) that captures pointer events on thin arrows.</summary>
    public int? HitWidth { get; set; }

    /// <summary>Enable interactive editing (select/delete arrows and draw new dependencies).</summary>
    public bool? Editable { get; set; }

    /// <summary>Allow linking a summary task to one of its own descendants.</summary>
    public bool? AllowSummaryDescendantLinks { get; set; }
}

/// <summary>Bar-label placement and content options.</summary>
public class BarLabelOptions
{
    /// <summary>Where the label sits relative to the bar.</summary>
    public BarLabelPosition? Position { get; set; }

    /// <summary>Task field whose value is used as the label text.</summary>
    public string? Field { get; set; }

    /// <summary>CSS class applied to the label element.</summary>
    public string? ClassName { get; set; }

    /// <summary>Empty space (px) added to the start of the timeline so left labels are not clipped.</summary>
    public int? LeadingPadding { get; set; }
}

/// <summary>Options for the built-in quick-filter search box.</summary>
public class QuickFilterOptions
{
    /// <summary>Placeholder text for the search input.</summary>
    public string? Placeholder { get; set; }

    /// <summary>Task string fields matched against the query. Defaults to ['name'].</summary>
    public List<string>? Fields { get; set; }

    /// <summary>Match case-sensitively.</summary>
    public bool? CaseSensitive { get; set; }
}

/// <summary>Undo/redo history options.</summary>
public class HistoryOptions
{
    /// <summary>Enable the undo/redo history stack.</summary>
    public bool? Enabled { get; set; }

    /// <summary>Maximum number of entries retained on the stack.</summary>
    public int? MaxSize { get; set; }
}

/// <summary>Crosshair (hover guide line) options.</summary>
public class CrosshairOptions
{
    /// <summary>Enable the vertical crosshair that follows the pointer.</summary>
    public bool? EnableCrosshair { get; set; }

    /// <summary>Crosshair line color.</summary>
    public string? CrosshairColor { get; set; }
}

/// <summary>localStorage state-persistence options.</summary>
public class GanttStatePersistenceOptions
{
    /// <summary>Storage key under which UI state (columns, selection, scroll) is persisted.</summary>
    public string? Key { get; set; }
}

/// <summary>A sort criterion (column + direction) for <c>SortBy</c> and the sort API.</summary>
public class SortCriterion
{
    /// <summary>Column key to sort by (a built-in key such as "startTime" or a custom column key).</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Sort direction. Defaults to ascending when omitted.</summary>
    public SortDirection? Direction { get; set; }
}

/// <summary>A task-list column definition for <c>ColumnConfig</c>.</summary>
public class ColumnListItem
{
    /// <summary>Column identifier (a built-in ColumnKey value such as "name", or a custom key).</summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>Header title.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Minimum column width, e.g. "120px".</summary>
    public string? MinWidth { get; set; }

    /// <summary>Upper bound for auto-sized width, e.g. "240px".</summary>
    public string? MaxWidth { get; set; }

    /// <summary>Flex-grow weight distributing leftover width.</summary>
    public double? FlexGrow { get; set; }

    /// <summary>Whether the column is visible.</summary>
    public bool? Visible { get; set; }

    /// <summary>Whether the column can be resized by dragging its header handle.</summary>
    public bool? Resizable { get; set; }

    /// <summary>Whether the column participates in sorting.</summary>
    public bool? Sortable { get; set; }
}
