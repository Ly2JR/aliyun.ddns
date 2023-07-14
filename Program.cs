using neverland.aliyun.ddns;
using System.Diagnostics;

//默认值
var key = Contracts.DEBUG_ALIBABA_CLOUD_ACCESS_KEY_ID;
var keySct = Contracts.DEBUG_ALIBABA_CLOUD_ACCESS_KEY_SECRET;
var domain = Contracts.DEBUG_ALIBABA_DOMAIN;
var ttl = Contracts.DEFAULT_ALIBABA_REQUEST_TTL;
//从环境变量读取
var varKeyId = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_ID);
if (varKeyId == null)
{
    Console.WriteLine($"{Contracts.TITLE}同步失败,未获取到环境变量[{Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_ID}]");
    return;
}
var varKeySecret = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_SECRET);
if (varKeySecret == null)
{
    Console.WriteLine($"{Contracts.TITLE}同步失败,未获取到环境变量[{Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_SECRET}]");
    return;
}
var varDomain = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLUND_DOMAIN);
if (varDomain == null)
{
    Console.WriteLine($"{Contracts.TITLE}同步失败,未获取到环境变量[{Contracts.VAR_ALIBABA_CLUND_DOMAIN}]");
    return;
}
var varTtl = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLOUD_TTL);
key = varKeyId;
keySct = varKeySecret;
domain = varDomain;
if(int.TryParse(varTtl, out int t))
{
    ttl = t;
}
//执行
await AliyunClient.Run(key, keySct, domain, ttl);
