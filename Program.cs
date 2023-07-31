#define DEBUG

using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Alidns20150109;
using AlibabaCloud.SDK.Alidns20150109.Models;
using AlibabaCloud.TeaUtil;
using AlibabaCloud.TeaUtil.Models;
using neverland.aliyun.ddns;
using Tea;

//DEBUG默认值
#if DEBUG
var key = Contracts.DEBUG_ALIBABA_CLOUD_ACCESS_KEY_ID;
var keySct = Contracts.DEBUG_ALIBABA_CLOUD_ACCESS_KEY_SECRET;
var domain = Contracts.DEBUG_ALIBABA_DOMAIN;
var ttl = Contracts.DEFAULT_ALIBABA_REQUEST_TTL;
#endif

//从环境变量读取
var varKey = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_ID);
if (varKey == null)
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
key = varKey;
keySct = varKeySecret;
domain = varDomain;
if (int.TryParse(varTtl, out int t))
{
    ttl = t;
}
var lastNetworkIpAddress = "127.0.0.1";

Console.Clear();
Console.CancelKeyPress += ExitHandler;
void ExitHandler(object? sender, ConsoleCancelEventArgs e)
{
    e.Cancel = true;
    Console.WriteLine("退出程序");
}

Console.Write("Press any key,or `q` to exit,or ");
Console.WriteLine("CTRL+C to interrupt the read operation:");

var cancelSource = new CancellationTokenSource();
Run(cancelSource.Token);

var cki = Console.ReadKey(true);
if (cki.Key == ConsoleKey.Q)
{
    cancelSource.Cancel();
}

void Run(CancellationToken cts=new CancellationToken())
{
    Task.Factory.StartNew(async() =>
    {
        while (!cts.IsCancellationRequested)
        {
            //查询外网地址
            var networkIp = await IPHelper.GetNetworkIPv4();          

            if (!string.IsNullOrEmpty(networkIp) && !networkIp.Equals(lastNetworkIpAddress))
            {
                //获取阿里OPENAPI客户端
                var client = CreateClient(key, keySct);
                //查询已有记录
                var query = QueryDns(client, domain);
                if (query != null)
                {
                    var record = query.Body.DomainRecords.Record.FirstOrDefault(o => o.DomainName == domain);
                    if (record == null)//新增云解析
                    {
                        var add = AddDns(client, networkIp, domain, ttl);
                        if (add != null)
                        {
                            //加个公网IP缓存，IP地址变动时更新
                            lastNetworkIpAddress = networkIp;
                            Console.WriteLine($"{Contracts.TITLE}新增云解析成功,域名:{domain},地址:{networkIp}");
                        }
                    }
                    else //修改云解析
                    {
                        var update = UpdateDns(client, record.RecordId, networkIp);
                        if (update != null)
                        {
                            //加个公网IP缓存，IP地址变动时更新
                            lastNetworkIpAddress = networkIp;
                            Console.WriteLine($"{Contracts.TITLE}修改云解析成功,域名:{domain},地址:{networkIp}");
                        }
                    }
                }
            }

            await Task.Delay(TimeSpan.FromSeconds(Contracts.DEFAULT_EXECUTION_FREQUENCY));
        }
    },cts);
}


#region 阿里OPENAPI
/**
        * 使用AK&SK初始化账号Client
        * @param accessKeyId
        * @param accessKeySecret
        * @return Client
        * @throws Exception
        */
Client CreateClient(string accessKey, string accessKeySecret)
{
    Config config = new Config
    {
        // 必填，您的 AccessKey ID
        AccessKeyId = accessKey,
        // 必填，您的 AccessKey Secret
        AccessKeySecret = accessKeySecret,
    };
    // Endpoint 请参考 https://api.aliyun.com/product/Alidns
    config.Endpoint = Contracts.DEFAULT_ALIBABA_ENDPOINT;
    return new Client(config);
}

UpdateDomainRecordResponse? UpdateDns(Client client, string recordId, string newIp, string RR = Contracts.DEFAULT_ALIBABA_REQUEST_RR, string type = Contracts.DEFAULT_ALIBABA_REQUEST_TYPE_4)
{
    UpdateDomainRecordRequest updateDomainRecordRequest = new UpdateDomainRecordRequest()
    {
        RecordId = recordId,
        RR = RR,
        Type = type,
        Value = newIp,
    };
    RuntimeOptions runtime = new RuntimeOptions();
    try
    {
        // 复制代码运行请自行打印 API 的返回值
        var response = client.UpdateDomainRecordWithOptions(updateDomainRecordRequest, runtime);
        return response;
    }
    catch (TeaException error)
    {
        // 如有需要，请打印 error
        var msg = Common.AssertAsString(error.Message);
        Console.WriteLine($"{Contracts.TITLE}修改云解析失败,{msg}");
    }
    catch (Exception _error)
    {
        TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
        // 如有需要，请打印 error
        var msg = Common.AssertAsString(error.Message);
        Console.WriteLine($"{Contracts.TITLE}修改云解析失败,{msg}");
    }
    return null;
}

DescribeDomainRecordsResponse? QueryDns(Client client, string domain = Contracts.DEBUG_ALIBABA_DOMAIN, string type = Contracts.DEFAULT_ALIBABA_REQUEST_TYPE_4)
{
    DescribeDomainRecordsRequest describeDomainRecordsRequest = new DescribeDomainRecordsRequest()
    {
        DomainName = domain,
        Type = type
    };
    RuntimeOptions runtime = new RuntimeOptions();
    try
    {
        // 复制代码运行请自行打印 API 的返回值
        var response = client.DescribeDomainRecordsWithOptions(describeDomainRecordsRequest, runtime);
        return response;
    }
    catch (TeaException error)
    {
        // 如有需要，请打印 error
        var msg = Common.AssertAsString(error.Message);
        Console.WriteLine($"{Contracts.TITLE}查询云解析失败,{msg}");
    }
    catch (Exception _error)
    {
        TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
        // 如有需要，请打印 error
        var msg = Common.AssertAsString(error.Message);
        Console.WriteLine($"{Contracts.TITLE}查询云解析失败,{msg}");
    }
    return null;
}

AddDomainRecordResponse? AddDns(Client client, string newIp, string domain = Contracts.DEBUG_ALIBABA_DOMAIN, int ttl = Contracts.DEFAULT_ALIBABA_REQUEST_TTL)
{
    //参数
    AddDomainRecordRequest addDomainRecordRequest = new AddDomainRecordRequest()
    {
        DomainName = domain,//域名名称
        RR = Contracts.DEFAULT_ALIBABA_REQUEST_RR,//主机记录
        Type = Contracts.DEFAULT_ALIBABA_REQUEST_TYPE_4,//解析记录类型
        Value = newIp,//记录值
        TTL = ttl
    };
    //运行时高级配置
    RuntimeOptions runtime = new RuntimeOptions();
    try
    {
        // 复制代码运行请自行打印 API 的返回值
        var response = client.AddDomainRecordWithOptions(addDomainRecordRequest, runtime);
        return response;
    }
    catch (TeaException error)
    {
        // 如有需要，请打印 error
        var msg = Common.AssertAsString(error.Message);
        Console.WriteLine($"{Contracts.TITLE}新增云解析失败,{msg}");
    }
    catch (Exception _error)
    {
        TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
        // 如有需要，请打印 error
        var msg = Common.AssertAsString(error.Message);
        Console.WriteLine($"{Contracts.TITLE}新增云解析失败,{msg}");
    }
    return null;
}

#endregion

