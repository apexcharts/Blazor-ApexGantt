using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Blazor_ApexGantt.Serialization;

namespace Blazor_ApexGantt.Models;

/// <summary>
/// The timeline header rows of a <see cref="TimelineHeaderFormat"/>: one optional
/// <see cref="TimelineHeaderTierFormat"/> per tier. It is also the shape of the
/// <see cref="TimelineHeaderFormat.Fiscal"/> block.
/// </summary>
/// <remarks>
/// Which tiers fill the header's two rows depends on the zoom (<see cref="GanttOptions.PixelsPerDay"/>):
/// years over quarters or months zoomed out, months over weeks at week zoom (about 26 px per day),
/// weeks over days at day zoom (42 px per day or more), days over half days or hours further in, and
/// hours over minutes at the deepest zoom. Set both rows of a tier when the zoom can vary.
/// <c>Minute.Top</c>, <c>HalfDay.Top</c> and <c>Quarter.Top</c> never render, because no finer tier
/// sits under them.
/// </remarks>
public class TimelineHeaderTierRows
{
    /// <summary>Labels of the minute tier. Default: null (the built-in labels).</summary>
    public TimelineHeaderTierFormat? Minute { get; set; }

    /// <summary>Labels of the hour tier. Default: null (the built-in labels).</summary>
    public TimelineHeaderTierFormat? Hour { get; set; }

    /// <summary>Labels of the half-day (AM/PM) tier. Default: null (the built-in labels).</summary>
    [JsonPropertyName("halfday")]
    public TimelineHeaderTierFormat? HalfDay { get; set; }

    /// <summary>Labels of the day tier. Default: null (the built-in labels).</summary>
    public TimelineHeaderTierFormat? Day { get; set; }

    /// <summary>Labels of the week tier. Default: null (the built-in labels).</summary>
    public TimelineHeaderTierFormat? Week { get; set; }

    /// <summary>Labels of the month tier. Default: null (the built-in labels).</summary>
    public TimelineHeaderTierFormat? Month { get; set; }

    /// <summary>Labels of the quarter tier. Default: null (the built-in labels).</summary>
    public TimelineHeaderTierFormat? Quarter { get; set; }

    /// <summary>Labels of the year tier. Default: null (the built-in labels).</summary>
    public TimelineHeaderTierFormat? Year { get; set; }
}

/// <summary>
/// Timeline header labels per tier and row, for <see cref="LocaleOptions.TimelineHeaderFormat"/>.
/// Only the rows you set change, so an empty format renders the built-in labels. Each row you set
/// wins over the locale pack's row for the same tier and position.
/// </summary>
/// <example>
/// <code>
/// TimelineHeaderFormat = new TimelineHeaderFormat
/// {
///     Year = new() { Top = "YYYY年", Sub = "YYYY年" },
///     // a bare Q is the calendar quarter token, so a literal Q is escaped as [Q]
///     Quarter = new() { Sub = new TimelineHeaderLabelCandidate[] { new(96, "第{fq}四半期"), new("{fq}[Q]") } },
///     Fiscal = new() { Year = new() { Top = "{fy}年度", Sub = "{fy}年度" } },
/// }
/// </code>
/// </example>
public sealed class TimelineHeaderFormat : TimelineHeaderTierRows
{
    /// <summary>
    /// Rows used instead of the ones above only while a fiscal year is in use, that is while
    /// <see cref="CalendarOptions.FiscalYearStartMonth"/> is set to a valid month, 1 included.
    /// With no fiscal year in use the block is ignored. While one is, each tier and row takes the
    /// first of: this block, the rows above, the pack's fiscal rows, the pack's rows, then the
    /// built-in fiscal labels (<c>FY2026</c>, <c>#1 Apr-Jun</c>). So a row you set yourself wins
    /// over every row of the pack, fiscal or not. Requires apexgantt 3.23.0 or later.
    /// Default: null (the rows above apply in fiscal-year mode too).
    /// </summary>
    public TimelineHeaderTierRows? Fiscal { get; set; }
}

/// <summary>Label overrides for one tier. A row left null keeps its built-in labels.</summary>
public class TimelineHeaderTierFormat
{
    /// <summary>Label when this tier is the top (coarse) row. Default: null (the built-in labels).</summary>
    public TimelineHeaderLabelFormat? Top { get; set; }

    /// <summary>Label when this tier is the sub (fine) row. Default: null (the built-in labels).</summary>
    public TimelineHeaderLabelFormat? Sub { get; set; }
}

