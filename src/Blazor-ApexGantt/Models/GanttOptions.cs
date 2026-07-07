using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Blazor_ApexGantt.Models;

/// <summary>
/// Configuration options for the gantt chart. Mirrors the high-value surface of the core
/// <c>GanttUserOptions</c> (apexgantt 3.15). Anything not modeled here can be supplied via
/// <see cref="AdditionalOptions"/>, including function-typed options (renderers, hooks,
/// formatters) which must be passed as raw values through that escape hatch.
/// </summary>
public class GanttOptions
{
    // --- General ------------------------------------------------------------

    /// <summary>Color theme preset (light or dark).</summary>
    public ThemeMode? Theme { get; set; }

    /// <summary>Localization and text-direction options.</summary>
    public LocaleOptions? Locale { get; set; }

    /// <summary>Chart height (pixels as a number, or a CSS string such as "600px").</summary>
    public object? Height { get; set; }

    /// <summary>Chart width (pixels as a number, or a CSS string such as "100%").</summary>
    public object? Width { get; set; }

    /// <summary>
    /// Zoom level in pixels per day. Replaces the old view-mode presets: roughly 40 = week,
    /// 12 = month, 4 = quarter. When omitted, the chart auto-fits the data span.
    /// </summary>
    public double? PixelsPerDay { get; set; }

    /// <summary>Height of each task row (px).</summary>
    public int? RowHeight { get; set; }

    /// <summary>Width of the task-list panel (px).</summary>
    public int? TasksContainerWidth { get; set; }

    /// <summary>Date format used to parse task date strings (dayjs tokens, default "YYYY-MM-DD").</summary>
    public string? InputDateFormat { get; set; }

    /// <summary>Snap unit for drag/resize commits (day, hour, minute).</summary>
    public SnapUnit? SnapUnit { get; set; }

    /// <summary>Number of snap units to snap to.</summary>
    public double? SnapValue { get; set; }

    /// <summary>Accessible label for the task-list region.</summary>
    public string? TaskListAriaLabel { get; set; }

    // --- Colors / appearance ------------------------------------------------

    /// <summary>Chart background color.</summary>
    public string? BackgroundColor { get; set; }

    /// <summary>General border color.</summary>
    public string? BorderColor { get; set; }

    /// <summary>Custom CSS applied to the canvas.</summary>
    public string? CanvasStyle { get; set; }

    /// <summary>Timeline header background color.</summary>
    public string? HeaderBackground { get; set; }

    /// <summary>Default text color.</summary>
    public string? FontColor { get; set; }

    /// <summary>Default font family.</summary>
    public string? FontFamily { get; set; }

    /// <summary>Default font size (CSS string).</summary>
    public string? FontSize { get; set; }

    /// <summary>Default font weight (CSS string).</summary>
    public string? FontWeight { get; set; }

    /// <summary>Default background color for task bars.</summary>
    public string? BarBackgroundColor { get; set; }

    /// <summary>Text color for task bars.</summary>
    public string? BarTextColor { get; set; }

    /// <summary>Border radius for task bars (CSS string, e.g. "4px").</summary>
    public string? BarBorderRadius { get; set; }

    /// <summary>Top and bottom margin for task bars (px).</summary>
    public int? BarMargin { get; set; }

    /// <summary>Color of summary (group) bars.</summary>
    public string? SummaryBarColor { get; set; }

    /// <summary>Color of milestone diamonds.</summary>
    public string? MilestoneColor { get; set; }

    /// <summary>Alternating row background colors.</summary>
    public List<string>? RowBackgroundColors { get; set; }

    /// <summary>Task-list cell border color.</summary>
    public string? CellBorderColor { get; set; }

    /// <summary>Task-list cell border width (CSS string).</summary>
    public string? CellBorderWidth { get; set; }

    /// <summary>Color of dependency arrows.</summary>
    public string? ArrowColor { get; set; }

    /// <summary>Bar color for tasks on the critical path (requires <see cref="EnableCriticalPath"/>).</summary>
    public string? CriticalBarColor { get; set; }

    /// <summary>Arrow color for dependencies on the critical path.</summary>
    public string? CriticalArrowColor { get; set; }

    /// <summary>Tooltip background color.</summary>
    public string? TooltipBGColor { get; set; }

    /// <summary>Tooltip border color.</summary>
    public string? TooltipBorderColor { get; set; }

    /// <summary>Custom element id used for the tooltip.</summary>
    public string? TooltipId { get; set; }

    // --- Annotations / project boundary ------------------------------------

    /// <summary>Background color for annotation regions.</summary>
    public string? AnnotationBgColor { get; set; }

    /// <summary>Border color for annotations.</summary>
    public string? AnnotationBorderColor { get; set; }

    /// <summary>Dash pattern for annotation borders.</summary>
    public List<double>? AnnotationBorderDashArray { get; set; }

    /// <summary>Border width for annotations.</summary>
    public int? AnnotationBorderWidth { get; set; }

    /// <summary>Default orientation for annotations.</summary>
    public Orientation? AnnotationOrientation { get; set; }

    /// <summary>Annotation lines/regions to draw on the timeline (plain objects matching the core Annotation shape).</summary>
    public List<object>? Annotations { get; set; }

    /// <summary>Draw vertical lines at the project's earliest start and latest end.</summary>
    public bool? EnableProjectBoundary { get; set; }

    /// <summary>Color of the project-boundary lines.</summary>
    public string? ProjectBoundaryColor { get; set; }

