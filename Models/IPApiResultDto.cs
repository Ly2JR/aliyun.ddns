using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace neverland.aliyun.ddns.Models
{
    /// <summary>
    /// 返回的IP结果
    /// <see cref="http://ip-api.com/json/?lang=zh-CN"/>
    /// </summary>
 
    public class IPApiResultDto
    {

        public string? Status { get; set; }

        public string? Message { get; set; }

        public string? Query { get; set; }
    }
}
