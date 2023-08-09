# 阿里云DDNS同步

- 下载

```command
git clone https://github.com/ly2jr/aliyun.ddns
```

- 签名

    将snk签名文件放入`snk`文件夹加下，没有则取消强名称选项。

- 环境变量

    `ALIKID`：阿里云账号
    `ALIKSCT`：阿里云密码
    `DOMAIN`：域名，
    `TTL`: 生效时间，默认600

    优先级从高到低
    最高:环境变量
    其次:`appsetting.json`
    最低:`NeverlandOption`

- 运行

  设置`docker-compose-override.yml`环境变量

  ```command
  docker compose up -d
  ```
