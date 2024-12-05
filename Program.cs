using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using neverland.aliyun.ddns.Consts;
using neverland.aliyun.ddns.Services;

var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration((config) =>
{
    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables(prefix: Contracts.VAR_PREFIEX);
});
builder.ConfigureServices((hostContext, services) =>
{
    services.AddSingleton<AliyunServer>();
    services.AddSingleton<IPServer>();
    services.AddHostedService<DDNSWorker>();
});

using var app = builder.Build();
await app.RunAsync();


