using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neverland.aliyun.ddns.Consts
{
    public sealed class Contracts
    {
        public const string TITLE = "NEVERLAND.ALIYUN.DDNS - ";

        public const string VAR_PREFIEX = "NEVERLAND__";
        public const string DEFAULT_ALIYUN_DOMAIN = "ilyl.life";
        /// <summary>
        /// 服务器地址
        /// <see cref="https://api.aliyun.com/product/Alidns"/>
        /// </summary>
        public const string DEFAULT_ALIYUN_ENDPOINT = "alidns.cn-hangzhou.aliyuncs.com";

        /// <summary>
        /// 主机记录
        /// </summary>
        public const string DEFAULT_ALIYUN_REQUEST_RR = "*";

        /// <summary>
        /// 解析记录类型IP4
        /// </summary>
        public const string DEFAULT_ALIYUN_REQUEST_TYPE_4 = "A";

        /// <summary>
        /// TTL
        /// 单位:s
        /// </summary>
        public const int DEFAULT_ALIYUN_REQUEST_TTL = 600;

        /// <summary>
        /// 检测频率
        /// 单位：s
        /// </summary>
        public const int DEFAULT_EXECUTION_FREQUENCY = 600;

        /// <summary>
        /// </summary>
        public const string DEFAULT_EXIT_KEY = "q";
        

        public const string QUERY_IPADDRESS_URL = "http://ip-api.com";

        public const string QUERY_IPADDRESS_DOMAIN = "ip-api.com";
        /// <summary>
        /// 查询本地外网地址
        /// 只查询IP地址和状态
        /// <see cref="http://ip-api.com"/>
        /// </summary>
        public const string QUERY_IPADDRESS_RESOURCE = "/json/?lang=zh-CN&fields=status,query";

        public const string QUERY_IPADDRESS_HEADER_TTL = "X-Ttl";
        public const string QUERY_IPADDRESS_HEADER_RI = "X-Rl";
    }
}
