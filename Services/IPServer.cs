using System.Net;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using neverland.aliyun.ddns.Consts;
using neverland.aliyun.ddns.Extensions;
using neverland.aliyun.ddns.Models;

namespace neverland.aliyun.ddns.Services
{
    public class IPServer
    {
        private readonly ILogger _logger;
        public IPServer(ILogger<IPServer> logger) { 
            _logger = logger;
        }

        public async Task<string> GetNetworkIPv4(CancellationToken cancelllationToken = new CancellationToken())
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Contracts.QUERY_IPADDRESS_URL);

                //不使用这个是因为有使用限制,https://ip-api.com/docs/api:json
                //var query = await client.GetFromJsonAsync<IPModelResult>(Contracts.QUERY_IPADDRESS_RESOURCE, cancelllationToken)
                //     .ConfigureAwait(false);

                try
                {
                    using var response = await client.GetAsync(Contracts.QUERY_IPADDRESS_RESOURCE, cancelllationToken)
                     .ConfigureAwait(false);
                    response.EnsureSuccessStatusCode().WriteRequestToConsole();
                    //检查受限情况
                    var ri = response.Headers.FirstOrDefault(it => it.Key == Contracts.QUERY_IPADDRESS_HEADER_RI).Value;
                    if (ri != null)
                    {
                        if (ri.ElementAt(0) == "0")
                        {
                            var ttl = response.Headers.FirstOrDefault(it => it.Key == Contracts.QUERY_IPADDRESS_HEADER_TTL).Value;
                            _logger.LogInformation("{0}ip地址查询受限,等待{1}秒后重试", Contracts.TITLE, ttl.ElementAt(0));
                            return string.Empty;
                        }
                    }
                    var jsonResponse = await response.Content.ReadFromJsonAsync<IPResultModel>(cancellationToken:cancelllationToken);
                    if (jsonResponse != null)
                    {
                        if (jsonResponse.Status != null && jsonResponse.Status == "success")
                        {
                            var networkIp = jsonResponse.Query!;
                            _logger.LogInformation("公网IP:{0}", networkIp);
                            return networkIp;
                        }
                    }
                }
                catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException tex)
                {
                    _logger.LogInformation("Timed out: {0}, {1}", ex.Message, tex.Message);
                }
                catch (HttpRequestException ex) when (ex is { StatusCode: HttpStatusCode.NotFound })
                {
                    // Handle 404
                    _logger.LogInformation("Not found: {0}", ex.Message);
                }
                catch (HttpRequestException ex) when (ex is { StatusCode: null })
                {
                    _logger.LogInformation("网络连接失败: {0}", ex.Message);
                }
            }
            return string.Empty;
        }
    }
}
