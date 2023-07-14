using neverland.aliyun.ddns;

var keyId = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_ID);
var keySecret = Environment.GetEnvironmentVariable(Contracts.VAR_ALIBABA_CLOUD_ACCESS_KEY_SECRET);

//提交阿里云
await AliyunClient.Run(Contracts.DEBUG_ALIBABA_CLOUD_ACCESS_KEY_ID, Contracts.DEBUG_ALIBABA_CLOUD_ACCESS_KEY_SECRET,Contracts.DEBUG_ALIBABA_DOMAIN);
