using System.Collections.Generic;
using System.Text.Json.Serialization;
using Blazor_ApexGantt.Serialization;

namespace Blazor_ApexGantt.Models;

/// <summary>
/// Configuration options for the gantt chart. Mirrors the high-value surface of the core
/// <c>GanttUserOptions</c> (apexgantt 3.23). Anything not modeled here can be supplied via
/// <see cref="AdditionalOptions"/>. Function-typed options (renderers, hooks, formatters) are
/// written as a <see cref="JsFunction"/>, whether through a typed property such as
/// <see cref="TooltipTemplate"/> or as a value in <see cref="AdditionalOptions"/>.
/// </summary>
public class GanttOptions
{
    // --- General ------------------------------------------------------------

    /// <summary>
    /// Color theme preset (light or dark). Left unset (with no <see cref="BackgroundColor"/>),
    /// the chart follows a design system that declares a dark variant. To use a theme registered
    /// on the family registry by name, set <see cref="ThemeName"/> instead.
    /// </summary>
    [JsonIgnore]
    public ThemeMode? Theme { get; set; }

    /// <summary>
    /// The name of a theme registered on the family-wide registry (with <c>registerTheme</c>
    /// from JavaScript), or "light" or "dark". Takes precedence over <see cref="Theme"/> when
    /// both are set. A name nothing is registered under renders as light. Requires apexgantt
    /// 3.21.0 or later.
    /// </summary>
    [JsonIgnore]
    public string? ThemeName { get; set; }

    /// <summary>The <c>theme</c> value sent to the core: <see cref="ThemeName"/>, else <see cref="Theme"/>.</summary>
    [JsonInclude]
    [JsonPropertyName("theme")]
    internal string? ThemeValue => !string.IsNullOrWhiteSpace(ThemeName)
        ? ThemeName
        : Theme switch
        {
            ThemeMode.Light => "light",
            ThemeMode.Dark => "dark",
            _ => null,
        };

    /// <summary>
    /// Set to false to stop the chart reading the --apx-* family tokens and the page's design
    /// system (including its light or dark variant). Default: true. Requires apexgantt 3.21.0 or
    /// later.
    /// </summary>
    public bool? Tokens { get; set; }

    /// <summary>
    /// Set to false to stop the chart reading the page's design system (the Ignite UI --ig-*
    /// tokens, including its light or dark variant). The --apx-* family tokens still apply.
    /// Has an effect only while the Ignite UI add-on is loaded (see
    /// <see cref="Configuration.ApexGanttConfiguration.IgniteUI"/>). Default: true. Requires
    /// apexgantt 3.21.0 or later.
    /// </summary>
    public bool? DesignSystem { get; set; }

    /// <summary>
    /// Localization and text-direction options: a bundled locale pack by name
    /// (<see cref="LocaleOptions.Pack"/>), message overrides, timeline header formats and more.
    /// </summary>
    public LocaleOptions? Locale { get; set; }

    /// <summary>Chart height (pixels as a number, or a CSS string such as "600px").</summary>
    public object? Height { get; set; }

    /// <summary>Chart width (pixels as a number, or a CSS string such as "100%").</summary>
    public object? Width { get; set; }

    /// <summary>
    /// Zoom level in pixels per day. Zoom is continuous (there are no view modes) and the
    /// timeline header tiers follow it automatically. Reference points: about 0.5 shows years,
    /// 1.6 quarters, 4.9 months, 25.7 weeks and 80 days. The toolbar zoom buttons and
    /// <c>ZoomInAsync</c>/<c>ZoomOutAsync</c> move within roughly 0.25 to 7680. When omitted, the
    /// chart fits the whole data span on first render.
    /// </summary>
    public double? PixelsPerDay { get; set; }

    /// <summary>Height of each task row (px).</summary>
    public int? RowHeight { get; set; }

    /// <summary>Width of the task-list panel (px).</summary>
    public int? TasksContainerWidth { get; set; }

    /// <summary>
    /// Format of task date strings, in dayjs tokens. The core default is "MM-DD-YYYY". Strings are
    /// parsed strictly against this format first, then with a lenient fallback, so ISO strings
    /// still draw under the default. The wrapper writes <see cref="System.DateTime"/> and
    /// <see cref="System.DateOnly"/> task dates as yyyy-MM-dd and sets no format of its own, so
    /// when you pass DateTime values set this to "YYYY-MM-DD": every date the chart writes back
    /// after a drag, resize or edit, including the dates in event args and in
    /// <c>GetTasksAsync</c>, uses this format, so with a mismatched one your data ends up in two
    /// formats.
    /// </summary>
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

    /// <summary>
    /// Custom task tooltip. A JavaScript function <c>(task, dateFormat) =&gt; string</c> that
    /// returns the HTML shown when a task bar or milestone is hovered. <c>task</c> is the core's
    /// resolved task (<c>name</c>, <c>startTime</c>, <c>endTime</c>, <c>progress</c>, <c>type</c>,
    /// and any extra fields from <see cref="GanttTask.AdditionalData"/>); <c>dateFormat</c> is the
    /// chart's <see cref="InputDateFormat"/>. When omitted, the built-in tooltip is used. Has no
    /// effect when <see cref="EnableTooltip"/> is false.
    /// </summary>
    /// <example>
    /// <code>
    /// TooltipTemplate = JsFunction.From("task => `&lt;b&gt;${task.name}&lt;/b&gt;&lt;br&gt;${task.progress}% done`"),
    /// </code>
    /// </example>
    public JsFunction? TooltipTemplate { get; set; }

