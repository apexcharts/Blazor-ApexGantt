using System.Collections.Generic;
using Blazor_ApexGantt.Serialization;

namespace Blazor_ApexGantt.Models;

/// <summary>
/// Localization and text-direction options. With none of them set the chart renders in English,
/// left to right, exactly as it did before localization existed.
/// </summary>
public class LocaleOptions
{
    /// <summary>
    /// A first-party locale pack bundled with this package, by name: <c>"ja"</c> (Japanese) is the
    /// one there is. A pack sets a whole language at once: every message, the date names, the
    /// timeline header word order, the read-only date format, the crosshair format and the week
    /// start. It is the base layer: every other <see cref="LocaleOptions"/> property, and
    /// <see cref="CalendarOptions.WeekStart"/> and <see cref="CalendarOptions.FiscalYearStartMonth"/>,
    /// win over the pack's value. The pack file is loaded the first time a chart names it. A name
    /// that is not bundled logs a console warning and is ignored. The <c>ja</c> pack sets no fiscal
    /// year; set <see cref="CalendarOptions.FiscalYearStartMonth"/> (4 for April) for 年度 labels.
    /// Setting it back to null and calling <c>UpdateOptionsAsync()</c> returns a live chart to
    /// English. Requires apexgantt 3.22.0 or later. Default: null (no pack).
    /// </summary>
    public string? Pack { get; set; }

    /// <summary>
    /// BCP 47 language tag such as "ja" or "ja-JP". When one resolves (here, or the pack's) it is
    /// set as the chart root's <c>lang</c>, and it drives the plural rules and number formatting of
    /// message templates and the collation used to sort and group text. A malformed tag such as
    /// "ja_JP" is ignored as if unset. Requires apexgantt 3.22.0 or later. Default: null (the
    /// pack's tag, else none).
    /// </summary>
    public string? Tag { get; set; }

    /// <summary>
    /// Text and layout direction. <see cref="TextDirection.Rtl"/> mirrors the timeline
    /// horizontally (time flows right to left and the task list moves to the right). When it is
    /// not set, the pack's direction applies. Default: null (the pack's, else left to right).
    /// </summary>
    public TextDirection? Direction { get; set; }

    /// <summary>
    /// dayjs pattern for read-only dates: the task list Start, End, Baseline Start and Baseline
    /// End columns and the bar, milestone and baseline tooltips. Editors keep
    /// <see cref="GanttOptions.InputDateFormat"/>, and stored dates are unchanged. Unset, the
    /// pack's format applies (<c>YYYY/MM/DD</c> for <c>ja</c>, <c>YYYY/MM/DD HH:mm</c> when
    /// <see cref="GanttOptions.InputDateFormat"/> has a time), else
    /// <see cref="GanttOptions.InputDateFormat"/>. Requires apexgantt 3.22.0 or later.
    /// Default: null.
    /// </summary>
    public string? DisplayDateFormat { get; set; }

    /// <summary>
    /// Patterns of the default crosshair label. Each wins over the pack's. Requires apexgantt
    /// 3.22.0 or later. Default: null (the built-in patterns).
    /// </summary>
    public GanttCrosshairFormat? CrosshairFormat { get; set; }

    /// <summary>
    /// Overrides for the chart's generated strings, by message key (<c>addTask</c>,
    /// <c>tooltipStartLabel</c>, <c>columnName</c>, and so on). Every key takes a string, the
    /// seven function keys included (<c>editTaskTitle</c>, <c>groupCountLabel</c>,
    /// <c>filterOperatorLabel</c>, <c>summaryAriaLabel</c>, <c>barAriaLabel</c>,
    /// <c>barAriaValueText</c>, <c>progressAriaLabel</c>), as a template such as
    /// <c>"Edit: {name}"</c> with <c>{n, number}</c> and <c>{n, plural, one {...} other {...}}</c>.
    /// Wins over the pack's messages key by key; unset keys keep the pack's text, else English.
    /// Messages are inserted without escaping, so never put untrusted input in them.
    /// Default: null (no overrides).
    /// </summary>
    public Dictionary<string, string>? Messages { get; set; }

    /// <summary>
    /// Timeline header labels per tier and row, in place of the built-in ones. Only the rows you
    /// set change, and each wins over the pack's row. Its <see cref="Models.TimelineHeaderFormat.Fiscal"/>
    /// block applies only while a fiscal year is in use. Requires apexgantt 3.22.0 or later
    /// (3.23.0 for <see cref="Models.TimelineHeaderFormat.Fiscal"/>). Default: null (the built-in labels).
    /// </summary>
    public TimelineHeaderFormat? TimelineHeaderFormat { get; set; }
}

/// <summary>Patterns of the default crosshair label, as dayjs format strings.</summary>
public class GanttCrosshairFormat
{
    /// <summary>
    /// Pattern on day and coarser tiers. Default: null (the pack's, else "ddd MM/DD/YYYY").
    /// </summary>
    public string? Date { get; set; }

