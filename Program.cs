using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using neverland.aliyun.ddns;
using neverland.aliyun.ddns.Consts;
using neverland.aliyun.ddns.Services;


var builder = Host.CreateDefaultBuilder(args);
builder.ConfigureAppConfiguration((host,config) =>
{
    //config.Sources.Clear();
    IHostEnvironment env = host.HostingEnvironment;
    config.SetBasePath(AppContext.BaseDirectory).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    config.SetBasePath(AppContext.BaseDirectory).AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
    config.AddEnvironmentVariables(prefix: Contracts.VAR_PREFIEX);
    var root=config.Build();
    NeverlandOption options = new();
    root.GetSection(nameof(NeverlandOption)).Bind(options);
});
builder.ConfigureServices((hostContext, services) =>
{
    services.AddOptions<NeverlandOption>().Bind(hostContext.Configuration.GetSection(nameof(NeverlandOption)));
    services.AddHttpClient(Contracts.QUERY_IPIFYADDRESS_NAME, client => client.BaseAddress = new Uri(Contracts.QUERY_IPIFYADDRESS_V64_URL));
    services.AddSingleton<AliyunServer>();
    services.AddSingleton<IPServer>();
    services.AddHostedService<DDNSWorker>();
});

using var app = builder.Build();
await app.RunAsync();


