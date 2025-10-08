FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

USER root
RUN apt-get update && \
    apt-get install -y --no-install-recommends tzdata libgdiplus && \
    ln -snf /usr/share/zoneinfo/Asia/Riyadh /etc/localtime && \
    echo "Asia/Riyadh" > /etc/timezone && \
    dpkg-reconfigure -f noninteractive tzdata && \
    apt-get clean && rm -rf /var/lib/apt/lists/*

USER app

WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release

WORKDIR /api
COPY . .

Run dotnet clean
RUN dotnet build "./REAC.Api/REAC.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

RUN dotnet publish "./REAC.Api/REAC.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

RUN apt-get update && apt-get install -y unzip

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "REAC.Api.dll"]