/// <summary>
/// How one tier and row of the timeline header is labelled: a pattern string, a list of
/// width-gated <see cref="TimelineHeaderLabelCandidate"/>s, or a <see cref="JsFunction"/>.
/// Assign a string, a candidate array or list, or a <see cref="JsFunction"/> directly; each
/// converts implicitly.
/// </summary>
/// <remarks>
/// <para>
/// A <b>pattern</b> is a dayjs format string applied to the cell start at every width, with the
/// date locale in force; a label wider than its cell is cut with an ellipsis, and an empty string
/// blanks the row. Besides the dayjs tokens it takes <c>Q</c>, <c>Do</c>, <c>w</c>/<c>ww</c> (week
/// of year, following the week start), <c>W</c>/<c>WW</c> (ISO week) and <c>L</c>/<c>LL</c>.
/// Wrap Latin text in square brackets (<c>"[Wk] w"</c>); CJK text such as 年, 月 and 週 needs none.
/// Three placeholders are filled in first: <c>{end:PATTERN}</c> (the cell's last instant),
/// <c>{fy}</c> (the fiscal year) and <c>{fq}</c> (the fiscal quarter). <c>YYYY</c> and <c>Q</c>
/// stay calendar-based, and since a bare <c>Q</c> is a token, a literal Q after <c>{fq}</c> is
/// written <c>"{fq}[Q]"</c>: <c>"{fq}Q"</c> prints <c>11</c>, <c>22</c>.
/// </para>
/// <para>
/// A <b>candidate list</b> replaces the built-in list of labels for the row. It is sorted
/// widest-first by the chart, so the order you write it in does not matter: the first candidate
/// whose <see cref="TimelineHeaderLabelCandidate.MinWidth"/> is at most the cell width is used,
/// and the narrowest one below that. An empty list keeps the built-in labels.
/// </para>
/// <para>
/// A <b>function</b> is JavaScript source for <c>(date, ctx) =&gt; string</c>, called for every
/// cell on every render and zoom step. <c>ctx</c> carries <c>tier</c>, <c>position</c>,
/// <c>cellWidth</c>, <c>end</c>, <c>week</c>, <c>quarter</c>, <c>fiscalYear</c>,
/// <c>fiscalQuarter</c>, <c>fiscal</c>, <c>locale</c> and <c>format(pattern, date?)</c>. It runs
/// in the browser's global scope (see <see cref="JsFunction"/>). A function that throws, or returns
/// anything but a string or a number, leaves the cell with its built-in label.
/// </para>
/// <para>Labels are HTML-escaped before they render.</para>
/// </remarks>
[JsonConverter(typeof(TimelineHeaderLabelFormatConverter))]
public sealed class TimelineHeaderLabelFormat
{
    private TimelineHeaderLabelFormat(string? pattern, IReadOnlyList<TimelineHeaderLabelCandidate>? candidates, JsFunction? function)
    {
        Pattern = pattern;
        Candidates = candidates;
        Function = function;
    }

    /// <summary>The pattern string, when this label is a pattern; otherwise null.</summary>
    public string? Pattern { get; }

    /// <summary>The width-gated candidates, when this label is a candidate list; otherwise null.</summary>
    public IReadOnlyList<TimelineHeaderLabelCandidate>? Candidates { get; }

    /// <summary>The label function, when this label is a function; otherwise null.</summary>
    public JsFunction? Function { get; }

    /// <summary>A dayjs pattern used at every cell width.</summary>
    /// <param name="pattern">The pattern; an empty string blanks the row.</param>
    public static TimelineHeaderLabelFormat FromPattern(string pattern)
        => new(pattern ?? throw new ArgumentNullException(nameof(pattern)), null, null);

    /// <summary>A list of width-gated candidates, in any order.</summary>
    /// <param name="candidates">The candidates; an empty list keeps the built-in labels.</param>
    public static TimelineHeaderLabelFormat FromCandidates(IEnumerable<TimelineHeaderLabelCandidate> candidates)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        var list = candidates.ToList();
        if (list.Any(c => c is null))
        {
            throw new ArgumentException("A candidate list cannot contain null.", nameof(candidates));
        }
        return new(null, list.AsReadOnly(), null);
    }

    /// <summary>A list of width-gated candidates, in any order.</summary>
    /// <param name="candidates">The candidates; an empty list keeps the built-in labels.</param>
    public static TimelineHeaderLabelFormat FromCandidates(params TimelineHeaderLabelCandidate[] candidates)
        => FromCandidates((IEnumerable<TimelineHeaderLabelCandidate>)candidates);

    /// <summary>A JavaScript label function, <c>(date, ctx) =&gt; string</c>.</summary>
    /// <param name="function">The function source.</param>
    public static TimelineHeaderLabelFormat FromFunction(JsFunction function)
        => new(null, null, function ?? throw new ArgumentNullException(nameof(function)));

    /// <summary>A dayjs pattern used at every cell width. A null string converts to null.</summary>
    [return: NotNullIfNotNull(nameof(pattern))]
    public static implicit operator TimelineHeaderLabelFormat?(string? pattern)
        => pattern is null ? null : FromPattern(pattern);

    /// <summary>A list of width-gated candidates. A null array converts to null.</summary>
    [return: NotNullIfNotNull(nameof(candidates))]
    public static implicit operator TimelineHeaderLabelFormat?(TimelineHeaderLabelCandidate[]? candidates)
        => candidates is null ? null : FromCandidates(candidates);

    /// <summary>A list of width-gated candidates. A null list converts to null.</summary>
    [return: NotNullIfNotNull(nameof(candidates))]
    public static implicit operator TimelineHeaderLabelFormat?(List<TimelineHeaderLabelCandidate>? candidates)
        => candidates is null ? null : FromCandidates(candidates);

    /// <summary>A JavaScript label function. A null function converts to null.</summary>
    [return: NotNullIfNotNull(nameof(function))]
    public static implicit operator TimelineHeaderLabelFormat?(JsFunction? function)
        => function is null ? null : FromFunction(function);

    /// <inheritdoc />
    public override string ToString()
        => Pattern ?? Function?.Source ?? $"[{Candidates?.Count ?? 0} candidates]";
}

