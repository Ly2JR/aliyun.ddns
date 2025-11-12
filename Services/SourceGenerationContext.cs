using neverland.aliyun.ddns.Models;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace neverland.aliyun.ddns.Services
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(IPApiResultDto))]
    public partial class SourceGenerationContext : JsonSerializerContext
    {

    }
}
