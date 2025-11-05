FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

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

RUN apt-get update \
    && apt-get install -y tzdata \
    && ln -fs /usr/share/zoneinfo/Asia/Riyadh /etc/localtime \
    && dpkg-reconfigure -f noninteractive tzdata

ENV TZ=Asia/Riyadh

RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone

RUN date

RUN dotnet publish "./REAC.Api/REAC.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "REAC.Api.dll"]