/// <summary>
/// One width-gated label in a candidate list (see <see cref="TimelineHeaderLabelFormat"/>).
/// </summary>
public sealed class TimelineHeaderLabelCandidate
{
    private TimelineHeaderLabelFormat _format = TimelineHeaderLabelFormat.FromPattern(string.Empty);

    /// <summary>Creates an empty candidate; set <see cref="Format"/>.</summary>
    public TimelineHeaderLabelCandidate()
    {
    }

    /// <summary>Creates a candidate used at every width the wider candidates do not cover.</summary>
    /// <param name="format">A pattern string or a <see cref="JsFunction"/>.</param>
    public TimelineHeaderLabelCandidate(TimelineHeaderLabelFormat format)
    {
        Format = format;
    }

    /// <summary>Creates a candidate used from <paramref name="minWidth"/> px up.</summary>
    /// <param name="minWidth">Narrowest cell width (px) the candidate is used at.</param>
    /// <param name="format">A pattern string or a <see cref="JsFunction"/>.</param>
    public TimelineHeaderLabelCandidate(double minWidth, TimelineHeaderLabelFormat format)
    {
        MinWidth = minWidth;
        Format = format;
    }

    /// <summary>Narrowest cell width (px) this candidate is used at. Default: null (0).</summary>
    public double? MinWidth { get; set; }

    /// <summary>
    /// The label: a pattern string or a <see cref="JsFunction"/>, as in
    /// <see cref="TimelineHeaderLabelFormat"/>. A candidate list cannot be nested here.
    /// </summary>
    /// <exception cref="ArgumentException">The value is a candidate list.</exception>
    public TimelineHeaderLabelFormat Format
    {
        get => _format;
        set
        {
            ArgumentNullException.ThrowIfNull(value);
            if (value.Candidates is not null)
            {
                throw new ArgumentException("A candidate's Format is a pattern string or a JsFunction, not another candidate list.", nameof(value));
            }
            _format = value;
        }
    }
}

/// <summary>
/// Writes a <see cref="TimelineHeaderLabelFormat"/> in the form it holds: a string, an array of
/// <c>{ minWidth, format }</c> objects, or a <see cref="JsFunction"/> marker the interop bridge
/// revives. Serialize-only: header formats are sent to the chart and never read back.
/// </summary>
public sealed class TimelineHeaderLabelFormatConverter : JsonConverter<TimelineHeaderLabelFormat>
{
    /// <summary>Not supported: header formats are only ever serialized.</summary>
    /// <exception cref="NotSupportedException">Always.</exception>
    public override TimelineHeaderLabelFormat? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => throw new NotSupportedException($"{nameof(TimelineHeaderLabelFormat)} is serialize-only.");

    /// <inheritdoc />
    public override void Write(Utf8JsonWriter writer, TimelineHeaderLabelFormat value, JsonSerializerOptions options)
    {
        if (value.Pattern is not null)
        {
            writer.WriteStringValue(value.Pattern);
        }
        else if (value.Function is not null)
        {
            JsonSerializer.Serialize(writer, value.Function, options);
        }
        else
        {
            writer.WriteStartArray();
            foreach (var candidate in value.Candidates!)
            {
                writer.WriteStartObject();
                if (candidate.MinWidth is double minWidth)
                {
                    writer.WriteNumber("minWidth", minWidth);
                }
                writer.WritePropertyName("format");
                Write(writer, candidate.Format, options);
                writer.WriteEndObject();
            }
            writer.WriteEndArray();
        }
    }
}
