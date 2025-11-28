using Blazor_ApexGantt.Configuration;
using Blazor_ApexGantt.Interop;
using Blazor_ApexGantt.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Blazor_ApexGantt.Extensions;

/// <summary>
/// extension methods for registering apexgantt services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// adds apexgantt services to the service collection
    /// </summary>
    public static IServiceCollection AddApexGantt(
        this IServiceCollection services, 
        Action<ApexGanttConfiguration>? configure = null)
    {
        // register configuration
        if (configure != null)
        {
            services.Configure(configure);
        }

        // register services
        services.AddScoped<ApexGanttInterop>();
        services.AddScoped<ApexGanttLicenseService>();

        return services;
    }
}