using Microsoft.Extensions.Logging;
using neverland.aliyun.ddns.Consts;
using neverland.aliyun.ddns.Extensions;
using neverland.aliyun.ddns.Models;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace neverland.aliyun.ddns.Services
{
    public class IPServer
    {
        private readonly ILogger _logger;
        private IHttpClientFactory _httpClientFactory;
        public IPServer(ILogger<IPServer> logger,IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<IPResultDto> GetNetworkIP(CancellationToken cancelllationToken = new CancellationToken())
        {
            var ret = new IPResultDto();
            try
            {
                var client= _httpClientFactory.CreateClient(Contracts.QUERY_IPIFYADDRESS_NAME);
                using var response = await client.GetAsync("/", cancelllationToken);
                response.EnsureSuccessStatusCode().WriteRequestToConsole();
                var ip = await response.Content.ReadAsStringAsync(cancelllationToken);
                if(IPAddress.TryParse(ip, out var ipAddress))
                {
                    ret.IsSuccess = true;
                    if (ipAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        ret.IsIPv6 = true;
                    }
                    ret.IP = ip;
                }
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogInformation("获取IP错误:{0}", ex.Message);
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
            return ret;
        }
    }
}