    // --- Annotations / project boundary ------------------------------------

    /// <summary>Background color for annotation regions.</summary>
    public string? AnnotationBgColor { get; set; }

    /// <summary>Border color for annotations.</summary>
    public string? AnnotationBorderColor { get; set; }

    /// <summary>
    /// Dash pattern for annotation borders. An annotation's own <c>borderDashArray</c> wins.
    /// </summary>
    public List<double>? AnnotationBorderDashArray { get; set; }

    /// <summary>Border width for annotations.</summary>
    public int? AnnotationBorderWidth { get; set; }

    /// <summary>
    /// Default orientation for annotations, the fallback for an annotation label's own
    /// orientation.
    /// </summary>
    public Orientation? AnnotationOrientation { get; set; }

    /// <summary>Annotation lines/regions to draw on the timeline (plain objects matching the core Annotation shape).</summary>
    public List<object>? Annotations { get; set; }

    /// <summary>
    /// Draw vertical lines at the project's earliest start and latest end. They follow drags,
    /// resizes, <c>UpdateTaskAsync</c> and undo.
    /// </summary>
    public bool? EnableProjectBoundary { get; set; }

    /// <summary>Color of the project-boundary lines.</summary>
    public string? ProjectBoundaryColor { get; set; }

    // --- Feature option objects --------------------------------------------

    /// <summary>Bar-label placement and content.</summary>
    public BarLabelOptions? BarLabel { get; set; }

    /// <summary>Baseline (planned-vs-actual) rendering.</summary>
    public BaselineOptions? Baseline { get; set; }

    /// <summary>
    /// Working-calendar options (working days, holidays, non-working snapping) and the timeline
    /// header's week start and fiscal year. A calendar that sets only
    /// <see cref="CalendarOptions.WeekStart"/> and/or <see cref="CalendarOptions.FiscalYearStartMonth"/>
    /// shapes the header and leaves every day a working day; any other calendar, an empty one
    /// included, turns the working calendar on.
    /// </summary>
    public CalendarOptions? Calendar { get; set; }

    /// <summary>Dependency-arrow appearance and interaction.</summary>
    public DependencyOptions? Dependencies { get; set; }

    /// <summary>Undo/redo history options.</summary>
    public HistoryOptions? History { get; set; }

    /// <summary>Quick-filter search-box options.</summary>
    public QuickFilterOptions? QuickFilter { get; set; }

    /// <summary>Task-list column configuration.</summary>
    public List<ColumnListItem>? ColumnConfig { get; set; }

    /// <summary>
    /// Initial sort criteria. When omitted, rows sort by start date (ascending, siblings within
    /// each parent). Pass an empty list to keep the order of your task list.
    /// </summary>
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

    /// <summary>
    /// Draw thin rollup markers under each summary bar at the date ranges of its leaf
    /// descendants, visible even while the parent is collapsed.
    /// </summary>
    public bool? EnableRollups { get; set; }

    /// <summary>
    /// Enable inline editing of task-list cells (double-click a cell). Turned on by
    /// <see cref="EnableTaskEdit"/> unless set to false.
    /// </summary>
    public bool? EnableInlineEdit { get; set; }

    /// <summary>
    /// Enable row selection. The selection methods and the toolbar Delete button do nothing
    /// without it.
    /// </summary>
    public bool? EnableSelection { get; set; }

    /// <summary>Show the export button in the toolbar (on by default).</summary>
    public bool? EnableExport { get; set; }

    /// <summary>
    /// Format produced by the toolbar export button: SVG (the default), PNG or PDF. PNG and PDF
    /// are raster images.
    /// </summary>
    public GanttExportFormat? ExportFormat { get; set; }

    /// <summary>Allow the task-list panel to be resized by dragging the divider.</summary>
    public bool? EnableResize { get; set; }

    /// <summary>
    /// Enable dragging task bars along the timeline to reschedule them. Dragging moves a bar in
    /// time; it does not reorder rows.
    /// </summary>
    public bool? EnableTaskDrag { get; set; }

    /// <summary>
    /// Double-clicking a task bar opens the edit dialog. Also turns on inline cell editing unless
    /// <see cref="EnableInlineEdit"/> is false.
    /// </summary>
    public bool? EnableTaskEdit { get; set; }

    /// <summary>Enable resizing task bars.</summary>
    public bool? EnableTaskResize { get; set; }

    /// <summary>Enable dragging a task's progress handle.</summary>
    public bool? EnableProgressDrag { get; set; }

    /// <summary>Enable keyboard shortcuts for task editing.</summary>
    public bool? EnableTaskEditingShortcuts { get; set; }

    /// <summary>
    /// Show built-in Add and Delete buttons in the toolbar. Delete removes the selected tasks, so
    /// it needs <see cref="EnableSelection"/>.
    /// </summary>
    public bool? EnableTaskCRUDToolbar { get; set; }

    /// <summary>Enable the right-click context menu.</summary>
    public bool? EnableContextMenu { get; set; }

    /// <summary>
    /// Show an "add task" row at the bottom of the task list. The row is hidden while more than
    /// 50 rows are visible (row virtualization is then active); use the toolbar or context menu
    /// to add tasks instead.
    /// </summary>
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
    /// Escape hatch for any core option not modeled above. Keys must be the exact camelCase
    /// names the core expects; values are serialized as-is, and a <see cref="JsFunction"/>
    /// value arrives in the browser as a real function.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object>? AdditionalOptions { get; set; }
}
