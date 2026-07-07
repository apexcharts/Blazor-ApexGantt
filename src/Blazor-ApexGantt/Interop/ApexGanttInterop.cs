using Microsoft.JSInterop;

namespace Blazor_ApexGantt.Interop;

/// <summary>
/// Handles JavaScript interop for apexgantt. Loads the interop bridge as an ES module
/// (which imports the vendored apexgantt core), so the host app needs no script tag.
/// </summary>
public class ApexGanttInterop : IAsyncDisposable
{
    private const string ModulePath = "./_content/Blazor-ApexGantt/js/blazor-apexgantt.js?ver=3.15.0";

    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    public ApexGanttInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    private async Task<IJSObjectReference> GetModuleAsync()
    {
        return _module ??= await _jsRuntime.InvokeAsync<IJSObjectReference>("import", ModulePath);
    }

    /// <summary>Sets the apexgantt license key.</summary>
    public async Task<bool> SetLicenseAsync(string licenseKey)
    {
        if (string.IsNullOrWhiteSpace(licenseKey)) return false;
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("setLicense", licenseKey);
    }

    /// <summary>Initializes a new gantt chart from a serialized options payload.</summary>
    public async Task<bool> InitAsync<T>(string elementId, string optionsJson, DotNetObjectReference<T>? dotNetRef = null) where T : class
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("init", elementId, optionsJson, dotNetRef);
    }

    /// <summary>Updates an existing gantt chart with a serialized options payload.</summary>
    public async Task UpdateAsync(string elementId, string optionsJson)
    {
        var module = await GetModuleAsync();
        await module.InvokeVoidAsync("update", elementId, optionsJson);
    }

    /// <summary>Destroys a gantt chart and removes its event listeners.</summary>
    public async Task DestroyAsync(string elementId)
    {
        if (_module is null) return;
        await _module.InvokeVoidAsync("destroy", elementId);
    }

    // --- Task CRUD -----------------------------------------------------------
    public async Task AddTaskAsync(string elementId, string taskJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("addTask", elementId, taskJson);

    public async Task UpdateTaskAsync(string elementId, string taskId, string updatesJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("updateTask", elementId, taskId, updatesJson);

    public async Task DeleteTaskAsync(string elementId, string taskId)
        => await (await GetModuleAsync()).InvokeVoidAsync("deleteTask", elementId, taskId);

    public async Task MoveTaskAsync(string elementId, string taskId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("moveTask", elementId, taskId, optionsJson);

    // --- Dependencies --------------------------------------------------------
    public async Task<bool> AddDependencyAsync(string elementId, string fromId, string toId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeAsync<bool>("addDependency", elementId, fromId, toId, optionsJson);

    public async Task<bool> RemoveDependencyAsync(string elementId, string fromId, string toId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("removeDependency", elementId, fromId, toId);

    // --- History -------------------------------------------------------------
    public async Task<bool> UndoAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("undo", elementId);

    public async Task<bool> RedoAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("redo", elementId);

    public async Task<bool> CanUndoAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("canUndo", elementId);

    public async Task<bool> CanRedoAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("canRedo", elementId);

    public async Task ClearHistoryAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearHistory", elementId);

    // --- Sort / group / filter ----------------------------------------------
    public async Task SortAsync(string elementId, string criteriaJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("sort", elementId, criteriaJson);

    public async Task ClearSortAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearSort", elementId);

    public async Task GroupByAsync(string elementId, string criterion)
        => await (await GetModuleAsync()).InvokeVoidAsync("groupBy", elementId, criterion);

    public async Task ClearGroupingAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearGrouping", elementId);

    public async Task SetFilterRulesAsync(string elementId, string? rulesJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("setFilterRules", elementId, rulesJson);

    public async Task ClearFilterAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearFilter", elementId);

    // --- Columns -------------------------------------------------------------
    public async Task SetColumnWidthAsync(string elementId, string key, int width)
        => await (await GetModuleAsync()).InvokeVoidAsync("setColumnWidth", elementId, key, width);

    public async Task ResetColumnWidthsAsync(string elementId, string? key)
        => await (await GetModuleAsync()).InvokeVoidAsync("resetColumnWidths", elementId, key);

    public async Task SetColumnOrderAsync(string elementId, string orderJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("setColumnOrder", elementId, orderJson);

    public async Task<string[]> GetColumnOrderAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<string[]>("getColumnOrder", elementId);

    // --- Selection / navigation / zoom / export ------------------------------
    public async Task<bool> ScrollToTaskAsync(string elementId, string taskId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("scrollToTask", elementId, taskId);

    public async Task<T> GetSelectedTasksAsync<T>(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<T>("getSelectedTasks", elementId);

    public async Task SetSelectedTasksAsync(string elementId, string idsJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("setSelectedTasks", elementId, idsJson);

    public async Task ClearSelectionAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearSelection", elementId);

    public async Task ZoomInAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("zoomIn", elementId);

    public async Task ZoomOutAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("zoomOut", elementId);

    public async Task ExportChartAsync(string elementId, string? format)
        => await (await GetModuleAsync()).InvokeVoidAsync("exportChart", elementId, format);

    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); }
            catch (JSDisconnectedException) { /* circuit already gone */ }
        }
    }
}
