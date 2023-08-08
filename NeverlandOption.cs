using neverland.aliyun.ddns.Consts;

namespace neverland.aliyun.ddns
{
    public sealed class NeverlandOption
    {
        public string? ALIKID { get; set; }

        public string? ALIKSCT { get; set; }

        public string? DOMAIN { get; set; } =Contracts.DEFAULT_ALIYUN_DOMAIN;

        public int TTL { get; set; } = Contracts.DEFAULT_ALIYUN_REQUEST_TTL;
    }
}
