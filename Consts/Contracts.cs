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
        public const string DEFAULT_ALIYUN_REQUEST_TYPE_A = "A";
        /// <summary>
        /// 解析记录类型IP6
        /// </summary>
        public const string DEFAULT_ALIYUN_REQUEST_TYPE_AAAA = "AAAA";

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
        /// 出现网络问题重试
        /// 单位:s
        /// </summary>
        public const int DEFAULT_EXCEPTION_FREQUENCY = 3;

        /// <summary>
        /// </summary>
        public const string DEFAULT_EXIT_KEY = "q";

        #region ipify   
        public const string QUERY_IPIFYADDRESS_NAME="ipify";

        public const string QUERY_IPIFYADDRESS_V4_URL = "https://api.ipify.org";
        public const string QUERY_IPIFYADDRESS_V64_URL = "https://api64.ipify.org";
        #endregion

        #region ipapi
        public const string QUERY_IPADDRESS_NAME = "ipapi";
        public const string QUERY_IPADDRESS_URL = "http://ip-api.com";
        /// <summary>
        /// 查询本地外网地址
        /// 只查询IP地址和状态
        /// <see cref="http://ip-api.com"/>
        /// </summary>
        public const string QUERY_IPADDRESS_RESOURCE = "/json/?lang=zh-CN&fields=status,message,query";

        public const string QUERY_IPADDRESS_HEADER_TTL = "X-Ttl";
        public const string QUERY_IPADDRESS_HEADER_RI = "X-Rl";
        #endregion
    }
}
