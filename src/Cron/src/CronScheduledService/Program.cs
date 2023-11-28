using CronScheduledService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSerilog((provider, configuration) =>
{
    configuration.WriteTo.Console();
});
builder.Services.AddHostedService<CronDemoService>();
IHost app = builder.Build();
await app.RunAsync();
