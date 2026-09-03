namespace neverland.aliyun.ddns.Models
{
    /// <summary>
    /// 返回的IP结果
    /// <see cref="http://ip-api.com/json/?lang=zh-CN"/>
    /// </summary>
 
    public record IPResultModel(string Status,string Message,string Query)
    {

        public string? Status { get; set; } = Status;

        public string? Message { get; set; }=Message;

        public string? Query { get; set; } = Query;
    }
}
