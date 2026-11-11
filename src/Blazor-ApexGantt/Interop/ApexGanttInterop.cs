using Microsoft.JSInterop;

namespace Blazor_ApexGantt.Interop;

/// <summary>
/// Handles JavaScript interop for apexgantt. Loads the interop bridge as an ES module
/// (which imports the vendored apexgantt core), so the host app needs no script tag.
/// </summary>
public class ApexGanttInterop : IAsyncDisposable
{
    private const string ModulePath = "./_content/Blazor-ApexGantt/js/blazor-apexgantt.js?ver=3.21.0";

    private readonly IJSRuntime _jsRuntime;
    private IJSObjectReference? _module;

    /// <summary>Creates the interop service; the bridge module is imported on the first call.</summary>
    /// <param name="jsRuntime">The JS runtime used to import and call the bridge module.</param>
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

    /// <summary>
    /// Loads the Ignite UI add-on (at most once); returns false when it cannot be loaded.
    /// </summary>
    public async Task<bool> LoadIgniteUIAsync()
    {
        var module = await GetModuleAsync();
        return await module.InvokeAsync<bool>("loadIgniteUI");
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
    /// <summary>Adds a task (a JSON task object) and re-renders. Recorded in undo history; raises <c>taskAdded</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="taskJson">The task, serialized as a JSON object.</param>
    public async Task AddTaskAsync(string elementId, string taskJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("addTask", elementId, taskJson);

    /// <summary>Applies partial updates to a task, with no update options.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="taskId">Id of the task to update.</param>
    /// <param name="updatesJson">The fields to change, serialized as a JSON object.</param>
    public async Task UpdateTaskAsync(string elementId, string taskId, string updatesJson)
        => await UpdateTaskAsync(elementId, taskId, updatesJson, null);

    /// <summary>Applies partial updates to a task, with the core's <c>UpdateTaskOptions</c> as JSON (or null).</summary>
    public async Task UpdateTaskAsync(string elementId, string taskId, string updatesJson, string? optionsJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("updateTask", elementId, taskId, updatesJson, optionsJson);

    /// <summary>Removes a task with the core's default cascade mode, which throws when the task has children. Raises <c>taskDeleted</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="taskId">Id of the task to remove.</param>
    public async Task DeleteTaskAsync(string elementId, string taskId)
        => await (await GetModuleAsync()).InvokeVoidAsync("deleteTask", elementId, taskId);

    /// <summary>Re-parents a task. Recorded in undo history; raises <c>taskMoved</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="taskId">Id of the task to move.</param>
    /// <param name="optionsJson">The core's move options as JSON (<c>{"newParentId": ...}</c>; a null parent moves the task to the root), or null to move it to the root.</param>
    public async Task MoveTaskAsync(string elementId, string taskId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("moveTask", elementId, taskId, optionsJson);

    // --- Reading the data ----------------------------------------------------
    /// <summary>A copy of one task as it is now, or default for an unknown id.</summary>
    public async Task<T> GetTaskAsync<T>(string elementId, string taskId)
        => await (await GetModuleAsync()).InvokeAsync<T>("getTask", elementId, taskId);

    /// <summary>Copies of every task, in tree order.</summary>
    public async Task<T> GetTasksAsync<T>(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<T>("getTasks", elementId);

    /// <summary>Copies of every dependency link as it is now.</summary>
    public async Task<T> GetDependenciesAsync<T>(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<T>("getDependencies", elementId);

    // --- Expand / collapse ---------------------------------------------------
    /// <summary>Opens a collapsed parent. False for an unknown id or a task with no children.</summary>
    public async Task<bool> ExpandTaskAsync(string elementId, string taskId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("expandTask", elementId, taskId);

    /// <summary>Collapses a parent. False for an unknown id or a task with no children.</summary>
    public async Task<bool> CollapseTaskAsync(string elementId, string taskId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("collapseTask", elementId, taskId);

    // --- Dependencies --------------------------------------------------------
    /// <summary>Adds a dependency link. Recorded in undo history; raises <c>dependencyAdded</c>. The core throws for a self-link, a missing task, a duplicate, a cycle or (unless allowed) a summary-descendant link.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="fromId">Id of the predecessor task.</param>
    /// <param name="toId">Id of the successor task.</param>
    /// <param name="optionsJson">The link's <c>type</c> and <c>lag</c> as JSON, or null for FS with no lag.</param>
    /// <returns>True when the link was added; false when a <c>beforeDependencyChange</c> hook cancelled it.</returns>
    public async Task<bool> AddDependencyAsync(string elementId, string fromId, string toId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeAsync<bool>("addDependency", elementId, fromId, toId, optionsJson);

    /// <summary>Removes a dependency link. Recorded in undo history; raises <c>dependencyRemoved</c>. The core throws when no such link exists.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="fromId">Id of the predecessor task.</param>
    /// <param name="toId">Id of the successor task.</param>
    /// <returns>True when the link was removed; false when a <c>beforeDependencyChange</c> hook cancelled it.</returns>
    public async Task<bool> RemoveDependencyAsync(string elementId, string fromId, string toId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("removeDependency", elementId, fromId, toId);

    // --- History -------------------------------------------------------------
    /// <summary>Undoes the most recent recorded step.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <returns>True when a step was undone; false when the undo stack is empty or history is disabled.</returns>
    public async Task<bool> UndoAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("undo", elementId);

    /// <summary>Redoes the most recently undone step.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <returns>True when a step was redone; false otherwise.</returns>
    public async Task<bool> RedoAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("redo", elementId);

    /// <summary>Whether there is a step to undo.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task<bool> CanUndoAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("canUndo", elementId);

    /// <summary>Whether there is a step to redo.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task<bool> CanRedoAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<bool>("canRedo", elementId);

    /// <summary>Drops every recorded undo and redo step; raises <c>historyChange</c> with kind "clear".</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task ClearHistoryAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearHistory", elementId);

    // --- Sort / group / filter ----------------------------------------------
    /// <summary>Applies a hierarchy-preserving sort; raises <c>sortChange</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="criteriaJson">One sort criterion or an array of them, serialized as JSON.</param>
    public async Task SortAsync(string elementId, string criteriaJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("sort", elementId, criteriaJson);

    /// <summary>Clears the sort and returns to natural (input) order; raises <c>sortChange</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task ClearSortAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearSort", elementId);

    /// <summary>Groups the task list by a column; raises <c>groupChange</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="criterion">Key of the column to group by.</param>
    public async Task GroupByAsync(string elementId, string criterion)
        => await (await GetModuleAsync()).InvokeVoidAsync("groupBy", elementId, criterion);

    /// <summary>Clears grouping and restores the tree view; raises <c>groupChange</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task ClearGroupingAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearGrouping", elementId);

    /// <summary>Applies a structured filter; raises <c>filterChange</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="rulesJson">The core's <c>FilterRuleSet</c> as JSON, or null to clear the filter.</param>
    public async Task SetFilterRulesAsync(string elementId, string? rulesJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("setFilterRules", elementId, rulesJson);

    /// <summary>Clears the filter so every row shows again; raises <c>filterChange</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task ClearFilterAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearFilter", elementId);

    // --- Columns -------------------------------------------------------------
    /// <summary>Pins a task-list column to a pixel width; raises <c>columnResize</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="key">Key of the column.</param>
    /// <param name="width">Width in pixels.</param>
    public async Task SetColumnWidthAsync(string elementId, string key, int width)
        => await (await GetModuleAsync()).InvokeVoidAsync("setColumnWidth", elementId, key, width);

    /// <summary>Returns one column, or every column, to its automatic width; raises <c>columnResize</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="key">Key of the column to reset, or null (or empty) to reset them all.</param>
    public async Task ResetColumnWidthsAsync(string elementId, string? key)
        => await (await GetModuleAsync()).InvokeVoidAsync("resetColumnWidths", elementId, key);

    /// <summary>Sets the left-to-right column order; visible columns left out keep their relative order at the end. Raises <c>columnReorder</c>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="orderJson">Column keys as a JSON array.</param>
    public async Task SetColumnOrderAsync(string elementId, string orderJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("setColumnOrder", elementId, orderJson);

    /// <summary>The current left-to-right column keys.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task<string[]> GetColumnOrderAsync(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<string[]>("getColumnOrder", elementId);

    // --- Selection / navigation / zoom / export ------------------------------
    /// <summary>Scrolls a task into view with the core's default options (collapsed ancestors are expanded first).</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="taskId">Id of the task.</param>
    public async Task<bool> ScrollToTaskAsync(string elementId, string taskId)
        => await ScrollToTaskAsync(elementId, taskId, null);

    /// <summary>Scrolls a task into view, with the core's <c>ScrollToTaskOptions</c> as JSON (or null).</summary>
    public async Task<bool> ScrollToTaskAsync(string elementId, string taskId, string? optionsJson)
        => await (await GetModuleAsync()).InvokeAsync<bool>("scrollToTask", elementId, taskId, optionsJson);

    /// <summary>The selected tasks, deserialized as <typeparamref name="T"/>.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <typeparam name="T">The type to deserialize the task array into.</typeparam>
    public async Task<T> GetSelectedTasksAsync<T>(string elementId)
        => await (await GetModuleAsync()).InvokeAsync<T>("getSelectedTasks", elementId);

    /// <summary>Replaces the selection with the given task ids.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="idsJson">Task ids as a JSON array.</param>
    public async Task SetSelectedTasksAsync(string elementId, string idsJson)
        => await (await GetModuleAsync()).InvokeVoidAsync("setSelectedTasks", elementId, idsJson);

    /// <summary>Clears the selection.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task ClearSelectionAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("clearSelection", elementId);

    /// <summary>Zooms the timeline in by one step, keeping its visible centre in place.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task ZoomInAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("zoomIn", elementId);

    /// <summary>Zooms the timeline out by one step.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    public async Task ZoomOutAsync(string elementId)
        => await (await GetModuleAsync()).InvokeVoidAsync("zoomOut", elementId);

    /// <summary>Exports the chart and triggers a download.</summary>
    /// <param name="elementId">Id of the chart's container element.</param>
    /// <param name="format">"svg", "png" or "pdf"; null (or empty) uses the chart's configured export format.</param>
    public async Task ExportChartAsync(string elementId, string? format)
        => await (await GetModuleAsync()).InvokeVoidAsync("exportChart", elementId, format);

    /// <summary>Releases the imported bridge module, if one was loaded. A disconnected circuit is ignored.</summary>
    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); }
            catch (JSDisconnectedException) { /* circuit already gone */ }
        }
    }
}
