#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Install clang/zlib1g-dev dependencies for publishing to native
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
    clang zlib1g-dev
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["GPU.Placeholders.Processing.WorkerService/GPU.Placeholders.Processing.WorkerService.csproj", "GPU.Placeholders.Processing.WorkerService/"]
RUN dotnet restore "./GPU.Placeholders.Processing.WorkerService/GPU.Placeholders.Processing.WorkerService.csproj"
COPY . .
WORKDIR "/src/GPU.Placeholders.Processing.WorkerService"
RUN dotnet build "./GPU.Placeholders.Processing.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./GPU.Placeholders.Processing.WorkerService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=true

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["./GPU.Placeholders.Processing.WorkerService"]