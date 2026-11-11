using Blazor_ApexGantt.Configuration;
using Blazor_ApexGantt.Interop;
using Microsoft.Extensions.Options;

namespace Blazor_ApexGantt.Services;

/// <summary>
/// service to handle apexgantt license initialization
/// </summary>
public class ApexGanttLicenseService
{
    private readonly ApexGanttInterop _interop;
    private readonly ApexGanttConfiguration _configuration;
    private bool _licenseInitialized;

    /// <summary>
    /// creates the service; the key in <see cref="ApexGanttConfiguration.LicenseKey"/> is applied
    /// on the first call to <see cref="InitializeLicenseAsync"/>
    /// </summary>
    /// <param name="interop">the interop used to pass the key to the core</param>
    /// <param name="configuration">the configuration that carries the licence key</param>
    public ApexGanttLicenseService(
        ApexGanttInterop interop, 
        IOptions<ApexGanttConfiguration> configuration)
    {
        _interop = interop;
        _configuration = configuration.Value;
    }

    /// <summary>
    /// initialize the license if configured
    /// </summary>
    public async Task<bool> InitializeLicenseAsync()
    {
        if (_licenseInitialized)
        {
            return true;
        }

        if (!string.IsNullOrWhiteSpace(_configuration.LicenseKey))
        {
            _licenseInitialized = await _interop.SetLicenseAsync(_configuration.LicenseKey);
            return _licenseInitialized;
        }

        // no license configured, proceed without it
        return true;
    }
}