    // --- Feature option objects --------------------------------------------

    /// <summary>Bar-label placement and content.</summary>
    public BarLabelOptions? BarLabel { get; set; }

    /// <summary>Baseline (planned-vs-actual) rendering.</summary>
    public BaselineOptions? Baseline { get; set; }

    /// <summary>Working-calendar options (working days, holidays, non-working snapping).</summary>
    public CalendarOptions? Calendar { get; set; }

    /// <summary>Dependency-arrow appearance and interaction.</summary>
    public DependencyOptions? Dependencies { get; set; }

    /// <summary>Undo/redo history options.</summary>
    public HistoryOptions? History { get; set; }

    /// <summary>Quick-filter search-box options.</summary>
    public QuickFilterOptions? QuickFilter { get; set; }

    /// <summary>Task-list column configuration.</summary>
    public List<ColumnListItem>? ColumnConfig { get; set; }

    /// <summary>Initial sort criteria.</summary>
    public List<SortCriterion>? SortBy { get; set; }

    /// <summary>Initial grouping column key.</summary>
    public string? GroupBy { get; set; }

    /// <summary>Initial structured filter rule set (plain object matching the core FilterRuleSet shape).</summary>
    public object? FilterRules { get; set; }

    /// <summary>localStorage state persistence: pass true, or a <see cref="GanttStatePersistenceOptions"/>.</summary>
    public object? PersistState { get; set; }

    /// <summary>Field-mapping configuration when your data uses different key names (plain object).</summary>
    public object? Parsing { get; set; }

    /// <summary>Custom toolbar items (plain objects matching the core ToolbarItem shapes).</summary>
    public List<object>? ToolbarItems { get; set; }

    // --- Feature flags ------------------------------------------------------

    /// <summary>Enable crosshair guide line following the pointer.</summary>
    public bool? EnableCrosshair { get; set; }

    /// <summary>Crosshair line color.</summary>
    public string? CrosshairColor { get; set; }

    /// <summary>Highlight the critical path.</summary>
    public bool? EnableCriticalPath { get; set; }

    /// <summary>Roll up progress/dates from children to summary tasks.</summary>
    public bool? EnableRollups { get; set; }

    /// <summary>Enable inline editing of task-list cells.</summary>
    public bool? EnableInlineEdit { get; set; }

    /// <summary>Enable row selection.</summary>
    public bool? EnableSelection { get; set; }

    /// <summary>Enable the export feature.</summary>
    public bool? EnableExport { get; set; }

    /// <summary>Default export format.</summary>
    public GanttExportFormat? ExportFormat { get; set; }

    /// <summary>Enable resizing of the chart/panel.</summary>
    public bool? EnableResize { get; set; }

    /// <summary>Enable dragging task bars along the timeline.</summary>
    public bool? EnableTaskDrag { get; set; }

    /// <summary>Enable editing task fields.</summary>
    public bool? EnableTaskEdit { get; set; }

    /// <summary>Enable resizing task bars.</summary>
    public bool? EnableTaskResize { get; set; }

    /// <summary>Enable dragging a task's progress handle.</summary>
    public bool? EnableProgressDrag { get; set; }

    /// <summary>Enable keyboard shortcuts for task editing.</summary>
    public bool? EnableTaskEditingShortcuts { get; set; }

    /// <summary>Show the built-in task CRUD toolbar.</summary>
    public bool? EnableTaskCRUDToolbar { get; set; }

    /// <summary>Enable the right-click context menu.</summary>
    public bool? EnableContextMenu { get; set; }

    /// <summary>Show an "add task" row at the bottom of the task list.</summary>
    public bool? EnableAddTaskRow { get; set; }

    /// <summary>Enable drawing a new task by dragging on empty timeline space.</summary>
    public bool? EnableDrawTask { get; set; }

    /// <summary>Show scroll buttons on the timeline.</summary>
    public bool? EnableScrollButtons { get; set; }

    /// <summary>Scroll the timeline to a task when its row is clicked.</summary>
    public bool? ScrollToTaskOnRowClick { get; set; }

    /// <summary>Enable task tooltips.</summary>
    public bool? EnableTooltip { get; set; }

    /// <summary>Show the advanced filter builder.</summary>
    public bool? EnableFilterBuilder { get; set; }

    /// <summary>Show the built-in quick-filter search box.</summary>
    public bool? EnableQuickFilter { get; set; }

    /// <summary>Draw vertical lines between task-list columns.</summary>
    public bool? ColumnLines { get; set; }

    /// <summary>Show the selection checkbox column.</summary>
    public bool? ShowCheckboxColumn { get; set; }

    /// <summary>Auto-size task-list columns to fit their content.</summary>
    public bool? AutoSizeColumns { get; set; }

    /// <summary>Allow resizing columns by dragging their header handles.</summary>
    public bool? ResizableColumns { get; set; }

    /// <summary>Allow reordering columns by dragging their headers.</summary>
    public bool? ReorderableColumns { get; set; }

    // --- Data ---------------------------------------------------------------

    /// <summary>The tasks to render. Serialized as the core's <c>series</c> field.</summary>
    [JsonPropertyName("series")]
    public List<GanttTask>? Tasks { get; set; }

    /// <summary>
    /// Escape hatch for any core option not modeled above (including function-typed options).
    /// Keys must be the exact camelCase names the core expects; values are serialized as-is.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalOptions { get; set; }
}
