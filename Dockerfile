FROM mcr.microsoft.com/dotnet/nightly/sdk:7.0 AS build-env
WORKDIR /neverland.aliyun.ddns

LABEL MAINTAINER=乌龙茶有点甜<982474256@qq.com>

# Copy everything
COPY . ./
# Restore as distinct layers
RUN dotnet restore
# Build and publish a release
RUN dotnet publish -c Release

# Build runtime image
FROM mcr.microsoft.com/dotnet/nightly/runtime:7.0
WORKDIR /neverland.aliyun.ddns
COPY --from=build-env /neverland.aliyun.ddns/bin/Release/net7.0/publish .
ENTRYPOINT ["dotnet", "neverland.aliyun.ddns.dll"]

ENV DOTNET_EnableDiagnostics=0 \
    ALIKID="" \
    ALIKSCT="" \
    ALIDOMAIN="" \
    ALITTL=600

