#define DEBUG

using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Alidns20150109;
using AlibabaCloud.SDK.Alidns20150109.Models;
using AlibabaCloud.TeaUtil;
using AlibabaCloud.TeaUtil.Models;
using neverland.aliyun.ddns;
using Tea;

#region 从环境变量读取
var varKey = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_ID);
if (string.IsNullOrEmpty(varKey))
{
#if DEBUG
    varKey = Contracts.DEBUG_ALIBABA_CLOUD_ACCESS_KEY_ID;
#else
    Console.WriteLine($"{Contracts.TITLE}同步失败,未获取到环境变量[{Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_ID}],3秒后退出...");
    await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(t => Environment.Exit(0)).ConfigureAwait(false);
#endif
}
var varKeySecret = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_SECRET);
if (string.IsNullOrEmpty(varKeySecret))
{
#if DEBUG
    varKeySecret = Contracts.DEBUG_ALIBABA_CLOUD_ACCESS_KEY_SECRET;
#else
    Console.WriteLine($"{Contracts.TITLE}同步失败,未获取到环境变量[{Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_SECRET}],3秒后退出...");
    await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(t => Environment.Exit(0)).ConfigureAwait(false);
#endif
}
var varDomain = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLUND_DOMAIN);
if (string.IsNullOrEmpty(varDomain))
{
#if DEBUG
    varDomain= Contracts.DEBUG_ALIBABA_DOMAIN;
#else
    Console.WriteLine($"{Contracts.TITLE}同步失败,未获取到环境变量[{Contracts.VAR_ALIBABA_CLUND_DOMAIN}],3秒后退出...");
    await Task.Delay(TimeSpan.FromSeconds(3)).ContinueWith(t => Environment.Exit(0)).ConfigureAwait(false);
#endif
}
var varTtl = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLOUD_TTL);
var ttl = Contracts.DEFAULT_ALIBABA_REQUEST_TTL;
if (int.TryParse(varTtl, out int t))
{
    ttl = t;
}
#endregion

var lastNetworkIpAddress = "127.0.0.1";

Console.Clear();
Console.CancelKeyPress += ExitHandler;
void ExitHandler(object? sender, ConsoleCancelEventArgs e)
{
    e.Cancel = true;
    Console.WriteLine("退出程序");
}

Console.WriteLine("Press any key to exit...");

var cts = new CancellationTokenSource();
while (!cts.IsCancellationRequested)
{
    //查询外网地址
    var networkIp = await IPHelper.GetNetworkIPv4();

    if (!string.IsNullOrEmpty(networkIp) && !networkIp.Equals(lastNetworkIpAddress))
    {
        //获取阿里OPENAPI客户端
        var client = CreateClient(varKey, varKeySecret);
        //查询已有记录
        var query = QueryDns(client, varDomain);
        if (query == null)
        {
            Console.WriteLine("查询阿里云信息错误,3秒后退出...");
            cts.CancelAfter(TimeSpan.FromMilliseconds(3_000));
            Environment.Exit(0);
            return;
        }

        var record = query.Body.DomainRecords.Record.FirstOrDefault(o => o.DomainName == varDomain);
        if (record == null)//新增云解析
        {
            var add = AddDns(client, networkIp, varDomain, ttl);
            if (add == null)
            {
                Console.WriteLine("新增阿里云信息错误,3秒后退出...");
                cts.CancelAfter(TimeSpan.FromMilliseconds(3_000));
                Environment.Exit(0);
                return;
            }
            Console.WriteLine($"{Contracts.TITLE}新增云解析成功,域名:{varDomain},地址:{networkIp}");
        }
        else //修改云解析
        {
            var update = UpdateDns(client, record.RecordId, networkIp);
            if (update == null)
            {
                Console.WriteLine("修改阿里云信息错误,3秒后退出...");
                cts.CancelAfter(TimeSpan.FromMilliseconds(3_000));
                Environment.Exit(0);
                return;
            }
            Console.WriteLine($"{Contracts.TITLE}修改云解析成功,域名:{varDomain},地址:{networkIp}");
        }
        //加个公网IP缓存，IP地址变动时更新
        lastNetworkIpAddress = networkIp;
    }
    else
    {
        Console.WriteLine("公网IP无变化，无需更新");
    }

    await Task.Delay(TimeSpan.FromSeconds(Contracts.DEFAULT_EXECUTION_FREQUENCY));
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

