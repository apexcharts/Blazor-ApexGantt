using Microsoft.JSInterop;

namespace Blazor_ApexGantt.Interop;

/// <summary>
/// handles javascript interop for apexgantt
/// </summary>
public class ApexGanttInterop : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private bool _initialized;
    private bool _licenseSet;

    public ApexGanttInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// set the apexgantt license key
    /// </summary>
    public async Task<bool> SetLicenseAsync(string licenseKey)
    {
        if (string.IsNullOrWhiteSpace(licenseKey))
        {
            return false;
        }

        await EnsureInitializedAsync();
        _licenseSet = await _jsRuntime.InvokeAsync<bool>("blazorApexGantt.setLicense", licenseKey);
        return _licenseSet;
    }

    /// <summary>
    /// ensure the javascript module is loaded
    /// </summary>
    private async Task EnsureInitializedAsync()
    {
        if (!_initialized)
        {
            await _jsRuntime.InvokeVoidAsync("eval", 
                "if (!window.blazorApexGanttLoaded) { " +
                "var script = document.createElement('script'); " +
                "script.src = '_content/Blazor-ApexGantt/js/blazor-apexgantt.js'; " +
                "document.head.appendChild(script); " +
                "window.blazorApexGanttLoaded = true; }");
            
            // wait for the blazorApexGantt object to be available
            await WaitForBlazorApexGanttAsync();
            
            _initialized = true;
        }
    }

    /// <summary>
    /// wait for blazorApexGantt to be available in window
    /// </summary>
    private async Task WaitForBlazorApexGanttAsync()
    {
        var maxAttempts = 50; // 5 seconds max
        var attempts = 0;

        while (attempts < maxAttempts)
        {
            var isAvailable = await _jsRuntime.InvokeAsync<bool>("eval", 
                "typeof window.blazorApexGantt !== 'undefined'");

            if (isAvailable)
            {
                return;
            }

            await Task.Delay(100);
            attempts++;
        }

        throw new InvalidOperationException(
            "blazorApexGantt script failed to load after 5 seconds");
    }

    /// <summary>
    /// initialize a new gantt chart
    /// </summary>
    public async Task<bool> InitAsync<T>(string elementId, object options, DotNetObjectReference<T>? dotNetRef = null) where T : class
    {
        await EnsureInitializedAsync();
        return await _jsRuntime.InvokeAsync<bool>("blazorApexGantt.init", elementId, options, dotNetRef);
    }

    /// <summary>
    /// update an existing gantt chart
    /// </summary>
    public async Task<bool> UpdateAsync(string elementId, object options)
    {
        return await _jsRuntime.InvokeAsync<bool>("blazorApexGantt.update", elementId, options);
    }

    /// <summary>
    /// destroy a gantt chart
    /// </summary>
    public async Task<bool> DestroyAsync(string elementId)
    {
        return await _jsRuntime.InvokeAsync<bool>("blazorApexGantt.destroy", elementId);
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}