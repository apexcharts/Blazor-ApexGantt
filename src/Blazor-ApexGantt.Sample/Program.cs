using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazor_ApexGantt.Sample;
using Blazor_ApexGantt.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// license for the public demo hosted at https://apexcharts.github.io/ (domain-locked, no watermark)
builder.Services.AddApexGantt(options =>
{
    options.LicenseKey = "APEX-eyJpc3N1ZURhdGUiOiIyMDI2LTA3LTA3IiwiZXhwaXJ5RGF0ZSI6IjIwNTAtMDctMDciLCJwbGFuIjoiZW50ZXJwcmlzZSIsImRvbWFpbnMiOlsiYXBleGNoYXJ0cy5naXRodWIuaW8iXX0=";
});

await builder.Build().RunAsync();