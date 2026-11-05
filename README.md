# Blazor-ApexGantt

A comprehensive Blazor wrapper library for [ApexGantt](https://apexcharts.com/apexgantt), enabling developers to create interactive Gantt charts and project timeline visualizations in Blazor applications with full C# integration.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Blazor-ApexGantt.svg)](https://www.nuget.org/packages/Blazor-ApexGantt/)
[![Downloads](https://img.shields.io/nuget/dt/Blazor-ApexGantt.svg)](https://www.nuget.org/packages/Blazor-ApexGantt/)

## Features

- 📊 **Interactive Gantt Charts** - Create beautiful, interactive timeline visualizations
- 🎯 **Type-Safe API** - Full C# models with IntelliSense support
- 🔄 **Task Dependencies** - Visual dependency arrows between related tasks
- ✏️ **Interactive Editing** - Drag, resize, and edit tasks directly in the chart
- 🎨 **Customizable Styling** - Full control over colors, sizes, and visual appearance
- 📱 **Responsive Design** - Automatic width calculation for any container size
- 🔧 **Blazor WebAssembly & Server** - Works with both hosting models
- 🚀 **Modern .NET** - Built on .NET 9.0 with nullable reference types

## Installation

### NuGet Package

```bash
dotnet add package Blazor-ApexGantt
```

### Manual Installation

1. Clone the repository:

```bash
git clone https://github.com/apexcharts/Blazor-ApexGantt.git
```

2. Add a project reference to your Blazor application:

```xml
<ProjectReference Include="..\Blazor-ApexGantt\Blazor-ApexGantt.csproj" />
```

## Quick Start

### 1. No script tag needed

The ApexGantt core library (apexgantt 3.20.0) is bundled with this package and loaded on
demand as an ES module, so you do not need to add any `<script>` tag or CDN reference. Just
install the NuGet package and register the services (see License Configuration below).

### 2. Add Using Directives

In your component or `_Imports.razor`:

```csharp
@using Blazor_ApexGantt.Components
@using Blazor_ApexGantt.Models
@using Blazor_ApexGantt.Serialization   // for JsFunction (custom tooltips)
```

### 3. Create Your First Gantt Chart

```razor
<ApexGantt Options="@options" Tasks="@tasks" />

@code {
    private GanttOptions options = new()
    {
        Width = "100%",
        Height = "600px",
        PixelsPerDay = 40,
        InputDateFormat = "YYYY-MM-DD", // matches how the wrapper writes DateTime values
        EnableTooltip = true
    };

    private List<GanttTask> tasks = new()
    {
        new GanttTask
        {
            Id = "task1",
            Name = "Planning Phase",
            StartTime = DateTime.Now,
            EndTime = DateTime.Now.AddDays(7),
            Progress = 100,
            BarBackgroundColor = "#3b82f6"
        },
        new GanttTask
        {
            Id = "task2",
            Name = "Development",
            StartTime = DateTime.Now.AddDays(7),
            EndTime = DateTime.Now.AddDays(21),
            Progress = 45,
            BarBackgroundColor = "#10b981",
            Dependency = "task1"
        }
    };
}
```

## License Configuration

ApexGantt is a commercial product, included in the Premium plan and above. There is no free tier
and no free or community edition: without a valid license key every chart renders with a trial
watermark, which is for evaluation only. See [LICENSE.md](https://github.com/apexcharts/Blazor-ApexGantt/blob/main/LICENSE.md)
for the terms.

Set your license key once at application start. The component hands it to the core before it
creates each chart, so no chart is drawn with the watermark first.

### Option 1: Direct Configuration in Program.cs

```csharp
using Blazor_ApexGantt.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// configure with license key
builder.Services.AddApexGantt(options =>
{
    options.LicenseKey = "your-license-key";
});

await builder.Build().RunAsync();
```

### Option 2: Using Environment Variables (Production)

For production deployments, use environment variables to keep the license key secure:

```csharp
using Blazor_ApexGantt.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// read from environment variable
builder.Services.AddApexGantt(options =>
{
    options.LicenseKey = builder.Configuration["APEXGANTT_LICENSE_KEY"];
});

await builder.Build().RunAsync();
```

### Evaluating Without a Key

To evaluate ApexGantt before you buy, register the services without a key. Every chart then
renders with the trial watermark:

```csharp
using Blazor_ApexGantt.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddApexGantt();

await builder.Build().RunAsync();
```

> **Note:** The core also checks the key's plan: only a Premium or higher key removes the
> ApexGantt watermark.

## Configuration

### GanttOptions

Configure your Gantt chart with comprehensive options:

```csharp
var options = new GanttOptions
{
    // Basic Settings
    Width = "100%",
    Height = "600px",
    PixelsPerDay = 40,          // zoom: ~4.9 = months, ~25.7 = weeks, 80 = days; omit to auto-fit
    TasksContainerWidth = 500,
    InputDateFormat = "YYYY-MM-DD", // the core default is MM-DD-YYYY; see Dates below

    // Visual Styling
    BarBackgroundColor = "#3b82f6",
    BarTextColor = "#ffffff",
    BarBorderRadius = "4px",
    BarMargin = 10,
    RowHeight = 40,
    RowBackgroundColors = new List<string> { "#f9fafb", "#ffffff" },
    MilestoneColor = "#e11d48",

    // Interactivity
    EnableSelection = true,       // needed by the selection methods and the toolbar Delete button
    EnableTaskCRUDToolbar = true, // Add and Delete buttons in the toolbar
    EnableTaskDrag = true,        // drag bars to new dates (does not reorder rows)
    EnableTaskResize = true,
    EnableTaskEdit = true,        // double-click a bar for the edit dialog
    EnableExport = true,          // toolbar export button, in ExportFormat (SVG unless set)
    EnableResize = true,          // drag the divider to resize the task-list panel

    // Tooltip
    EnableTooltip = true,
    TooltipTemplate = JsFunction.From("task => `<b>${task.name}</b> ${task.progress}%`"),

    // Dependencies
    ArrowColor = "#94a3b8",

    // Any core option not modeled above, keyed by its exact camelCase name
    AdditionalOptions = new Dictionary<string, object>
    {
        { "someCoreOption", "value" }
    }
};
```

### GanttTask

Define tasks with rich properties:

```csharp
var task = new GanttTask
{
    Id = "unique-id",                  // Required: Unique identifier
    Name = "Task Name",                // Required: Display name
    StartTime = DateTime.Now,          // Start date (DateTime, DateOnly or string); a parent may omit both dates
    EndTime = DateTime.Now.AddDays(5), // End date: required for a bar, not needed by a milestone
    Type = TaskType.Task,              // Optional: Task (bar) or Milestone (diamond, needs no EndTime)
    Progress = 75,                     // Optional: 0-100 percentage
    ParentId = "parent-task-id",       // Optional: For hierarchical tasks
    Dependency = "dependent-task-id",  // Optional: Task id, or a TaskDependency for FS/SS/FF/SF + lag
    BarBackgroundColor = "#3b82f6",    // Optional: Custom bar color
    BarTextColor = "#ffffff",          // Optional: Color of the label inside this bar
    RowBackgroundColor = "#f8fafc",    // Optional: Custom row color
    AdditionalData = new()             // Optional: extra fields, readable from a tooltip template
    {
        ["priority"] = JsonSerializer.SerializeToElement("high")
    }
};
```

### Dates

`StartTime` and `EndTime` take a `DateTime`, a `DateOnly` or a string. The wrapper writes
`DateTime` and `DateOnly` values as `yyyy-MM-dd` and sets no date format of its own, while the
core's default `InputDateFormat` is `MM-DD-YYYY`. Bars still draw under that default, because the
core falls back to a lenient parse for ISO dates, but every date the chart writes back after a
drag, resize or edit, including the dates in event args and in `GetTasksAsync()`, uses
`InputDateFormat`, so the chart's data ends up in two formats. So when you pass `DateTime` or
`DateOnly` values, set the format to match:

```csharp
var options = new GanttOptions { InputDateFormat = "YYYY-MM-DD" };
```

The time of day in a `DateTime` is dropped. For hour or minute scheduling, pass strings that include
the time and set a matching format, such as `"YYYY-MM-DD HH:mm"`.

### Milestones

Set `Type = TaskType.Milestone` and the task renders as a diamond on its `StartTime`; it needs no
`EndTime`. This is the clearest form. A childless task with a `StartTime`, no `EndTime` and no
`Type` is drawn as a milestone too (with children it is a summary over them), while an explicit
`Type = TaskType.Task` with no `EndTime` makes the core throw. `MilestoneColor` on `GanttOptions`
sets the diamond color.

```csharp
new GanttTask
{
    Id = "launch",
    Name = "Launch",
    Type = TaskType.Milestone,
    StartTime = new DateTime(2025, 3, 1),
    Dependency = "qa"
}
```

### Parent Tasks and Summary Bars

Any task with children (tasks whose `ParentId` points at it) draws a summary bar spanning its
children's earliest start and latest end; you do not need to set `ShowSummaryBar`. A parent may
omit both dates, as long as a dated task sits somewhere under it (any other row with no start date
is skipped with a warning). If it has dates, the summary bar does not use them, and nothing is
logged. Set `ShowSummaryBar = false`, and give the parent both dates, to draw its own dates as a
normal bar.

### Dependencies

`Dependency` takes a task id (Finish-to-Start, no lag) or a `TaskDependency` with a `Type` (FS,
SS, FF or SF) and a `Lag` in days (negative for a lead). Lag counts working days when a working
calendar is active and calendar days otherwise. Dependencies are drawn as arrows and feed the
critical path and the arrow tooltip; they never move tasks, so dragging a predecessor does not
reschedule its successors. `TaskDependency.LagUnit` is marked `[Obsolete]`: it has no effect, the
core has never read it, and apexgantt 3.20.0 deprecates it.

Any `Calendar` object turns the working calendar on, even an empty one, with a Monday to Friday
week unless you set `WorkingWeekdays`. Without a `Calendar` every day is a working day.

### Zoom

Zoom is continuous: `PixelsPerDay` sets it, there are no view modes, and the header tiers follow
the zoom. Reference points: about 0.5 shows years, 1.6 quarters, 4.9 months, 25.7 weeks and 80
days. The toolbar zoom buttons and `ZoomInAsync`/`ZoomOutAsync` move within roughly 0.25 to 7680.
Leave `PixelsPerDay` unset and the chart fits the whole project on first render.

### Rows, Editing and Export

- Rows sort by start date by default (siblings within each parent). Set `SortBy = new()` to keep
  the order of your task list.
- `EnableTaskDrag` moves bars in time; it does not reorder rows. Drawing new links is
  `Dependencies.Editable`.
- `EnableAddTaskRow` adds an "add task" row that is hidden while more than 50 rows are visible.
- `EnableExport` shows a toolbar export button in `ExportFormat` (SVG unless set);
  `ExportChartAsync` takes SVG, PNG or PDF. PNG and PDF are raster images.
- `EnableProjectBoundary` lines follow drags, resizes, `UpdateTaskAsync` and undo.
- A focused bar moves one snap unit with the left and right arrow keys (`EnableTaskDrag`), and
  Shift with an arrow resizes its end (`EnableTaskResize`, not milestones).

### Custom Tooltips

The tooltip options that take a function in the core take a `JsFunction` here: JavaScript source
written in C#, compiled once in the browser.

| Option | Signature | Shown on |
| --- | --- | --- |
| `GanttOptions.TooltipTemplate` | `(task, dateFormat, context) => string` (`context.durationText` is the Duration cell's text) | hovering a task bar or milestone |
| `DependencyOptions.TooltipTemplate` | `ctx => string` (`fromTask`, `toTask`, `type`, `lag`) | hovering a dependency arrow (needs a non-zero `HitWidth`) |
| `CalendarOptions.HolidayTooltip` | `ctx => string` (`date`, `label`) | hovering a holiday stripe |

```csharp
var options = new GanttOptions
{
    EnableTooltip = true,
    TooltipTemplate = JsFunction.From(@"
        (task) => task.type === 'milestone'
          ? `<b>${task.name}</b><div>Milestone</div>`
          : `<b>${task.name}</b><div>${task.progress}% complete</div><div>Owner: ${task.owner ?? '-'}</div>`"),
    Dependencies = new DependencyOptions
    {
        HitWidth = 10,
        TooltipTemplate = JsFunction.From("ctx => `${ctx.fromTask.name} → ${ctx.toTask.name} (${ctx.type})`")
    }
};
```

`task` is the core's resolved task, so it carries `name`, `startTime`, `endTime`, `progress` and
`type`, plus any field you put in `GanttTask.AdditionalData` (`task.owner` above).

These run synchronously on every hover, which is why they are JavaScript rather than a .NET
callback: Blazor's interop is asynchronous and cannot hand back the HTML in time. A `JsFunction` is
compiled with `new Function`, so it runs in the global scope: it can see `window` and anything your
app put there, and it cannot see your C# variables, so interpolate the values it needs. It is
evaluated as code, so never build one from untrusted input.

A `JsFunction` also works as a value in `AdditionalOptions`, for any other function-typed core
option that has no typed property yet, such as the `before*` hooks:

```csharp
AdditionalOptions = new Dictionary<string, object>
{
    ["beforeTaskUpdate"] = JsFunction.From("({ task, updates }) => !task.locked")
}
```

Hooks run synchronously: return `false` to cancel. A returned Promise does not cancel anything.
Each hook receives one object: `beforeTaskUpdate` gets `{ task, updates }` and `beforeTaskAdd`
gets `{ input, parentId }`. `beforeTaskUpdate` runs for every edit path: drag, resize, progress
drag, the edit dialog, inline edits, the keyboard and `UpdateTaskAsync`.

## Events

Every core event is an `EventCallback` parameter on the component:

```razor
<ApexGantt Options="@options" Tasks="@tasks"
           OnTaskDragged="@OnDragged"
           OnTaskUpdateSuccess="@OnSaved" />
```

| Parameter | Args | Raised when |
| --- | --- | --- |
| `OnTaskUpdate` | `TaskUpdateEventArgs` | a task edit is applied (see below for which edits) |
| `OnTaskUpdateSuccess` | `TaskUpdateSuccessEventArgs` | that edit has been stored |
| `OnTaskUpdateError` | `TaskUpdateErrorEventArgs` | storing an edit failed |
| `OnTaskValidationError` | `TaskValidationErrorEventArgs` | the edit dialog rejects its input |
| `OnTaskDragged` | `TaskDraggedEventArgs` | a bar is dragged to new dates |
| `OnTaskResized` | `TaskResizedEventArgs` | a bar is resized |
| `OnTaskProgressChanged` | `TaskProgressChangedEventArgs` | the progress handle is dragged |
| `OnTaskAdded` | `TaskAddedEventArgs` | a task is added (`AddTaskAsync` or the chart's add controls) |
| `OnTaskDeleted` | `TaskDeletedEventArgs` | a task is deleted |
| `OnTaskMoved` | `TaskMovedEventArgs` | a task is re-parented (`MoveTaskAsync`, indent or outdent) |
| `OnDependencyAdded`, `OnDependencyRemoved` | `DependencyChangeEventArgs` | a dependency is added or removed |
| `OnDependencyArrowUpdate` | `DependencyArrowUpdateEventArgs` | an arrow's geometry is recomputed |
| `OnSelectionChange` | `SelectionChangeEventArgs` | the selection changes (needs `EnableSelection`) |
| `OnHistoryChange` | `HistoryChangeEventArgs` | the undo/redo history changes |
| `OnSortChange`, `OnFilterChange`, `OnGroupChange` | `SortChangeEventArgs`, `FilterChangeEventArgs`, `GroupChangeEventArgs` | sorting, filtering or grouping changes |
| `OnColumnResize`, `OnColumnReorder` | `ColumnResizeEventArgs`, `ColumnReorderEventArgs` | a task-list column is resized or reordered |

Dates in event args are strings in the chart's `InputDateFormat` (see Dates above).

`OnTaskUpdate` and `OnTaskUpdateSuccess` fire once per committed edit on every path, but not for
undo or redo, or for `UpdateTaskAsync(id, updates, silent: true)`. Their `Source` names the path,
one of the `TaskUpdateSource` constants: `Api` (`UpdateTaskAsync`), `Dialog`, `Drag`, `Inline`,
`Keyboard`, `Progress` or `Resize`. `Source` is a string, so a path added by a later core still
arrives.

```csharp
private Task OnSaved(TaskUpdateSuccessEventArgs e) =>
    e.Source == TaskUpdateSource.Api
        ? Task.CompletedTask              // our own UpdateTaskAsync call: already saved
        : api.SaveTaskAsync(e.UpdatedTask!);
```

A drag also raises `OnTaskDragged` (a resize `OnTaskResized`, a progress drag
`OnTaskProgressChanged`), so an app that saves on both saves the edit twice: save in
`OnTaskUpdateSuccess` and read `Source` when the path matters. Keyboard moves and resizes raise
only the update events.

## Methods

Take a reference with `@ref` and call the methods on it:

```razor
<ApexGantt @ref="gantt" Options="@options" Tasks="@tasks" />

@code {
    private ApexGantt? gantt;

    private Task Locate(string id) => gantt!.ScrollToTaskAsync(id);
}
```

| Method | Notes |
| --- | --- |
| `UpdateAsync()` | Re-applies `Options`, and sends `Tasks` only when the task list (or `Parsing`) changed since it was last sent. See Updating the chart below. |
| `UpdateOptionsAsync()` | Re-applies `Options` and never sends the tasks: the chart keeps its data, edits included, and its undo history. |
| `UpdateAsync(replaceTasks)` | `true` always sends the tasks, so the chart reloads your list and drops its own edits and its undo history; `false` is `UpdateOptionsAsync()`. |
| `GetTaskAsync(id)` | A copy of the task as it is now, edits included, as a `GanttTaskInfo`; `null` for an unknown id. |
| `GetTasksAsync()` | Copies of every task in tree order (parents before children), collapsed and filtered-out ones included. |
| `GetDependenciesAsync()` | Every link as it is now, as `DependencyLinkInfo` (`FromId`, `ToId`, `Type`, `Lag`): loaded, added or drawn, less removed ones. |
| `AddTaskAsync(task)` | Raises `OnTaskAdded`. |
| `UpdateTaskAsync(id, updates)` | Sends only the non-null fields of `updates`. Runs `beforeTaskUpdate`, is recorded for undo, and raises `OnTaskUpdate` and `OnTaskUpdateSuccess` with `Source` "api". |
| `UpdateTaskAsync(id, updates, silent)` | With `silent: true` the two update events are not raised: for a rollback or a server echo, so your save handler does not send the change back. Still checked by `beforeTaskUpdate` and recorded for undo. |
| `DeleteTaskAsync(id)` | Throws a `JSException` for a task that has children (no cascade option is passed) or an unknown id. |
| `MoveTaskAsync(id, newParentId)` | Re-parents a task; `null` moves it to the root. |
| `ExpandTaskAsync(id)`, `CollapseTaskAsync(id)` | Open or close a parent. View state only, not recorded for undo. `false` for an unknown id or a task with no children. |
| `AddDependencyAsync(fromId, toId, type, lag)` | `false` when a `beforeDependencyChange` hook vetoes. Throws for an unknown id, a link that already exists, a self-link, a cycle, or (unless `allowSummaryDescendantLinks` is set) a link between a summary task and its descendant. |
| `RemoveDependencyAsync(fromId, toId)` | `false` when a `beforeDependencyChange` hook vetoes. Throws when there is no such link. |
| `UndoAsync()`, `RedoAsync()`, `CanUndoAsync()`, `CanRedoAsync()`, `ClearHistoryAsync()` | Undo/redo history. |
| `SortAsync(criteria)`, `ClearSortAsync()` | `ClearSortAsync` returns rows to the order of your task list. |
| `GroupByAsync(columnKey)`, `ClearGroupingAsync()` | Grouping. |
| `SetFilterRulesAsync(rules)`, `ClearFilterAsync()` | Filtering. |
| `SetColumnWidthAsync`, `ResetColumnWidthsAsync`, `SetColumnOrderAsync`, `GetColumnOrderAsync` | Task-list columns. |
| `ScrollToTaskAsync(id)` | Opens collapsed parents first, then scrolls the row and bar into view. `true` when the row is shown, `false` for an unknown id or a task the active filter hides. |
| `ScrollToTaskAsync(id, expand)` | With `expand: false` collapsed parents stay closed, and a task they hide is not scrolled to (`false`). |
| `GetSelectedTasksAsync()`, `SetSelectedTasksAsync(ids)`, `ClearSelectionAsync()` | Need `EnableSelection = true`; without it they do nothing and the core logs a one-time warning. |
| `ZoomInAsync()`, `ZoomOutAsync()` | Step the zoom; see Zoom above. |
| `ExportChartAsync(format)` | SVG, PNG or PDF (PNG and PDF are raster images). Defaults to `ExportFormat`. |

### Updating the chart

The component never re-sends options by itself: change `Options` (or `Tasks`) and call one of
the update methods. Sending the tasks replaces the chart's data, which drops the edits made in the
chart (drags, dialog and inline edits) that you have not copied into your list, and clears the undo
history. So:

- `UpdateOptionsAsync()` for a theme, zoom, column or other option change. The tasks and `Parsing`
  are left out and the chart keeps everything.
- `UpdateAsync()` when you may also have changed the task list. The tasks go only when they differ
  (by value) from what was last sent, so calling it after an option change keeps the chart's edits.
- `UpdateAsync(replaceTasks: true)` to reload your list as it is, discarding the chart's edits.

```csharp
options.Theme = ThemeMode.Dark;
await gantt!.UpdateOptionsAsync();   // edits and undo history survive
```

### Saving the chart's data

Edits made in the chart are not written back into your `Tasks` list. Read them with
`GetTasksAsync()` and `GetDependenciesAsync()`, and save both: a task's `dependency` (in
`GanttTaskInfo.AdditionalData`) holds only the link it was loaded with, so links added with
`AddDependencyAsync` or drawn in the chart are only in `GetDependenciesAsync()`.

```csharp
var tasks = await gantt!.GetTasksAsync();        // dates in InputDateFormat, Level counts from 1
var links = await gantt.GetDependenciesAsync();  // FromId, ToId, Type ("FS", ...), Lag
```

## Examples

### Basic Chart

A simple Gantt chart with three tasks:

```razor
<ApexGantt Options="@basicOptions" Tasks="@basicTasks" />

@code {
    private GanttOptions basicOptions = new()
    {
        Width = "100%",
        Height = "400px",
        PixelsPerDay = 40,
        InputDateFormat = "YYYY-MM-DD"
    };

    private List<GanttTask> basicTasks = new()
    {
        new() { Id = "1", Name = "Planning", StartTime = new DateTime(2025, 1, 1),
                EndTime = new DateTime(2025, 1, 5), Progress = 100 },
        new() { Id = "2", Name = "Development", StartTime = new DateTime(2025, 1, 6),
                EndTime = new DateTime(2025, 1, 20), Progress = 60 },
        new() { Id = "3", Name = "Testing", StartTime = new DateTime(2025, 1, 21),
                EndTime = new DateTime(2025, 1, 25), Progress = 0 }
    };
}
```

### Interactive Chart with Dependencies

Enable all interactive features and show task dependencies:

```razor
<ApexGantt Options="@interactiveOptions" Tasks="@interactiveTasks" />

@code {
    private GanttOptions interactiveOptions = new()
    {
        Width = "100%",
        Height = "600px",
        PixelsPerDay = 40,
        InputDateFormat = "YYYY-MM-DD",
        EnableSelection = true,
        EnableTaskCRUDToolbar = true,
        EnableTaskDrag = true,
        EnableTaskResize = true,
        EnableTaskEdit = true,
        EnableExport = true,
        ArrowColor = "#6366f1"
    };

    private List<GanttTask> interactiveTasks = new()
    {
        new() { Id = "1", Name = "Requirements", StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(3), Progress = 100, BarBackgroundColor = "#10b981" },
        new() { Id = "2", Name = "Design", StartTime = DateTime.Now.AddDays(3),
                EndTime = DateTime.Now.AddDays(7), Progress = 80, BarBackgroundColor = "#3b82f6",
                Dependency = "1" },
        new() { Id = "3", Name = "Implementation", StartTime = DateTime.Now.AddDays(7),
                EndTime = DateTime.Now.AddDays(21), Progress = 40, BarBackgroundColor = "#f59e0b",
                Dependency = "2" },
        new() { Id = "4", Name = "QA Testing", StartTime = DateTime.Now.AddDays(21),
                EndTime = DateTime.Now.AddDays(28), Progress = 0, BarBackgroundColor = "#ef4444",
                Dependency = "3" }
    };
}
```

### Custom Styling

Apply custom colors and styling:

```razor
<ApexGantt Options="@styledOptions" Tasks="@styledTasks" />

@code {
    private GanttOptions styledOptions = new()
    {
        Width = "100%",
        Height = "500px",
        PixelsPerDay = 4,               // about month level
        InputDateFormat = "YYYY-MM-DD",
        BarBackgroundColor = "#1e293b",
        BarTextColor = "#f8fafc",
        BarBorderRadius = "8px",
        BarMargin = 15,
        RowHeight = 50,
        RowBackgroundColors = new List<string> { "#f1f5f9", "#ffffff" }
    };

    private List<GanttTask> styledTasks = new()
    {
        new() { Id = "1", Name = "Q1 Planning", StartTime = new DateTime(2025, 1, 1),
                EndTime = new DateTime(2025, 3, 31), Progress = 100, BarBackgroundColor = "#8b5cf6" },
        new() { Id = "2", Name = "Q2 Execution", StartTime = new DateTime(2025, 4, 1),
                EndTime = new DateTime(2025, 6, 30), Progress = 50, BarBackgroundColor = "#ec4899" },
        new() { Id = "3", Name = "Q3 Review", StartTime = new DateTime(2025, 7, 1),
                EndTime = new DateTime(2025, 9, 30), Progress = 0, BarBackgroundColor = "#14b8a6" }
    };
}
```

## Demo Application

The repository includes a comprehensive sample application showcasing all features:

- **Basic Demo** - Simple Gantt chart with minimal configuration
- **Styling Demo** - Custom colors, borders, and row styling
- **Interactive Demo** - Drag, resize, edit, and export capabilities
- **Dependencies Demo** - Task dependency arrows and relationships
- **Milestones & Tooltips Demo** - Diamond milestones and custom HTML tooltips
- **Advanced Demo** - Complete project timeline with all features combined

Run the demo:

```bash
cd src/Blazor-ApexGantt.Sample
dotnet run

# Navigate to: https://localhost:5001 (or the port shown in console)
```

Then navigate to the demo pages to see examples in action.

## Project Structure

```
Blazor-ApexGantt/
├── src/
│   ├── Blazor-ApexGantt/              # Main library
│   │   ├── Components/
│   │   │   └── ApexGantt.razor        # Main Gantt component
│   │   ├── Models/
│   │   │   ├── GanttTask.cs           # Task model
│   │   │   └── GanttOptions.cs        # Options model
│   │   ├── Interop/
│   │   │   └── ApexGanttInterop.cs    # JavaScript interop
│   │   └── wwwroot/js/
│   │       ├── apexgantt.es.min.js    # ApexGantt core (ES module, imported by the bridge)
│   │       ├── apexgantt.min.js       # ApexGantt core (UMD build)
│   │       └── blazor-apexgantt.js    # Bridge script
│   │
│   └── Blazor-ApexGantt.Sample/       # Demo application
│       └── Pages/                      # Demo pages
├── LICENSE.md
├── README.md
└── Blazor-ApexGantt.sln
```

## Requirements

- .NET 9.0 SDK or later
- Blazor WebAssembly or Blazor Server application
- Modern web browser with JavaScript enabled
- An ApexGantt license key (Premium plan or above) to remove the trial watermark; the ApexGantt
  core itself is bundled, so no separate script or CDN reference is needed

## Browser Support

Blazor-ApexGantt works on all modern browsers:

- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- Opera (latest)

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

See the [LICENSE.md](https://github.com/apexcharts/Blazor-ApexGantt/blob/main/LICENSE.md) file for details.

## Acknowledgments

- Built on top of [ApexGantt](https://apexcharts.com/apexgantt) JavaScript library
- Inspired by the Blazor community's component ecosystem

## Support

- 📫 Report issues on [GitHub Issues](https://github.com/apexcharts/Blazor-ApexGantt/issues)
- 💬 Join discussions on [GitHub Discussions](https://github.com/apexcharts/Blazor-ApexGantt/discussions)
- 📖 View the [ApexGantt documentation](https://apexcharts.com/apexgantt) for underlying library features

## Roadmap

- [ ] Real-time data updates
- [ ] Performance optimizations for large datasets

---

Made with ❤️ for the Blazor community
