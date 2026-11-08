namespace Blazor_ApexGantt.Configuration;

/// <summary>
/// configuration options for apexgantt library
/// </summary>
public class ApexGanttConfiguration
{
    /// <summary>
    /// license key for apexgantt
    /// </summary>
    public string? LicenseKey { get; set; }

    /// <summary>
    /// load the Ignite UI add-on (_content/Blazor-ApexGantt/js/apexgantt.ignite-ui.es.min.js)
    /// before the first chart renders, so charts follow the Ignite UI theme on the page (its
    /// --ig-* tokens) as the lowest-ranked layer above the built-in defaults. the add-on is free
    /// and needs no license key. GanttOptions.DesignSystem = false turns it off for one chart.
    /// </summary>
    public bool IgniteUI { get; set; }
}