    /// <summary>
    /// Pattern on the halfday, hour and minute tiers. Default: null (the pack's, else
    /// "MM/DD HH:mm").
    /// </summary>
    public string? Time { get; set; }
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

/// <summary>
/// Working-calendar options (working days, holidays, non-working-day snapping) and the timeline
/// header's week start and fiscal year. A <see cref="CalendarOptions"/> object turns the working
/// calendar on, even an empty one, with a Monday to Friday week unless
/// <see cref="WorkingWeekdays"/> says otherwise. The exception is one that sets only
/// <see cref="WeekStart"/> and/or <see cref="FiscalYearStartMonth"/>: properties left null are
/// not sent, so the core receives a header-only calendar, which moves where week, quarter and
/// year cells start and leaves every day a working day (no stripes, no snapping). With no
/// <see cref="GanttOptions.Calendar"/> every day is a working day.
/// </summary>
/// <remarks>
/// <see cref="GanttOptions.Calendar"/> is replaced as a whole on an update. Setting it to null and
/// calling <c>UpdateOptionsAsync()</c> removes the calendar from the live chart (calendar years,
/// every day a working day); setting <see cref="FiscalYearStartMonth"/> to null turns the fiscal
/// year off.
/// </remarks>
public class CalendarOptions
{
    /// <summary>
    /// Weekdays that count as working days (0 = Sunday .. 6 = Saturday). Defaults to Monday to
    /// Friday when the calendar is active.
    /// </summary>
    public List<int>? WorkingWeekdays { get; set; }

    /// <summary>
    /// Non-working dates regardless of weekday. Each entry may be a date string or an
    /// object such as <c>{ date, label }</c> for tooltipped entries. Date strings are read with
    /// <see cref="GanttOptions.InputDateFormat"/>, falling back to ISO.
    /// </summary>
    public List<object>? Holidays { get; set; }

    /// <summary>
    /// Render tinted bands over non-working columns (on by default). They draw above the row
    /// backgrounds and under the bars.
    /// </summary>
    public bool? ShowNonWorkingStripes { get; set; }

    /// <summary>
    /// Tooltip for a holiday stripe. A JavaScript function <c>ctx =&gt; string</c> returning
    /// HTML, where <c>ctx.date</c> is the holiday's Date and <c>ctx.label</c> is the label from
    /// the matching <see cref="Holidays"/> entry (undefined for a plain date). It shows on hover.
    /// </summary>
    public JsFunction? HolidayTooltip { get; set; }

    /// <summary>What happens when a drag/resize commit lands on a non-working day.</summary>
    public DragSnapMode? DragSnapMode { get; set; }

    /// <summary>
    /// First day of the week, 0 (Sunday) to 6 (Saturday). Week cells in the timeline header start
    /// on it, and week numbers (the <c>w</c> token, the built-in week labels) follow it. Wins over
    /// the locale pack's week start (Monday for <c>ja</c>). Anything outside 0 to 6 logs a warning
    /// and is ignored. Shapes the header only: a calendar holding nothing but this and
    /// <see cref="FiscalYearStartMonth"/> does not turn the working calendar on. Requires
    /// apexgantt 3.22.0 or later. Default: null (the pack's week start, else Sunday).
    /// </summary>
    public int? WeekStart { get; set; }

    /// <summary>
    /// First month of the fiscal year, 1 (January) to 12 (December). Setting it, 1 included, puts
    /// the timeline header in fiscal-year mode: quarter and year cells start on fiscal boundaries,
    /// the built-in labels read <c>FY2026</c> and <c>#1 Apr-Jun</c> (a fiscal year is named by the
    /// calendar year it starts in), and the <see cref="TimelineHeaderFormat.Fiscal"/> rows apply
    /// (<c>2026年度</c> with the <c>ja</c> pack). Left null, the header shows calendar years and
    /// quarters. Anything outside 1 to 12 logs a warning and is ignored. Shapes the header only: a
    /// calendar holding nothing but this and <see cref="WeekStart"/> does not turn the working
    /// calendar on. Requires apexgantt 3.22.0 or later; before 3.23.0 a start month of 1 gave
    /// calendar labels. Default: null (calendar years, no fiscal labels).
    /// </summary>
    public int? FiscalYearStartMonth { get; set; }
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

    /// <summary>
    /// Tooltip for a dependency arrow. A JavaScript function <c>ctx =&gt; string</c> returning
    /// HTML, where <c>ctx</c> carries <c>fromTask</c>, <c>toTask</c>, <c>type</c> (FS, FF, SS, SF)
    /// and <c>lag</c> (days). Needs a non-zero <see cref="HitWidth"/> so the arrow can be hovered.
    /// </summary>
    public JsFunction? TooltipTemplate { get; set; }
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
