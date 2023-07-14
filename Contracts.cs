using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neverland.aliyun.ddns
{
    public sealed class Contracts
    {
        /// <summary>
        /// 注意:请勿生产环境使用,
        /// 生产环境请使用`VAR_ALIBABA_CLOUD_ACCESS_KEY_ID`从环境变量获取
        /// </summary>
        public const string DEBUG_ALIBABA_CLOUD_ACCESS_KEY_ID = "";
        /// <summary>
        /// 注意:请勿生产环境使用
        /// 生产环境请使用`VAR_ALIBABA_CLOUD_ACCESS_KEY_ID`从环境变量获取
        /// </summary>
        public const string DEBUG_ALIBABA_CLOUD_ACCESS_KEY_SECRET = "";

        /// <summary>
        /// 域名
        /// </summary>
        public const string DEBUG_ALIBABA_DOMAIN = "ilyl.life";

        /// <summary>
        /// 服务器地址
        /// <see cref="https://api.aliyun.com/product/Alidns"/>
        /// </summary>
        public const string DEFAULT_ALIBABA_ENDPOINT = "alidns.cn-hangzhou.aliyuncs.com";

        /// <summary>
        /// 主机记录
        /// </summary>
        public const string DEFAULT_ALIBABA_REQUEST_RR = "*";

        /// <summary>
        /// 解析记录类型IP4
        /// </summary>
        public const string DEFAULT_ALIBABA_REQUEST_TYPE_4 = "A";

        /// <summary>
        /// TTL
        /// 单位:s
        /// </summary>
        public const int DEFAULT_ALIBABA_REQUEST_TTL = 600;


        public const string VAR_ALIBABA_CLOUD_ACCESS_KEY_ID = "ALIKID";
        public const string VAR_ALIBABA_CLOUD_ACCESS_KEY_SECRET = "ALIKSCT";
        /// <summary>
        /// 从环境变量获取域名
        /// </summary>
        public const string VAR_ALIBABA_CLUND_DOMAIN = "ALIDOMAIN";
        /// <summary>
        /// 重环境变量获取TTL,单位s，默认600
        /// </summary>
        public const string VAR_ALIBABA_CLOUD_TTL = "ALITTL";

        public const string QUERY_IPADDRESS_URL = "http://ip-api.com/";
        /// <summary>
        /// 查询本地外网地址
        /// 只查询IP地址和状态
        /// <see cref="http://ip-api.com"/>
        /// </summary>
        public const string QUERY_IPADDRESS_RESOURCE = "json/?lang=zh-CN&fields=status,query";

        public const string QUERY_IPADDRESS_HEADER_TTL = "X-Ttl";
        public const string QUERY_IPADDRESS_HEADER_RI = "X-Rl";
    }
}
