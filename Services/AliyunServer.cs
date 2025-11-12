using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Alidns20150109;
using AlibabaCloud.SDK.Alidns20150109.Models;
using AlibabaCloud.TeaUtil;
using AlibabaCloud.TeaUtil.Models;
using Microsoft.Extensions.Logging;
using neverland.aliyun.ddns.Consts;
using Tea;

namespace neverland.aliyun.ddns.Services
{
    internal class AliyunServer
    {

        private readonly ILogger _logger;
        public AliyunServer(ILogger<AliyunServer> logger) {
            _logger = logger;
        }

        #region 阿里OPENAPI
        /**
                * 使用AK&SK初始化账号Client
                * @param accessKeyId
                * @param accessKeySecret
                * @return Client
                * @throws Exception
                */
        public Client CreateClient(string accessKey, string accessKeySecret)
        {
            var config = new Config()
            {
                // 必填，您的 AccessKey ID
                AccessKeyId = accessKey,
                // 必填，您的 AccessKey Secret
                AccessKeySecret = accessKeySecret,
                // Endpoint 请参考 https://api.aliyun.com/product/Alidns
                Endpoint = Contracts.DEFAULT_ALIYUN_ENDPOINT
            };
            return new Client(config);
        }

        public UpdateDomainRecordResponse? UpdateDns(Client client, string recordId, string newIp, string type, string RR = Contracts.DEFAULT_ALIYUN_REQUEST_RR)
        {
            var updateDomainRecordRequest = new UpdateDomainRecordRequest()
            {
                RecordId = recordId,
                RR = RR,
                Type = type,
                Value = newIp,
            };
            var runtime = new RuntimeOptions();
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
                _logger.LogInformation("{0}修改云解析失败,{1}", Contracts.TITLE,msg);
            }
            catch (Exception _error)
            {
                var error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                var msg = Common.AssertAsString(error.Message);
                _logger.LogInformation("{0}修改云解析失败,{1}", Contracts.TITLE,msg);
            }
            return null;
        }

        public DescribeDomainRecordsResponse? QueryDns(Client client, string domain, string type)
        {
            var describeDomainRecordsRequest = new DescribeDomainRecordsRequest()
            {
                DomainName = domain,
                Type = type
            };
            var runtime = new RuntimeOptions();
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
                _logger.LogInformation("{0}查询云解析失败,{1}", Contracts.TITLE,msg);
            }
            catch (Exception _error)
            {
                var error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                var msg = Common.AssertAsString(error.Message);
                _logger.LogInformation("{0}查询云解析失败,{1}", Contracts.TITLE,msg);
            }
            return null;
        }

        public AddDomainRecordResponse? AddDns(Client client, string newIp, string domain,string type, int ttl = Contracts.DEFAULT_ALIYUN_REQUEST_TTL)
        {
            //参数
            var addDomainRecordRequest = new AddDomainRecordRequest()
            {
                DomainName = domain,//域名名称
                RR = Contracts.DEFAULT_ALIYUN_REQUEST_RR,//主机记录
                Type =type,//解析记录类型
                Value = newIp,//记录值
                TTL = ttl
            };
            //运行时高级配置
            var runtime = new RuntimeOptions();
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
                _logger.LogInformation("{0}新增云解析失败,{1}", Contracts.TITLE,msg);
            }
            catch (Exception _error)
            {
                var error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                // 如有需要，请打印 error
                var msg = Common.AssertAsString(error.Message);
                _logger.LogInformation("{0}新增云解析失败,{1}", Contracts.TITLE,msg);
            }
            return null;
        }

        #endregion
    }
}
