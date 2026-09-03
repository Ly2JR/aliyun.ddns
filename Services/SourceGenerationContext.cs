using neverland.aliyun.ddns.Models;
using System.Text.Json.Serialization;

namespace neverland.aliyun.ddns.Services
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(IPApiResultDto))]
    [JsonSerializable(typeof(IPResultDto))]
    [JsonSerializable(typeof(IPResultModel))]
    public partial class SourceGenerationContext : JsonSerializerContext
    {

    }
}
