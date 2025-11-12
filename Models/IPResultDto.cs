using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace neverland.aliyun.ddns.Models
{
    public  class IPResultDto
    {
        public bool IsSuccess { get; set; }

        public string? Result { get; set; }
        public bool IsIPv6 { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string? IP { get; set; }
    }
}
