using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor_ApexGantt.Sample;
using Blazor_ApexGantt.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddApexGantt();

// configure license if key available
// builder.Services.AddApexGantt(options =>
// {
//     options.LicenseKey = "your-license-key-here";
// });

await builder.Build().RunAsync();