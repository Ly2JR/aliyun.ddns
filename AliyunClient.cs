using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Alidns20150109;
using AlibabaCloud.SDK.Alidns20150109.Models;
using AlibabaCloud.TeaUtil;
using AlibabaCloud.TeaUtil.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tea;

namespace neverland.aliyun.ddns
{
    public abstract class AliyunClient
    {
        /**
         * 使用AK&SK初始化账号Client
         * @param accessKeyId
         * @param accessKeySecret
         * @return Client
         * @throws Exception
         */
        private static Client CreateClient(string accessKeyId, string accessKeySecret)
        {
            Config config = new Config
            {
                // 必填，您的 AccessKey ID
                AccessKeyId = accessKeyId,
                // 必填，您的 AccessKey Secret
                AccessKeySecret = accessKeySecret,
            };
            // Endpoint 请参考 https://api.aliyun.com/product/Alidns
            config.Endpoint =Contracts.DEFAULT_ALIBABA_ENDPOINT;
            return new Client(config);
        }

        public static async Task Run(string accessKeyId, string accessKeySecret,string domain,int ttl=600)
        {
            //查询外网地址
            var networkIp = await IPHelper.GetNetworkIPv4();
            if (string.IsNullOrEmpty(networkIp)) return;

            //
            var client = CreateClient(accessKeyId, accessKeySecret);
            //参数
            AddDomainRecordRequest addDomainRecordRequest = new AddDomainRecordRequest()
            {
                DomainName = domain,//域名名称
                RR = Contracts.DEFAULT_ALIBABA_REQUEST_RR,//主机记录
                Type = "A",//解析记录类型
                Value = networkIp,//记录值
                TTL = ttl
            };
            //运行时高级配置
            RuntimeOptions runtime = new RuntimeOptions();
            try
            {
                // 复制代码运行请自行打印 API 的返回值
               var response= client.AddDomainRecordWithOptions(addDomainRecordRequest, runtime);
               var aa = response;
            }
            catch (TeaException error)
            {
                Console.WriteLine($"阿里云DDNS同步失败,{error.Code},{error.Message}");
                //// 如有需要，请打印 error
                //Common.AssertAsString(error.Message);
            }
            catch (Exception _error)
            {
                TeaException error = new TeaException(new Dictionary<string, object>
                {
                    { "message", _error.Message }
                });
                Console.WriteLine($"阿里云DDNS同步失败,{error.Code},{error.Message}");
                //// 如有需要，请打印 error
                //Common.AssertAsString(error.Message);
            }
        }
    }
}
