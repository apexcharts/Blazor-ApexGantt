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

The ApexGantt core library (v3.15.0) is bundled with this package and loaded on demand
as an ES module, so you do not need to add any `<script>` tag or CDN reference. Just
install the NuGet package and register the services (see Setup below).

### 2. Add Using Directives

In your component or `_Imports.razor`:

```csharp
@using Blazor_ApexGantt.Components
@using Blazor_ApexGantt.Models
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
            Color = "#3b82f6"
        },
        new GanttTask
        {
            Id = "task2",
            Name = "Development",
            StartTime = DateTime.Now.AddDays(7),
            EndTime = DateTime.Now.AddDays(21),
            Progress = 45,
            Color = "#10b981",
            Dependency = "task1"
        }
    };
}
```

## License Configuration

If you have a commercial license for ApexGantt, you need to configure the license key during application startup.

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

### Without License (Free Version)

If you don't have a license key, simply register the services without configuration:

```csharp
using Blazor_ApexGantt.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddApexGantt();

await builder.Build().RunAsync();
```

> **Note:** The license will be automatically initialized before any charts are rendered. You only need to configure it once during application startup.

## Configuration

### GanttOptions

Configure your Gantt chart with comprehensive options:

```csharp
var options = new GanttOptions
{
    // Basic Settings
    Title = "Project Timeline",
    Width = "100%",
    Height = "600px",
    ViewMode = "week", // day, week, month, quarter, year
    TasksContainerWidth = 500,

    // Visual Styling
    BarBackgroundColor = "#3b82f6",
    BarTextColor = "#ffffff",
    BarBorderRadius = 4,
    BarMargin = 10,
    RowHeight = 40,
    RowBackgroundColors = new List<string> { "#f9fafb", "#ffffff" },

    // Interactivity
    EnableToolbar = true,
    EnableTaskDrag = true,
    EnableTaskResize = true,
    EnableTaskEdit = true,
    EnableExport = true,
    EnableResize = true,

    // Dependencies
    ArrowColor = "#94a3b8",

    // Custom Options (for advanced ApexGantt features)
    CustomOptions = new Dictionary<string, object>
    {
        { "customProperty", "customValue" }
    }
};
```

### GanttTask

Define tasks with rich properties:

```csharp
var task = new GanttTask
{
    Id = "unique-id",              // Required: Unique identifier
    Name = "Task Name",             // Required: Display name
    StartTime = DateTime.Now,       // Required: Start date/time
    EndTime = DateTime.Now.AddDays(5), // Required: End date/time
    Progress = 75,                  // Optional: 0-100 percentage
    ParentId = "parent-task-id",    // Optional: For hierarchical tasks
    Dependency = "dependent-task-id", // Optional: Task this depends on
    Color = "#3b82f6",              // Optional: Custom bar color
    ClassName = "custom-class",     // Optional: Custom CSS class
    CustomData = new Dictionary<string, object> // Optional: Additional data
    {
        { "priority", "high" },
        { "assignee", "John Doe" }
    }
};
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
        ViewMode = "day"
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
        ViewMode = "week",
        EnableToolbar = true,
        EnableTaskDrag = true,
        EnableTaskResize = true,
        EnableTaskEdit = true,
        EnableExport = true,
        ArrowColor = "#6366f1"
    };

    private List<GanttTask> interactiveTasks = new()
    {
        new() { Id = "1", Name = "Requirements", StartTime = DateTime.Now,
                EndTime = DateTime.Now.AddDays(3), Progress = 100, Color = "#10b981" },
        new() { Id = "2", Name = "Design", StartTime = DateTime.Now.AddDays(3),
                EndTime = DateTime.Now.AddDays(7), Progress = 80, Color = "#3b82f6",
                Dependency = "1" },
        new() { Id = "3", Name = "Implementation", StartTime = DateTime.Now.AddDays(7),
                EndTime = DateTime.Now.AddDays(21), Progress = 40, Color = "#f59e0b",
                Dependency = "2" },
        new() { Id = "4", Name = "QA Testing", StartTime = DateTime.Now.AddDays(21),
                EndTime = DateTime.Now.AddDays(28), Progress = 0, Color = "#ef4444",
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
        ViewMode = "month",
        BarBackgroundColor = "#1e293b",
        BarTextColor = "#f8fafc",
        BarBorderRadius = 8,
        BarMargin = 15,
        RowHeight = 50,
        RowBackgroundColors = new List<string> { "#f1f5f9", "#ffffff" }
    };

    private List<GanttTask> styledTasks = new()
    {
        new() { Id = "1", Name = "Q1 Planning", StartTime = new DateTime(2025, 1, 1),
                EndTime = new DateTime(2025, 3, 31), Progress = 100, Color = "#8b5cf6" },
        new() { Id = "2", Name = "Q2 Execution", StartTime = new DateTime(2025, 4, 1),
                EndTime = new DateTime(2025, 6, 30), Progress = 50, Color = "#ec4899" },
        new() { Id = "3", Name = "Q3 Review", StartTime = new DateTime(2025, 7, 1),
                EndTime = new DateTime(2025, 9, 30), Progress = 0, Color = "#14b8a6" }
    };
}
```

## Demo Application

The repository includes a comprehensive sample application showcasing all features:

- **Basic Demo** - Simple Gantt chart with minimal configuration
- **Styling Demo** - Custom colors, borders, and row styling
- **Interactive Demo** - Drag, resize, edit, and export capabilities
- **Dependencies Demo** - Task dependency arrows and relationships
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
│   │       ├── apexgantt.min.js       # ApexGantt library
│   │       └── blazor-apexgantt.js    # Bridge script
│   │
│   └── Blazor-ApexGantt.Sample/       # Demo application
│       └── Pages/                      # Demo pages
├── LICENSE
├── README.md
└── Blazor-ApexGantt.sln
```

## Requirements

- .NET 9.0 SDK or later
- Blazor WebAssembly or Blazor Server application
- Modern web browser with JavaScript enabled
- ApexGantt JavaScript library (loaded via CDN or local)

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

See the [LICENSE](https://github.com/apexcharts/Blazor-ApexGantt/blob/main/LICENSE) file for details.

## Acknowledgments

- Built on top of [ApexGantt](https://apexcharts.com/apexgantt) JavaScript library
- Inspired by the Blazor community's component ecosystem

## Support

- 📫 Report issues on [GitHub Issues](https://github.com/apexcharts/Blazor-ApexGantt/issues)
- 💬 Join discussions on [GitHub Discussions](https://github.com/apexcharts/Blazor-ApexGantt/discussions)
- 📖 View the [ApexGantt documentation](https://apexcharts.com/apexgantt) for underlying library features

## Roadmap

- [ ] Additional event callbacks (OnTaskClick, OnTaskDrag, etc.)
- [ ] Real-time data updates
- [ ] Advanced filtering and grouping
- [ ] Export to PDF/PNG functionality
- [ ] Localization support
- [ ] Performance optimizations for large datasets

---

Made with ❤️ for the Blazor community
