# 阿里云DDNS同步

## 部署

- 下载

```command
git clone https://github.com/ly2jr/aliyun.ddns
```

- 部署

更改`docker-compose.yml`环境变量,

`ALIKID`：阿里云账号
`ALIKSCT`：阿里云密码
`ALIDOMAIN`：域名

```command
docker compose up -d
```
