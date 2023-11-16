#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["neverland.aliyun.ddns.csproj", "."]
RUN dotnet restore "./neverland.aliyun.ddns.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "neverland.aliyun.ddns.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "neverland.aliyun.ddns.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

LABEL MAINTAINER=乌龙茶有点甜<982474256@qq.com>

ENV DOTNET_EnableDiagnostics=0

ENTRYPOINT ["dotnet", "neverland.aliyun.ddns.dll"]

ENV ALIKID= \
    ALIKSCT= \
    ALIDOMAIN=ilyl.life \
    ALITTL=600