using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using neverland.aliyun.ddns.Consts;
using System.Diagnostics.CodeAnalysis;

namespace neverland.aliyun.ddns.Services
{
    class DDNSWorker:BackgroundService
    {
        private readonly NeverlandOption _aliyunOption;
        private readonly AliyunServer _aliyunServer;
        private readonly IPServer _ipServer;
        private readonly ILogger _logger;
        private string _lastNetworkIpAddress = "127.0.0.1";
        public DDNSWorker(
            IOptions<NeverlandOption> options,
            ILogger<DDNSWorker> logger,
            AliyunServer aliyunServer,
            IPServer iPServer,
            IConfiguration config)
        {
            _aliyunOption = options.Value;
           
            _logger = logger;
            _aliyunServer = aliyunServer;
            _ipServer= iPServer;

            #region 环境变量
            var alikid = config.AsEnumerable().FirstOrDefault(it => it.Key == nameof(NeverlandOption.ALIKID));
            if (!string.IsNullOrEmpty(alikid.Value))
            {
                _aliyunOption.ALIKID = alikid.Value;
            }

            var aliksct = config.AsEnumerable().FirstOrDefault(it => it.Key == nameof(NeverlandOption.ALIKSCT));
            if (!string.IsNullOrEmpty(aliksct.Value))
            {
                _aliyunOption.ALIKSCT = aliksct.Value;
            }

            var domain = config.AsEnumerable().FirstOrDefault(it => it.Key == nameof(NeverlandOption.DOMAIN));
            if (!string.IsNullOrEmpty(domain.Value))
            {
                _aliyunOption.DOMAIN = domain.Value;
            }
            var ttl = config.AsEnumerable().FirstOrDefault(it => it.Key == nameof(NeverlandOption.TTL));
            if (!string.IsNullOrEmpty(ttl.Value))
            {
                if(int.TryParse(ttl.Value, out int varTtl))
                {
                    if (varTtl >= 600 && varTtl <= 86400)
                    {
                        _aliyunOption.TTL = varTtl;
                    }
                }
            }

            #endregion
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (string.IsNullOrEmpty(_aliyunOption.ALIKID))
            {
                _logger.LogInformation("未检索到阿里账号,3秒后退出...");
                await Task.Delay(TimeSpan.FromMilliseconds(3_000), stoppingToken);
                Environment.Exit(0);
                return;
            }
            if (string.IsNullOrEmpty(_aliyunOption.ALIKSCT))
            {
                _logger.LogInformation("未检索到阿里账号,3秒后退出...");
                await Task.Delay(TimeSpan.FromMilliseconds(3_000), stoppingToken);
                Environment.Exit(0);
                return;
            }
            if (string.IsNullOrEmpty(_aliyunOption.DOMAIN))
            {
                _logger.LogInformation("未检索到域名" +
                    ",3秒后退出...");
                await Task.Delay(TimeSpan.FromMilliseconds(3_000), stoppingToken);
                Environment.Exit(0);
                return;
            }
            while (!stoppingToken.IsCancellationRequested)
            {
                //查询外网地址
                var networkIp = await _ipServer.GetNetworkIPv4(stoppingToken);

                if (!string.IsNullOrEmpty(networkIp) && !networkIp.Equals(_lastNetworkIpAddress))
                {
                    //获取阿里OPENAPI客户端
                    var client = _aliyunServer.CreateClient(_aliyunOption.ALIKID, _aliyunOption.ALIKSCT);
                    //查询已有记录
                    var query = _aliyunServer.QueryDns(client, _aliyunOption.DOMAIN);
                    if (query == null)
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(3_000), stoppingToken);
                        Environment.Exit(0);
                        return;
                    }
                    if (query.Body.DomainRecords.Record.Count == 0)
                    {
                        var add = _aliyunServer.AddDns(client, networkIp, _aliyunOption.DOMAIN, _aliyunOption.TTL);
                        if (add == null)
                        {
                            await Task.Delay(TimeSpan.FromMilliseconds(3_000), stoppingToken);
                            Environment.Exit(0);
                            return;
                        }
                        _logger.LogInformation("{Source}新增云解析成功,域名:{Domain},地址:{ip}", Contracts.TITLE, _aliyunOption.DOMAIN, networkIp);
                    }
                    else
                    {
                        var record = query.Body.DomainRecords.Record.FirstOrDefault(o => o.DomainName == _aliyunOption.DOMAIN && o.Type == Contracts.DEFAULT_ALIYUN_REQUEST_TYPE_4);
                        if (record != null)//新增云解析
                        {
                            if (!record.Value.Equals(networkIp))
                            {
                                var update = _aliyunServer.UpdateDns(client, record.RecordId, networkIp);
                                if (update == null)
                                {
                                    await Task.Delay(TimeSpan.FromMilliseconds(3_000), stoppingToken);
                                    Environment.Exit(0);
                                    return;
                                }
                                _logger.LogInformation("{Source}修改云解析成功,域名:{Domain},地址:{ip}", Contracts.TITLE, _aliyunOption.DOMAIN, networkIp);
                            }
                            else
                            {
                                _logger.LogInformation("公网IP:{ip}无变化，无需同步", networkIp);
                            }
                        }
                    }

                    //加个公网IP缓存，IP地址变动时更新
                    _lastNetworkIpAddress = networkIp;
                }
                else
                {
                    if (!string.IsNullOrEmpty(networkIp))
                    {
                        _logger.LogInformation("公网IP:{ip}无变化，无需同步", networkIp);
                    }
                }

                await Task.Delay(TimeSpan.FromSeconds(Contracts.DEFAULT_EXECUTION_FREQUENCY), stoppingToken);
            }

            await Task.CompletedTask;
        }
    }
}