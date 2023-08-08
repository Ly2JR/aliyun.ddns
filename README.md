# 阿里云DDNS同步

- 下载

```command
git clone https://github.com/ly2jr/aliyun.ddns
```

- 签名

  将snk签名文件放入`snk`文件夹加下，没有则取消强名称选项。

- 密钥

1. Docker secret

    ```command
    docker swarm init
    ```

    ```command
    docker secret create \
    --label alikid=阿里云账号 \
    --label alikct=阿里云密码 \
    aliyun_secret ./secret.json
    ```

2. 环境变量

    设置`docker-compose-override.yml`环境变量,

    `ALIKID`：阿里云账号
    `ALIKSCT`：阿里云密码
    `ALIDOMAIN`：域名

- 运行

  ```command
  docker compose up -d
  ```
